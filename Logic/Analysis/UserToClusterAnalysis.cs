using DataLayer;
using Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Analysis
{
    public sealed class UserToClusterAnalysis
    {
        #region Singleton Pattern
        private static volatile UserToClusterAnalysis instance;
        private static object syncRoot = new Object();

        private UserToClusterAnalysis() 
        {
            //Получаем полный список доступных кластеров
            using (var context = new RecomendationSystemModelContainer())
            {
                totalArrayClusters = context.Clusters.ToArray();
            }
        }
        public static UserToClusterAnalysis Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new UserToClusterAnalysis();
                    }
                }

                return instance;
            }
        }
        #endregion
        Cluster[] totalArrayClusters;
        //таблица "пользователи/кластеры"
        UserToClusterCell[,] _allUserCluster;
        public UserToClusterCell[,] AllUserCluster
        {
            get 
            {
                if (_allUserCluster == null)
                    _allUserCluster = AnalyseAllUserCluster();

                return _allUserCluster; 
            }
        }

        public List<ItemPosition> UsersToClusterPosition { get; set; }
        public List<ItemPosition> ClustersToUserPosition { get; set; }

        public async Task CalculateUserToClusterForLSA()
        {
            var task = Task.Factory.StartNew(() =>
            {
                //Проводим LSA анализ и получаем координаты для пользователей и кластеров
                CalculateUserAndClusterCoord();
            });
            await task;
        }
        /// <summary>
        /// Ищет в бд информацию о пользователях, из которой строит "таблицу": пользователи/кластеры.
        /// </summary>
        /// <returns>Таблица с значениеями и описанием ячейки </returns>
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
            var matrix = CalculateMatrix(AllUserCluster);
            int row = matrix.GetLength(0);
            int column = matrix.GetLength(1);

            LSA lsa = new LSA(matrix);

            //Проанализируем пользователей
            UsersToClusterPosition = new List<ItemPosition>();
            for (int i = 0; i < row; i++)
            {
                int id = AllUserCluster[i, 0].UserId;
                string name = AllUserCluster[i, 0].UserName;
                UsersToClusterPosition.Add(
                    new ItemPosition(id, name, lsa.UCoords[i, 0], lsa.UCoords[i, 1]));
            }

            //Проанализируем кластеры обучения
            ClustersToUserPosition = new List<ItemPosition>();
            for (int i = 0; i < column; i++)
            {
                int id = AllUserCluster[0, 1].ClusterId;
                var name = AllUserCluster[0, i].ClusterName;

                ClustersToUserPosition.Add(
                    new ItemPosition(id, name, lsa.VCoords[i, 0], lsa.VCoords[i, 1]));
            }

        }
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
                    matrixCluster[i, j] = itemToCluster[i, j].Value;
                }
            }
            return matrixCluster;
        }
    }
}
