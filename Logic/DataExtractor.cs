using DataLayer;
using Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Logic
{
    /// <summary>
    /// Вспомогательтный класс для извлечения информации из бд и представления ее в удобном для дальнейшей обработки методами виде
    /// </summary>
    public  class DataExtractor
    {
         Cluster[] totalArrayClusters;
        //таблица пользователи/кластеры в виде словаря
         Dictionary<string, Dictionary<string, double>> allUserCluster = new Dictionary<string, Dictionary<string, double>>();
        //простое табличное представление  пользователи/кластеры
         double[,] matrixUserCluster;

        public  List<UserAnalyzed> UsersCoords { get; set; }
        public  List<ClusterAnalyzed> ClustersCoords { get; set; }


        public async Task Init()
        {
            var task = Task.Factory.StartNew(() =>
                {
                    CalculateInfoForLSA();

                    CalculateInfoForParretoSet();
                });
            await task;
        }

        /// <summary>
        /// Получает иноформацию из бд и приводит ее к матричному виду для дальнейшей обработки алгоритмом построения мн-ва Парето.
        /// В данной версии считается, что порядок названий в бд меняться на протяжении работы не будет(так по идее и должно быть)
        /// </summary>
        private void CalculateInfoForParretoSet()
        {
            using(var context = new RecomendationSystemModelContainer())
            {
                //TODO: Направлению обучения не хватает названия(одного когда не достаточно)
                
                //Получаем списки всех нарпавлений обучения + информация о требованиях
                var educationLinesAndRequirementTable = (from edLines in context.DepartmentEducationLines
                        select new
                        {
                            Id=edLines.Id,
                            EducationCode = edLines.Code,
                            Requrement = (from requirement in edLines.DepartmentLinesRequirement
                                         select requirement.Requirement)
                        }).ToList();
                //Получаем остальные списки: оценки, увлечения, предпочтения
                
                var finalParetto = educationLinesAndRequirementTable;
                //Тут 2 варианта:
                //1. Получаем 1 общий список со всеми прараметрами и строим мн-во Парето
                //2. Или несколько, и тогда находим паретто для каждого и ищем пересечение
                //(Пока делаем только 1-й)
                for (int i=0;i<educationLinesAndRequirementTable.Count;i++)
                {
                    var comparedItemRequirements = educationLinesAndRequirementTable[i].Requrement.ToArray();
                    for (int j=i+1;j<educationLinesAndRequirementTable.Count;j++)
                    {
                        var itemRequirements = educationLinesAndRequirementTable[j].Requrement.ToArray();
                        bool comparedIsMore = true;
                        for (int k = 0; k < comparedItemRequirements.Count(); k++)
                        {
                            if (comparedItemRequirements[k] < itemRequirements[k])
                            {
                                //переход к шагу 5 алгоритма
                                comparedIsMore = false;
                                break;
                            }
                        }

                        if (comparedIsMore)
                        {
                            //т.к наш вектор оказался больше другого, удлаяем другой из общего множества
                            educationLinesAndRequirementTable.Remove(educationLinesAndRequirementTable[j]);
                            //т.к. массив по которому мы проходим поменялся
                            --j;
                            --i;
                        }
                        else//проверяем на равенство эти же объекты, но в другом порядке(шаг 5)
                        {
                            bool itemIsMore = true;
                            for (int k = 0; k < comparedItemRequirements.Count(); k++)
                            {
                                if ( itemRequirements[k]< comparedItemRequirements[k])
                                {
                                    itemIsMore = false;
                                    break;
                                }
                            }
                            if (itemIsMore)
                            {
                                //т.к наш вектор оказался больше другого, удлаяем другой из общего множества
                                educationLinesAndRequirementTable.Remove(educationLinesAndRequirementTable[i]);
                                //т.к. массив по которому мы проходим поменялся
                                --j;
                                --i;
                            }
                            else
                                ++j;
                        }
                        
                    }
                }

                //Дальше алгоритм построения множества паретто
                //Сравниваем направления обучения по требованиям
                


            }
        }

        private void CalculateInfoForLSA()
        {
            //Получаем полный список доступных кластеров
            using (var context = new RecomendationSystemModelContainer())
            {
                totalArrayClusters = context.Clusters.ToArray();
            }
            //Строим таблицу пользователь/кластер
            AnalyseAllUserCluster();
            //На выходе что то вроде:
            //Вася: Русскоий-123
            //      Математика 170
            //      ...
            //Получаем из нее более простую матрицу
            CalculateMatrix();
            //Проводим LSA анализ и получаем координаты для пользователей и кластеров
            CalculateUserAndClusterCoord();
        }
        public  Dictionary<string, double> AnalyseUserCluster(string userName)
        {
            using (var context = new RecomendationSystemModelContainer())
            {
                //берем какого то пользователя
                var user = (from u in context.Users
                            where u.FirstName == userName
                            select u).FirstOrDefault();
                if (user == null)
                {
                    throw new NullReferenceException();
                }

                //Dictionary<string, double> clusterResult = totalClusters;
                Dictionary<string, double> clusterResult = new Dictionary<string, double>();
                foreach (var item in totalArrayClusters)
                {
                    clusterResult.Add(item.Name, 0);
                }

                //заполняем оценками и кластерами
                foreach (var stateExam in user.UnitedStateExam)
                {
                    //Получаем текущую дисциплину и ее весовые коэффициенты. Запонляем : Дисциплина/кластер
                    string discipline = stateExam.Discipline.Name;
                    int mark = stateExam.Result;

                    foreach (var weight in stateExam.Discipline.Weight)
                    {
                        double coeff = weight.Coefficient;
                        var calcResult = mark * coeff;
                        string name = weight.Cluster.Name;

                        clusterResult[name] += calcResult;
                    }
                }

                //analyseUserCluster = clusterResult
                //    .OrderByDescending(elem => elem.Value)
                //    .ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
                return clusterResult;
            }
        }

        /// <summary>
        /// Ищет в бд информацию о пользователях, из которой строит "таблицу": пользователи/кластеры. Реузльтат в переменной allUserCluster
        /// </summary>
        private void AnalyseAllUserCluster()
        {
            using (var context = new RecomendationSystemModelContainer())
            {
                int countSteps=context.Users.Count();
                int currentStep=0;
                //берем всех пользователей
                foreach (var user in context.Users)
                {
                    Dictionary<string, double> clusterResult = new Dictionary<string, double>();
                    foreach (var item in totalArrayClusters)
                    {
                        clusterResult.Add(item.Name, 0);
                    }
                    //заполняем оценками и кластерами
                    foreach (var stateExam in user.UnitedStateExam)
                    {
                        //Получаем текущую дисциплину и ее весовые коэффициенты. Запонляем : Дисциплина/кластер
                        string discipline = stateExam.Discipline.Name;
                        int mark = stateExam.Result;

                        foreach (var weight in stateExam.Discipline.Weight)
                        {
                            double coeff = weight.Coefficient;
                            var calcResult = mark * coeff;
                            string name = weight.Cluster.Name;

                            clusterResult[name] += calcResult;
                        }
                    }
                    allUserCluster.Add(user.FirstName, clusterResult);

                    currentStep++;
                }
            }
        }
        /// <summary>
        /// Строит матрицу для LSA на основе всех данных из бд
        /// </summary>
        private  void CalculateMatrix()
        {
            //Представляем нашу таблицу в более простом виде
            int row = allUserCluster.Count;
            int column = allUserCluster.FirstOrDefault().Value.Count;
            matrixUserCluster = new double[row, column];

            var allUserClusterArray = allUserCluster.ToArray();
            for (int i = 0; i < row; i++)
            {
                var nuc2 = allUserClusterArray[i].Value.Values.ToArray();
                for (int j = 0; j < column; j++)
                {
                    matrixUserCluster[i, j] = nuc2[j];
                }
            }
        }

        private  void CalculateUserAndClusterCoord()
        {
            LSA lsa = new LSA(matrixUserCluster);
            var ClusterCoords = lsa.FirtsCoords;
            var UserCoords = lsa.SecondCoords;

            //создаем списки с результатами
            //Для кластеров
            var allUserClusterArray = allUserCluster.ToArray();
            int row = allUserCluster.Count;
            int column = allUserCluster.FirstOrDefault().Value.Count;

            ClustersCoords = new List<ClusterAnalyzed>();
            var clustersArray = allUserClusterArray[0].Value.ToArray();
            for (int i = 0; i < column; i++)
            {
                ClustersCoords.Add(
                    new ClusterAnalyzed(clustersArray[i].Key, ClusterCoords[i]));
            }

            //Для пользователей
            UsersCoords = new List<UserAnalyzed>();
            for (int i = 0; i < row; i++)
            {
                UsersCoords.Add(
                    new UserAnalyzed(allUserClusterArray[i].Key, UserCoords[i]));
            }
        }
    }
}
