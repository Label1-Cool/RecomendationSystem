using DataLayer;
using Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Analysis
{
    public sealed class EducationLineToClusterAnalysis
    {
        #region Singleton Pattern
        private static volatile EducationLineToClusterAnalysis instance;
        private static object syncRoot = new Object();

        private EducationLineToClusterAnalysis()
        {
            //Получаем полный список доступных кластеров
            using (var context = new RecomendationSystemModelContainer())
            {
                totalArrayClusters = context.Clusters.ToArray();
            }
        }
        public static EducationLineToClusterAnalysis Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new EducationLineToClusterAnalysis();
                    }
                }

                return instance;
            }
        }
        #endregion
        Cluster[] totalArrayClusters;
        //таблица "направления обучения/кластеры"
        EducationLineToClusterCell[,] _allEducationLinesCluster;
        public EducationLineToClusterCell[,] AllEducationLinesCluster
        {
            get
            {
                if (_allEducationLinesCluster == null)
                    _allEducationLinesCluster = AnalyseAllEducationLineCluster();

                return _allEducationLinesCluster;
            }
        }

        public List<ItemPosition> EducationLinesToClusterPosition { get; set; }
        public List<ItemPosition> ClustersToEducationLinesPosition { get; set; }

        public async Task CalculateEducationLinesToClusterForLSA()
        {
            var task = Task.Factory.StartNew(() =>
            {
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
            var matrix = CalculateMatrix(AllEducationLinesCluster);
            int row = matrix.GetLength(0);
            int column = matrix.GetLength(1);

            LSA lsa = new LSA(matrix);

            //Проанализируем пользователей
            EducationLinesToClusterPosition = new List<ItemPosition>();
            for (int i = 0; i < row; i++)
            {
                int id = AllEducationLinesCluster[i, 0].EducationLineId;
                string name = AllEducationLinesCluster[i, 0].EducationLineName;
                EducationLinesToClusterPosition.Add(
                    new ItemPosition(id, name, lsa.UCoords[i, 0], lsa.UCoords[i, 1]));
            }

            //Проанализируем кластеры обучения
            ClustersToEducationLinesPosition = new List<ItemPosition>();
            for (int i = 0; i < column; i++)
            {
                int id = AllEducationLinesCluster[0, i].ClusterId;
                string name = AllEducationLinesCluster[0, i].ClusterName;
                ClustersToEducationLinesPosition.Add(
                    new ItemPosition(id, name, lsa.VCoords[i, 0], lsa.VCoords[i, 1]));
            }

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
    }
}
