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

        private DataExtractor() 
        {
            //Получаем полный список доступных кластеров
            using (var context = new RecomendationSystemModelContainer())
            {
                totalArrayClusters = context.Clusters.ToArray();
            }
        }
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
        UserToClusterCell[,] allUserCluster;
        //таблица "направления обучения/кластеры"
        EducationLineToClusterCell[,] allEducationLinesCluster;
        //таблица "направления обучения/кластеры" в виде словаря
        UserToEducationLineCell[,] allUserEducationLine;

        public List<ItemPosition> UsersToClusterPosition { get; set; }
        public List<ItemPosition> ClustersToUserPosition { get; set; }

        public List<ItemPosition> EducationLinesToClusterPosition { get; set; }
        public List<ItemPosition> ClustersToEducationLinesPosition { get; set; }

        public List<ItemPosition> UsersToEducationLinesPosition { get; set; }
        public List<ItemPosition> EducationLinesToUsersPosition { get; set; }
        #endregion

        #region LSA
        #region UserToCluster
        public async Task CalculateUserToClusterForLSA()
        {
            var task = Task.Factory.StartNew(() =>
            {
                //Строим таблицу пользователь/кластер. Значение считается на основе перемножения весов предметов и оценок по предметам 
                //+ сложение влияющих предметов(если 1 предмет имеет веса как по своему, так и по остальным)
                if (allUserCluster == null)
                    allUserCluster = AnalyseAllUserCluster();

                //Проводим LSA анализ и получаем координаты для пользователей и кластеров
                CalculateUserAndClusterCoord();
            });
            await task;
        }
        /// <summary>
        /// Ищет в бд информацию о пользователях, из которой строит "таблицу": пользователи/кластеры. Реузльтат в переменной allUserCluster
        /// </summary>
        private UserToClusterCell[,] AnalyseAllUserCluster()
        {
            using (var context = new RecomendationSystemModelContainer())
            {
                var allUsers = (from user in context.Users
                                select user).ToArray();
                var allUserClusterMatrix = new UserToClusterCell[allUsers.Length, totalArrayClusters.Length];

                //берем всех пользователей
                for (int i = 0; i < allUsers.Length; i++)
                {
                    Dictionary<Cluster, double> clusterResult = new Dictionary<Cluster, double>();
                    foreach (var item in totalArrayClusters)
                    {
                        clusterResult.Add(item, 0);
                    }
                    //заполняем оценками и кластерами
                    foreach (var stateExam in allUsers[i].UnitedStateExam)
                    {
                        //Получаем текущую дисциплину и ее весовые коэффициенты. Запонляем : Дисциплина/кластер
                        int mark = stateExam.Result;
                        foreach (var weight in stateExam.Discipline.Weight)
                        {
                            var t = weight.Cluster;
                            var cluster = (from  item in totalArrayClusters
                                           where item.Name == weight.Cluster.Name
                                          select item).SingleOrDefault();
                            //Прибавляет заданному кластуру значение. Именно прибавляет.
                            //Т.е. если у нас есть к примеру есть значение по математике, 
                            //и мы смотрим информатику,которая в свою очередь имеет вклад и в Информатику, и в Математику(меньший). 
                            //Соответсвенно начальное значение математики увеличится
                            clusterResult[cluster] += mark * weight.Coefficient;
                        }
                    }
                    for (int j = 0; j < totalArrayClusters.Length; j++)
                    {
                        //Результат
                        allUserClusterMatrix[i, j] = new UserToClusterCell
                        {
                            UserId = allUsers[i].Id,
                            UserName = allUsers[i].FirstName,

                            ClusterId = clusterResult.ElementAtOrDefault(j).Key.Id,
                            ClusterName = clusterResult.ElementAtOrDefault(j).Key.Name,
                            Value = clusterResult.ElementAtOrDefault(j).Value
                        };
                    }
                }
                return allUserClusterMatrix;
            }

        }

        private void CalculateUserAndClusterCoord()
        {
            //Получаем чистое представление нашей матрицы(только числа, без доп. информации)
            var matrix = CalculateMatrix(allUserCluster);
            int row = matrix.GetLength(0);
            int column = matrix.GetLength(1);

            LSA lsa = new LSA(matrix);

            //Проанализируем пользователей
            UsersToClusterPosition = new List<ItemPosition>();
            for (int i = 0; i < row; i++)
            {
                int id = allUserCluster[i, 0].UserId;
                string name = allUserCluster[i, 0].UserName;
                UsersToClusterPosition.Add(
                    new ItemPosition(id,name, lsa.UCoords[i, 0], lsa.UCoords[i, 1]));
            }

            //Проанализируем кластеры обучения
            ClustersToUserPosition = new List<ItemPosition>();
            for (int i = 0; i < column; i++)
            {
                int id = allUserCluster[0,1].ClusterId;
                var name = allUserCluster[0, i].ClusterName;
                
                ClustersToUserPosition.Add(
                    new ItemPosition(id, name, lsa.VCoords[i, 0], lsa.VCoords[i, 1]));
            }

        }
        #endregion
        #region EducationLinesToCluster

        public async Task CalculateEducationLinesToClusterForLSA()
        {
            var task = Task.Factory.StartNew(() =>
            {
                //Строим таблицу пользователь/кластер
                if (allEducationLinesCluster == null)
                    allEducationLinesCluster=AnalyseAllEducationLineCluster();

                //Проводим LSA анализ и получаем координаты для направлений и кластеров
                CalculateEducationLineAndClusterCoord();
            });
            await task;
        }
        /// <summary>
        /// Ищет в бд информацию о направлениях, из которой строит "таблицу": направления/кластеры. Реузльтат в переменной allEducationLineCluster
        /// </summary>
        private EducationLineToClusterCell[,] AnalyseAllEducationLineCluster()
        {
            using (var context = new RecomendationSystemModelContainer())
            {
                var allEducationLines = (from user in context.DepartmentEducationLines
                                         select user).ToArray();
                var allEducationLinesClusterMatrix = new EducationLineToClusterCell[allEducationLines.Length, totalArrayClusters.Length];

                //пробешамеся по всем направлениям
                for (int i = 0; i < allEducationLines.Length; i++)
                {
                    Dictionary<Cluster, double> clusterResult = new Dictionary<Cluster, double>();
                    foreach (var item in totalArrayClusters)
                    {
                        clusterResult.Add(item, 0);
                    }
                    //заполняем оценками и кластерами
                    foreach (var requipment in allEducationLines[i].DepartmentLinesRequirement)
                    {
                        //Получаем текущую дисциплину и ее весовые коэффициенты. Запонляем : Дисциплина/кластер
                        int mark = requipment.Requirement;
                        foreach (var weight in requipment.Discipline.Weight)
                        {
                            var t = weight.Cluster;
                            var cluster = (from item in totalArrayClusters
                                           where item.Name == weight.Cluster.Name
                                           select item).SingleOrDefault();
                            //Прибавляет заданному кластуру значение. Именно прибавляет.
                            //Т.е. если у нас есть к примеру есть значение по математике, 
                            //и мы смотрим информатику,которая в свою очередь имеет вклад и в Информатику, и в Математику(меньший). 
                            //Соответсвенно начальное значение математики увеличится
                            clusterResult[cluster] += mark * weight.Coefficient;
                        }
                    }

                    for (int j = 0; j < totalArrayClusters.Length; j++)
                    {
                        //Результат
                        allEducationLinesClusterMatrix[i, j] = new EducationLineToClusterCell
                        {
                            EducationLineId = allEducationLines[i].Id,
                            EducationLineName = allEducationLines[i].Name,

                            ClusterId = clusterResult.ElementAtOrDefault(j).Key.Id,
                            ClusterName = clusterResult.ElementAtOrDefault(j).Key.Name,
                            Value = clusterResult.ElementAtOrDefault(j).Value
                        };
                    }
                }

                return allEducationLinesClusterMatrix;
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
            EducationLinesToClusterPosition = new List<ItemPosition>();
            for (int i = 0; i < row; i++)
            {
                int id = allEducationLinesCluster[i, 0].EducationLineId;
                string name = allEducationLinesCluster[i, 0].EducationLineName;
                EducationLinesToClusterPosition.Add(
                    new ItemPosition(id,name, lsa.UCoords[i, 0], lsa.UCoords[i, 1]));
            }

            //Проанализируем кластеры обучения
            ClustersToEducationLinesPosition = new List<ItemPosition>();
            for (int i = 0; i < column; i++)
            {
                int id = allEducationLinesCluster[0, i].ClusterId;
                string name = allEducationLinesCluster[0, i].ClusterName;
                ClustersToEducationLinesPosition.Add(
                    new ItemPosition(id, name, lsa.VCoords[i, 0], lsa.VCoords[i, 1]));
            }

        }
        #endregion
        #region UserToEducationLines
        public async Task CalculateUserToEducationLinesForLSA()
        {
            var task = Task.Factory.StartNew(() =>
            {
                if (allUserCluster == null)
                    allUserCluster = AnalyseAllUserCluster();
                if (allEducationLinesCluster == null)
                    allEducationLinesCluster = AnalyseAllEducationLineCluster();

                allUserEducationLine = CreateUserToEducationLineMatrix();

                CalculateEducationLineAndUsersCoord();
            });
            await task;
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
                        UserId = allUserCluster[i, 0].UserId,
                        UserName = allUserCluster[i, 0].UserName,

                        EducationLineId = allEducationLinesCluster[j, 0].EducationLineId,
                        EducationLineName = allEducationLinesCluster[j, 0].EducationLineName,

                        Value = distance
                    };
                }
            }
            return userToEducLineMatrix;
        }
        private void CalculateEducationLineAndUsersCoord()
        {
            //Получаем чистое представление нашей матрицы(только числа, без доп. информации)
            var matrix = CalculateMatrix(allUserEducationLine);
            int row = matrix.GetLength(0);
            int column = matrix.GetLength(1);

            LSA lsa = new LSA(matrix);

            //Проанализируем пользователей
            UsersToEducationLinesPosition = new List<ItemPosition>();
            for (int i = 0; i < row; i++)
            {
                int id = allUserEducationLine[i, 0].UserId;
                string name = allUserEducationLine[i, 0].UserName;

                UsersToEducationLinesPosition.Add(
                    new ItemPosition(id,name, lsa.UCoords[i, 0], lsa.UCoords[i, 1]));
            }

            //Проанализируем направления обучения
            EducationLinesToUsersPosition = new List<ItemPosition>();
            for (int i = 0; i < column; i++)
            {
                int id = allUserEducationLine[0, i].EducationLineId;
                string name = allUserEducationLine[0, i].EducationLineName;

                EducationLinesToUsersPosition.Add(
                    new ItemPosition(id,name, lsa.VCoords[i, 0], lsa.VCoords[i, 1]));
            }
        }
        #endregion
        /// <summary>
        /// Строит числовую матрицу для LSA анализа из заданной нечисловой матрицы для пользователей
        /// </summary>
        /// <param name="itemToCluster">Нечисловая матрица, из которого строится числовая матрица</param>
        /// <returns>Построенная числовая матрица</returns>
        private double[,] CalculateMatrix(UserToClusterCell[,] itemToCluster)
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
        /// Строит числовую матрицу для LSA анализа из заданной нечисловой матрицы для направлений
        /// </summary>
        /// <param name="itemToCluster">Нечисловая матрица, из которого строится числовая матрица</param>
        /// <returns>Построенная числовая матрица</returns>
        private double[,] CalculateMatrix(EducationLineToClusterCell[,] itemToCluster)
        {
            //Представляем нашу таблицу в более простом виде
            int row = itemToCluster.GetLength(0);
            int column = itemToCluster.GetLength(1);
            var matrixCluster = new double[row, column];

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    matrixCluster[i, j] = itemToCluster[i, j].Value;
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
    public class UserToClusterCell
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        public int ClusterId { get; set; }
        public string ClusterName { get; set; }

        public double Value { get; set; }
    }

    public class EducationLineToClusterCell
    {
        public int EducationLineId { get; set; }
        public string EducationLineName { get; set; }

        public int ClusterId { get; set; }
        public string ClusterName { get; set; }

        public double Value { get; set; }
    }
}
