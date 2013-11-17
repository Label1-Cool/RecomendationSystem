using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Models
{
    public class UserAnalyzed : IItemAnalyzed
    {
        public KeyValuePair<int, string> Info { get; private set; }
        public KeyValuePair<double, double> Coords { get; private set; }

        public UserAnalyzed(KeyValuePair<int, string> info, KeyValuePair<double, double> coords)
        {
            Info = info;
            Coords = coords;
        }

        public Dictionary<string,double> CalculateOptimalDirections(List<ClusterAnalyzed> allClusters)
        {
            Dictionary<string, double> allClustersDirections = new Dictionary<string, double>();
            foreach (var cluster in allClusters)
            {
                //cluster.Coords
                double x1 = this.Coords.Key;
                double x2 = cluster.Coords.Key;
                double y1 = this.Coords.Value;
                double y2 = cluster.Coords.Value;

                double d = Math.Pow(x2 - x1,2) + Math.Pow(y2 - y1,2);

                double distance = Math.Sqrt(d);
                allClustersDirections.Add(cluster.Name, distance);
            }
            //сортируем в порядке убывания
            allClustersDirections = allClustersDirections
                    .OrderBy(elem => elem.Value)
                    .ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
            return allClustersDirections;
        }
    }
}
