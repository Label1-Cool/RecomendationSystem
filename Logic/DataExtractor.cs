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
    /// Вспомогательтный класс для извлечения информации из бд и представления ее в удобном для дальнейшей работы виде
    /// </summary>
    public  class DataExtractor
    {
        //static Dictionary<string, double> totalClusters;
         Cluster[] totalArrayClusters;
        //таблица пользователи/кластеры в виде словаря
         Dictionary<string, Dictionary<string, double>> allUserCluster = new Dictionary<string, Dictionary<string, double>>();
        //простое табличное представление  пользователи/кластеры
         double[,] matrixUserCluster;

        public  List<UserAnalyzed> UsersCoords { get; set; }
        public  List<ClusterAnalyzed> ClustersCoords { get; set; }

        public DataExtractor()
        {
            //Init();
        }

        public async Task Init(IProgress<int> progress)
        {
            var task = Task.Factory.StartNew(() =>
                {
                    //Получаем полный список доступных кластеров
                    using (var context = new RecomendationSystemModelContainer())
                    {
                        totalArrayClusters = context.Clusters.ToArray();
                    }
                    //Строим таблицу пользователь/кластер
                    AnalyseAllUserCluster(progress);
                    //Получаем из нее более простую матрицу
                    CalculateMatrix();
                    //Проводим LSA анализ и получаем координаты для пользователей и кластеров
                    CalculateUserAndClusterCoord();
                });
            await task;
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
        private void AnalyseAllUserCluster(IProgress<int> progress)
        {
            if (progress==null)
            {
                throw new ArgumentNullException("Progress is null");
            }

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
                    progress.Report((currentStep * 100) / countSteps);
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
