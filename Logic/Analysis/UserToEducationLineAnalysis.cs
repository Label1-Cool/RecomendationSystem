using DataLayer;
using Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Logic.Analysis
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
    }
}
