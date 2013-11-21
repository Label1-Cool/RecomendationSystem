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
    public sealed class DataExtractor
    {
        #region Singleton Pattern
        private static volatile DataExtractor instance;
        private static object syncRoot = new Object();

        private DataExtractor() { }
        public static DataExtractor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DataExtractor();
                    }
                }

                return instance;
            }
        }
        #endregion
        #region Fields and Properties
        Cluster[] totalArrayClusters;

        //таблица "пользователи/кластеры"
        ItemToClusterCell[,] allUserCluster;
        //таблица "направления обучения/кластеры"
        ItemToClusterCell[,] allEducationLinesCluster;
        //таблица "направления обучения/кластеры" в виде словаря
        UserToEducationLineCell[,] allUserEducationLine;

        public List<UserClusterAnalyzed> UsersAnalysed { get; set; }
        public List<ClusterAnalyzed> UsersClustersAnalysed { get; set; }

        public List<EducationLineToClusterAnalyzed> EducationLinesAnalysed { get; set; }
        public List<ClusterAnalyzed> EducationLinesClustersAnalysed { get; set; }

        public List<UserEducationLineAnalysed> UsersToEducationLinesAnalysed { get; set; }
        public List<EducationLineToUserAnalysed> EducationLinesToUsersAnalysed { get; set; }
        #endregion

        #region LSA
        public async Task CalculateUserToClusterForLSA()
        {
            var task = Task.Factory.StartNew(() =>
            {
                //Получаем полный список доступных кластеров
                using (var context = new RecomendationSystemModelContainer())
                {
                    totalArrayClusters = context.Clusters.ToArray();
                }

                //Строим таблицу пользователь/кластер. Значение считается на основе перемножения весов предметов и оценок по предметам 
                //+ сложение влияющих предметов(если 1 предмет имеет веса как по своему, так и по остальным)
                allUserCluster = AnalyseAllUserCluster();

                //Проводим LSA анализ и получаем координаты для пользователей и кластеров
                CalculateUserAndClusterCoord();
            });
            await task;
        }
        public async Task CalculateEducationLinesToClusterForLSA()
        {
            var task = Task.Factory.StartNew(() =>
            {
                //Получаем полный список доступных кластеров
                using (var context = new RecomendationSystemModelContainer())
                {
                    totalArrayClusters = context.Clusters.ToArray();
                }
                //Строим таблицу пользователь/кластер
                allEducationLinesCluster=AnalyseAllEducationLineCluster();

                //Проводим LSA анализ и получаем координаты для направлений и кластеров
                CalculateEducationLineAndClusterCoord();
            });
            await task;
        }
        public async Task CalculateUserToEducationLinesForLSA()
        {
            var task = Task.Factory.StartNew(() =>
            {
                //Получаем полный список доступных кластеров
                using (var context = new RecomendationSystemModelContainer())
                {
                    totalArrayClusters = context.Clusters.ToArray();
                }
                if (allUserCluster==null)
                    allUserCluster=AnalyseAllUserCluster();
                if(allEducationLinesCluster==null)
                    allEducationLinesCluster=AnalyseAllEducationLineCluster();

                allUserEducationLine = CreateUserToEducationLineMatrix();

                CalculateEducationLineAndUsersCoord();
            });
            await task;
        }

        /// <summary>
        /// Ищет в бд информацию о пользователях, из которой строит "таблицу": пользователи/кластеры. Реузльтат в переменной allUserCluster
        /// </summary>
        private ItemToClusterCell[,] AnalyseAllUserCluster()
        {
            using (var context = new RecomendationSystemModelContainer())
            {
                var allUsers = (from user in context.Users
                               select user).ToArray();
                var allUserClusterMatrix = new ItemToClusterCell[allUsers.Length,totalArrayClusters.Length];

                //берем всех пользователей
                for (int i = 0; i < allUsers.Length; i++)
			    {
			        Dictionary<string, double> clusterResult = new Dictionary<string, double>();
                    foreach (var item in totalArrayClusters)
                    {
                        clusterResult.Add(item.Name, 0);
                    }
                    //заполняем оценками и кластерами
                    foreach (var stateExam in allUsers[i].UnitedStateExam)
                    {
                        //Получаем текущую дисциплину и ее весовые коэффициенты. Запонляем : Дисциплина/кластер
                        int mark = stateExam.Result;
                        foreach (var weight in stateExam.Discipline.Weight)
                        {
                            var t = weight.Cluster;
                            //Прибавляет заданному кластуру значение. Именно прибавляет.
                            //Т.е. если у нас есть к примеру есть значение по математике, 
                            //и мы смотрим информатику,которая в свою очередь имеет вклад и в Информатику, и в Математику(меньший). 
                            //Соответсвенно начальное значение математики увеличится
                            clusterResult[weight.Cluster.Name] += mark * weight.Coefficient;
                        }
                    }
                    for (int j = 0; j < totalArrayClusters.Length; j++)
                    {
                        //Результат
                        allUserClusterMatrix[i, j] = new ItemToClusterCell
                        {
                            Id = allUsers[i].Id,
                            Name = allUsers[i].FirstName,
                            ClusterName = clusterResult.ElementAtOrDefault(j).Key,
                            Value = clusterResult.ElementAtOrDefault(j).Value
                        }; 
                    }
			    }
                return allUserClusterMatrix;
            }

        }

        /// <summary>
        /// Ищет в бд информацию о направлениях, из которой строит "таблицу": направления/кластеры. Реузльтат в переменной allEducationLineCluster
        /// </summary>
        private ItemToClusterCell[,] AnalyseAllEducationLineCluster()
        {
            using (var context = new RecomendationSystemModelContainer())
            {
                var allEducationLines = (from user in context.DepartmentEducationLines
                                select user).ToArray();
                var allEducationLinesClusterMatrix = new ItemToClusterCell[allEducationLines.Length, totalArrayClusters.Length];

                //пробешамеся по всем направлениям
                for (int i = 0; i < allEducationLines.Length; i++)
                {
                    Dictionary<string, double> clusterResult = new Dictionary<string, double>();
                    foreach (var item in totalArrayClusters)
                    {
                        clusterResult.Add(item.Name, 0);
                    }
                    //заполняем оценками и кластерами
                    foreach (var requipment in allEducationLines[i].DepartmentLinesRequirement)
                    {
                        //Получаем текущую дисциплину и ее весовые коэффициенты. Запонляем : Дисциплина/кластер
                        int mark = requipment.Requirement;
                        foreach (var weight in requipment.Discipline.Weight)
                        {
                            var t = weight.Cluster;
                            //Прибавляет заданному кластуру значение
                            clusterResult[weight.Cluster.Name] += mark * weight.Coefficient;
                        }
                    }

                    for (int j = 0; j < totalArrayClusters.Length; j++)
                    {
                        //Результат
                        allEducationLinesClusterMatrix[i, j] = new ItemToClusterCell
                        {
                            Id = allEducationLines[i].Id,
                            Name = allEducationLines[i].Name,
                            ClusterName = clusterResult.ElementAtOrDefault(j).Key,
                            Value = clusterResult.ElementAtOrDefault(j).Value
                        };
                    }
                }

                return allEducationLinesClusterMatrix;
            }
        }

        /// <summary>
        /// Из таблиц пользователь/кластер и "направление обучения/кластер" строим таблицу
        /// "пользователь/направление обучения"
        /// </summary>
        private UserToEducationLineCell[,] CreateUserToEducationLineMatrix()
        {
            UserToEducationLineCell[,] userToEducLineMatrix = new UserToEducationLineCell[allUserCluster.GetLength(0), allEducationLinesCluster.GetLength(0)];
            //Пробегамеся по всем пользователям и находи расстояние до соотвествующих направлений
            for (int i = 0; i < allUserCluster.GetLength(0); i++)
            {
                for (int j = 0; j < allEducationLinesCluster.GetLength(0); j++)
                {
                    double distance = 0;
                    for (int clusterNum = 0; clusterNum < totalArrayClusters.Length; clusterNum++)
                    {
                        distance += Math.Pow(allUserCluster[i, clusterNum].Value - allEducationLinesCluster[j, clusterNum].Value, 2);
                    }

                    distance = Math.Sqrt(distance);

                    userToEducLineMatrix[i, j] = new UserToEducationLineCell
                    {
                        UserId = allUserCluster[i, 0].Id,
                        UserName = allUserCluster[i, 0].Name,

                        EducationLineId = allEducationLinesCluster[j, 0].Id,
                        EducationLineName = allEducationLinesCluster[j, 0].Name,

                        Value = distance
                    };
                }
            }
            return userToEducLineMatrix;
        }

        /// <summary>
        /// Строит числовую матрицу для LSA анализа из заданной нечисловой матрицы для пользователей и направлений
        /// </summary>
        /// <param name="itemToCluster">Нечисловая матрица, из которого строится числовая матрица</param>
        /// <returns>Построенная числовая матрица</returns>
        private double[,] CalculateMatrix(ItemToClusterCell[,] itemToCluster)
        {
            //Представляем нашу таблицу в более простом виде
            int row = itemToCluster.GetLength(0);
            int column = itemToCluster.GetLength(1);
            var matrixCluster = new double[row, column];

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    matrixCluster[i, j] = itemToCluster[i,j].Value;
                }
            }
            return matrixCluster;
        }
        /// <summary>
        /// Строит числовую матрицу для LSA анализа из заданной нечисловой матрицы для матрицы пользоватлей-направлений
        /// </summary>
        /// <param name="userToEdLine">Нечисловая матрица, из которого строится числовая матрица</param>
        /// <returns>Построенная числовая матрица</returns>
        private double[,] CalculateMatrix(UserToEducationLineCell[,] userToEdLine)
        {
            //Представляем нашу таблицу в более простом виде
            int row = userToEdLine.GetLength(0);
            int column = userToEdLine.GetLength(1);
            var matrixCluster = new double[row, column];

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    matrixCluster[i, j] = userToEdLine[i, j].Value;
                }
            }
            return matrixCluster;
        }

        private void CalculateUserAndClusterCoord()
        {
            //Получаем чистое представление нашей матрицы(только числа, без доп. информации)
            var matrix = CalculateMatrix(allUserCluster);
            int row = matrix.GetLength(0);
            int column = matrix.GetLength(1);

            LSA lsa = new LSA(matrix);
            
            //Проанализируем пользователей
            UsersAnalysed = new List<UserClusterAnalyzed>();
            for (int i = 0; i < row; i++)
            {
                UsersAnalysed.Add(
                    new UserClusterAnalyzed(allUserCluster[i,0],lsa.UCoords[i,0],lsa.UCoords[i,1]));
            }

            //Проанализируем кластеры обучения
            UsersClustersAnalysed = new List<ClusterAnalyzed>();
            for (int i = 0; i < column; i++)
            {
                UsersClustersAnalysed.Add(
                    new ClusterAnalyzed(allUserCluster[0, i], lsa.VCoords[i, 0], lsa.VCoords[i, 1]));
            }

        }
        private void CalculateEducationLineAndClusterCoord()
        {
            //Получаем чистое представление нашей матрицы(только числа, без доп. информации)
            var matrix = CalculateMatrix(allEducationLinesCluster);
            int row = matrix.GetLength(0);
            int column = matrix.GetLength(1);

            LSA lsa = new LSA(matrix);

            //Проанализируем пользователей
            EducationLinesAnalysed = new List<EducationLineToClusterAnalyzed>();
            for (int i = 0; i < row; i++)
            {
                EducationLinesAnalysed.Add(
                    new EducationLineToClusterAnalyzed(allEducationLinesCluster[i, 0], lsa.UCoords[i, 0], lsa.UCoords[i, 1]));
            }

            //Проанализируем кластеры обучения
            EducationLinesClustersAnalysed = new List<ClusterAnalyzed>();
            for (int i = 0; i < column; i++)
            {
                EducationLinesClustersAnalysed.Add(
                    new ClusterAnalyzed(allEducationLinesCluster[0, i], lsa.VCoords[i, 0], lsa.VCoords[i, 1]));
            }

        }

        private void CalculateEducationLineAndUsersCoord()
        {
            //Получаем чистое представление нашей матрицы(только числа, без доп. информации)
            var matrix = CalculateMatrix(allUserEducationLine);
            int row = matrix.GetLength(0);
            int column = matrix.GetLength(1);

            LSA lsa = new LSA(matrix);

            //Проанализируем пользователей
            UsersToEducationLinesAnalysed = new List<UserEducationLineAnalysed>();
            for (int i = 0; i < row; i++)
            {
                UsersToEducationLinesAnalysed.Add(
                    new UserEducationLineAnalysed(allUserEducationLine[i, 0], lsa.UCoords[i, 0], lsa.UCoords[i, 1]));
            }

            //Проанализируем направления обучения
            EducationLinesToUsersAnalysed = new List<EducationLineToUserAnalysed>();
            for (int i = 0; i < column; i++)
            {
                EducationLinesToUsersAnalysed.Add(
                    new EducationLineToUserAnalysed(allUserEducationLine[0, i], lsa.VCoords[i, 0], lsa.VCoords[i, 1]));
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
    public class UserToEducationLineCell
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        public int EducationLineId { get; set; }
        public string EducationLineName { get; set; }

        public double Value { get; set; }
    }
    public class ItemToClusterCell
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string ClusterName { get; set; }

        public double Value { get; set; }
    }
}
