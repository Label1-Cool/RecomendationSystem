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
    /// Singleton Класс для извлечения информации из бд и представления ее в удобном для дальнейшей обработки методами виде. 
    /// </summary>
    public sealed class UserToEducationLineAnalysis
    {
        #region Singleton Pattern
        private static volatile UserToEducationLineAnalysis instance;
        private static object syncRoot = new Object();

        private UserToEducationLineAnalysis() 
        {
            //Получаем полный список доступных кластеров
            using (var context = new RecomendationSystemModelContainer())
            {
                totalArrayClusters = context.Clusters.ToArray();
            }
        }
        public static UserToEducationLineAnalysis Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new UserToEducationLineAnalysis();
                    }
                }

                return instance;
            }
        }
        #endregion
        #region Fields and Properties
        Cluster[] totalArrayClusters;

        /// <summary>
        /// Таблица пользователь-кластер с подсчитанными значениями
        /// </summary>
        UserToClusterCell[,] AllUserClusterTable
        {
            get
            {
                return UserToClusterAnalysis.Instance.AllUserCluster;
            }
        }

        /// <summary>
        /// Таблица учебное направлени-кластер с подсчитанными значениями
        /// </summary>
        EducationLineToClusterCell[,] AllEducationLinesClusterTable
        { 
            get
            { return EducationLineToClusterAnalysis.Instance.AllEducationLinesCluster; }
        }

        //таблица "направления обучения/кластеры" в виде словаря
        UserToEducationLineCell[,] allUserEducationLine;

        public List<ItemPosition> UsersToEducationLinesPosition { get; set; }
        public List<ItemPosition> EducationLinesToUsersPosition { get; set; }

        public List<UserAndDistancesRow> UsersToEducationsDistances { get; set; }
        #endregion

        #region LSA
        #region UserToEducationLines
        public async Task CalculateUserToEducationLinesForLSA()
        {
            var task = Task.Factory.StartNew(() =>
            {
                allUserEducationLine = CreateUserToEducationLineMatrix();

                CalculateEducationLineAndUsersCoord();
            });
            await task;
        }

        /// <summary>
        /// Из таблиц пользователь/кластер и "направление обучения/кластер" строим таблицу
        /// "пользователь/направление обучения" (массив и список(он для вывода))
        /// </summary>
        private UserToEducationLineCell[,] CreateUserToEducationLineMatrix()
        {
            UserToEducationLineCell[,] userToEducLineMatrix = new UserToEducationLineCell[AllUserClusterTable.GetLength(0), AllEducationLinesClusterTable.GetLength(0)];

            UsersToEducationsDistances = new List<UserAndDistancesRow>();
            //Пробегамеся по всем пользователям и находи расстояние до соотвествующих направлений
            for (int i = 0; i < AllUserClusterTable.GetLength(0); i++)
            {
                var usersToEducationsDistancesRow = new UserAndDistancesRow()
                {
                    Id = AllUserClusterTable[i, 0].UserId,
                    Name = AllUserClusterTable[i, 0].UserName,

                    Cells=new List<UserToEducationLineCell>()
                };

                for (int j = 0; j < AllEducationLinesClusterTable.GetLength(0); j++)
                {
                    double distance = 0;
                    for (int clusterNum = 0; clusterNum < totalArrayClusters.Length; clusterNum++)
                    {
                        distance += Math.Pow(AllUserClusterTable[i, clusterNum].Value - AllEducationLinesClusterTable[j, clusterNum].Value, 2);
                    }

                    distance = Math.Sqrt(distance);

                    userToEducLineMatrix[i, j] = new UserToEducationLineCell
                    {
                        UserId = AllUserClusterTable[i, 0].UserId,
                        UserName = AllUserClusterTable[i, 0].UserName,

                        EducationLineId = AllEducationLinesClusterTable[j, 0].EducationLineId,
                        EducationLineName = AllEducationLinesClusterTable[j, 0].EducationLineName,

                        Value = distance
                    };
                    //Добавил полученный результат так же в список
                    usersToEducationsDistancesRow.Cells.Add(userToEducLineMatrix[i, j]);
                }
                UsersToEducationsDistances.Add(usersToEducationsDistancesRow);
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
    public class UserAndDistancesRow
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<UserToEducationLineCell> Cells { get; set; }
    }

}
