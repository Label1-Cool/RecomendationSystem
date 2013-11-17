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
        #region Fields and Properties
        Cluster[] totalArrayClusters;

        //таблица "пользователи/кластеры" в виде словаря
        Dictionary<KeyValuePair<int, string>, Dictionary<string, double>> allUserCluster = new Dictionary<KeyValuePair<int, string>, Dictionary<string, double>>();
        //таблица "направления обучения/кластеры" в виде словаря
        Dictionary<KeyValuePair<int, string>, Dictionary<string, double>> allEducationLineCluster = new Dictionary<KeyValuePair<int, string>, Dictionary<string, double>>();

        //простое табличное представление  "пользователи/кластеры"
        double[,] matrixUserCluster;
        //простое табличное представление  "направления обучения/кластеры"
        double[,] matrixEducationLineCluster;

        public List<UserAnalyzed> UsersAnalysed { get; set; }
        public List<ClusterAnalyzed> UsersClustersAnalysed { get; set; }

        public List<EducationLineAnalyzed> EducationLinesAnalysed { get; set; }
        public List<ClusterAnalyzed> EducationLinesClustersAnalysed { get; set; }
        #endregion

        public async Task Init()
        {
            var task = Task.Factory.StartNew(() =>
                {
                    CalculateInfoForLSA();

                    //CalculateInfoForParretoSet();
                });
            await task;
        }

        #region LSA


        private void CalculateInfoForLSA()
        {
            //Получаем полный список доступных кластеров
            using (var context = new RecomendationSystemModelContainer())
            {
                totalArrayClusters = context.Clusters.ToArray();
            }

            //Строим таблицу пользователь/кластер
            AnalyseAllUserCluster();
            //Строим таблицу "направление обучения/кластер"
            AnalyseAllEducationLineCluster();

            //На выходе что то вроде:
            //Вася: Русскоий-123
            //      Математика 170
            //      ...
            //Получаем из нее более простую матрицу
            matrixUserCluster = CalculateMatrix(allUserCluster);
            //аналогично для направлений обучения
            matrixEducationLineCluster = CalculateMatrix(allEducationLineCluster);

            //Проводим LSA анализ и получаем координаты для пользователей и кластеров
            CalculateUserAndClusterCoord();
            CalculateEducationLineAndClusterCoord();
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
                        int mark = stateExam.Result;

                        foreach (var weight in stateExam.Discipline.Weight)
                        {
                            var calcResult = mark * weight.Coefficient;
                            //Прибавляет заданному кластуру значение. Именно прибавляет.
                            //Т.е. если у нас есть к примеру есть значение по математике, 
                            //и мы смотрим информатику,которая в свою очередь имеет вклад и в Информатику, и в Математику(меньший). 
                            //Соответсвенно начальное значение математики увеличится
                            clusterResult[weight.Cluster.Name] += calcResult;
                        }
                    }
                    allUserCluster.Add(new KeyValuePair<int,string>(user.Id,user.FirstName), clusterResult);

                    currentStep++;
                }
            }
        }

        /// <summary>
        /// Ищет в бд информацию о направлениях, из которой строит "таблицу": направления/кластеры. Реузльтат в переменной allEducationLineCluster
        /// </summary>
        private void AnalyseAllEducationLineCluster()
        {
            using (var context = new RecomendationSystemModelContainer())
            {
                int countSteps = context.Users.Count();
                int currentStep = 0;

                //берем все направления
                foreach (var educationLine in context.DepartmentEducationLines)
                {
                    Dictionary<string, double> clusterResult = new Dictionary<string, double>();
                    foreach (var item in totalArrayClusters)
                    {
                        clusterResult.Add(item.Name, 0);
                    }
                    //заполняем оценками и кластерами
                    foreach (var requirement in educationLine.DepartmentLinesRequirement)
                    {
                        //Получаем текущую дисциплину и ее весовые коэффициенты. Запонляем : Дисциплина/кластер
                        int mark = requirement.Requirement;

                        foreach (var weight in requirement.Discipline.Weight)
                        {
                            var calcResult = mark * weight.Coefficient;
                            //Прибавляет заданному кластеру значение. Именно прибавляет.
                            //Т.е. если у нас есть к примеру есть значение по математике, 
                            //и мы смотрим информатику,которая в свою очередь имеет вклад и в Информатику, и в Математику(меньший). 
                            //Соответсвенно начальное значение математики увеличится
                            clusterResult[weight.Cluster.Name] += calcResult;
                        }
                    }
                    allEducationLineCluster.Add(new KeyValuePair<int, string>(educationLine.Id, educationLine.Name), clusterResult);

                    currentStep++;
                }
            }
        }

        /// <summary>
        /// Строит матрицу для LSA из словаря
        /// </summary>
        /// <param name="clusterDictionary">Словарь, из которого строится матрица</param>
        /// <returns>Построенная матрица</returns>
        private  double[,] CalculateMatrix(Dictionary<KeyValuePair<int, string>, Dictionary<string, double>> clusterDictionary)
        {
            //Представляем нашу таблицу в более простом виде
            int row = clusterDictionary.Count;
            int column = clusterDictionary.FirstOrDefault().Value.Count;
            var matrixCluster = new double[row, column];

            var allUserClusterArray = clusterDictionary.ToArray();
            for (int i = 0; i < row; i++)
            {
                var nuc2 = allUserClusterArray[i].Value.Values.ToArray();
                for (int j = 0; j < column; j++)
                {
                    matrixCluster[i, j] = nuc2[j];
                }
            }
            return matrixCluster;
        }

        private void CalculateUserAndClusterCoord()
        {
            LSA lsa = new LSA(matrixUserCluster);
            var ClusterCoords = lsa.FirtsCoords;
            var ItemCoords = lsa.SecondCoords;
            int row = matrixUserCluster.GetLength(0);
            int column = matrixUserCluster.GetLength(1);

            //Проанализируем пользователей
            UsersAnalysed = new List<UserAnalyzed>();
            for (int i = 0; i < row; i++)
            {
                UsersAnalysed.Add(
                    new UserAnalyzed(allUserCluster.ToArray()[i].Key, ItemCoords[i]));
            }

            //Проанализируем кластеры обучения
            UsersClustersAnalysed = new List<ClusterAnalyzed>();
            for (int i = 0; i < column; i++)
            {
                UsersClustersAnalysed.Add(
                    new ClusterAnalyzed(totalArrayClusters[i].Name, ClusterCoords[i]));
            }

        }
        private void CalculateEducationLineAndClusterCoord()
        {
            LSA lsa = new LSA(matrixEducationLineCluster);
            var ClusterCoords = lsa.FirtsCoords;
            var ItemCoords = lsa.SecondCoords;
            int row = matrixEducationLineCluster.GetLength(0);
            int column = matrixEducationLineCluster.GetLength(1);

            //Проанализируем направления обучения
            EducationLinesAnalysed = new List<EducationLineAnalyzed>();
            for (int i = 0; i < row; i++)
            {
                EducationLinesAnalysed.Add(
                    new EducationLineAnalyzed(allEducationLineCluster.ToArray()[i].Key, ItemCoords[i]));
            }

            //Проанализируем кластеры обучения
            EducationLinesClustersAnalysed = new List<ClusterAnalyzed>();
            for (int i = 0; i < column; i++)
            {
                EducationLinesClustersAnalysed.Add(
                    new ClusterAnalyzed(totalArrayClusters[i].Name, ClusterCoords[i]));
            }
        }
        #endregion

        #region Pareto
        /// <summary>
        /// Получает иноформацию из бд и приводит ее к матричному виду для дальнейшей обработки алгоритмом построения мн-ва Парето.
        /// В данной версии считается, что порядок названий в бд меняться на протяжении работы не будет(так по идее и должно быть)
        /// </summary>
        private void CalculateInfoForParretoSet()
        {
            List<EducationLineAndRequirementRow> educationLinesAndRequirementTable;
            using(var context = new RecomendationSystemModelContainer())
            {
                //TODO: Направлению обучения не хватает названия(одного когда не достаточно)
                
                //Получаем списки всех нарпавлений обучения + информация о требованиях
                educationLinesAndRequirementTable = 
                    (from edLines in context.DepartmentEducationLines
                     where edLines.DepartmentLinesRequirement.Count==3//пока захардкодим(Должно быть обязательно указано 3 экзамена егэ (мин.баллы)
                        select new EducationLineAndRequirementRow()
                        {
                            Id=edLines.Id,
                            Code = edLines.Code,
                            Requirements = (from requirement in edLines.DepartmentLinesRequirement
                                            select requirement.Requirement).ToList()
                        }).ToList();
                //Получаем остальные списки: оценки, увлечения, предпочтения
            }
            if (educationLinesAndRequirementTable.Count > 0)
            {
                //Дальше алгоритм построения множества Паретто
                MulticriterialAnalysis multicriterialAnalysis = new MulticriterialAnalysis();
                var parretoSet = multicriterialAnalysis.ParretoSetCreate(educationLinesAndRequirementTable);
                //Теперь необходимо сузить наше множество:
                #region 1. Выявлении информации об относительной важности критериев
                //Опрос ЛПР
                //TODO: Подумать над возможностью циклического уточнения, с опросом уточяющей информации на каждом шаге
                //+ проверке непротиворечивости нескольких утверждений
                #endregion
                #region 2. Замена менее важных криетриев новыми
                //Пока сделал 1 шаг с фиксированными коэффициентами(нужно обдумать что они правильные). ЛПР в данном случае -мы(я)
                //Пусть у нас 3 критерия  - минимальные баллы по предметам
                //На сколько мы готовы пожертвовать русским ради математикик и профильного предмета(информатика)

                //когда один критерий важнее второго, а он, в свою очередь,
                //важнее третьего (здесь можно дважды применить теорему 2.5 —
                //сначала пересчитывается третий критерий, а затем второй; см.
                //п. 1 разд. 4.1)
                //Пусть русский<математика<информатика
                //      80      70 
                //      30      60
                //      60      30
                //Пересчитываем сначала русский, потом математику
                //      72
                //      72
                //      72
                //...
                double tetaMath_Rus = multicriterialAnalysis.GetTeta(20, 10);
                double tetaProf_Math = multicriterialAnalysis.GetTeta(20, 5);
                //Определить на каких позициях стоит какой предмет и пересчить поп формуле. Пусть для начала считаем что на 0 - русский, на 1 - математика, на 3-информатика
                foreach (var row in parretoSet)
                {
                    //teta*болееВажный + (1-teta)* менееВажный
                    //Пересчитываем русский 70 80 -> 72 80
                    //Пересчитываем информатику 80 90-> .. ..
                    //...
                    for (int i = 0; i < row.Requirements.Count - 1; i++)
                    {
                        row.Requirements[i] = RecalculateElementsRow(row.Requirements[i + 1], row.Requirements[i], tetaMath_Rus);
                    }
                }
                //или
                //когда два критерия по отдельности важнее третьего (тогда для формирования 
                //нового векторного критерия следует использовать формулу (4.7)

                //или еще варианты...


                //TODO: Решить как распределять коэффициенты и какой у нас случай. По идее мы можем сгруппировать наши требования в группы: ЕГЭ, Оценки, Увлечения и пр.
                //В каждой из них - есть свои требования(балы по математике, русскому, информтатике) которые так же сравнимы... как быть. Счить все как 1 кучу и выявять нужные или по группам.
                //Есть идея сделать рекурсивно:
                //Сначала считаем на вершинах дерева - сравниваем Егэ по русскому, математике и информатике. Получим равнозначные критерии в итоге.
                //Потом сравниваем полученную группу из 3-х критериев с остальными.

                //Самый простой вариант на мой взгялд - заранее определить важность в виде a1<a2<a3<a4<...<an
                //После этого упорядочить наш список критериев(в бд или в коде)
                #endregion
                #region Нахождение нового мноежства паретто
                var parretoSetNew = multicriterialAnalysis.ParretoSetCreate(parretoSet);
                #endregion
                #region 3. Анализ ЛПР-ом получившихся значений
                
                #endregion

            }
            else
                throw new Exception("Не удалось получить списко информации об направлениях обучения и требованиях");

        }
        private int RecalculateElementsRow(int ImpotantElement, int notImpotantElement, double teta)
        {
            return (int)(teta * ImpotantElement + (1 - teta) * notImpotantElement);
        }
        #endregion
    }
    public class EducationLineAndRequirementRow
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public List<int> Requirements { get; set; }
    }
}
