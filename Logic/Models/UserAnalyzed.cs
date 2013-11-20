﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Models
{
    public class UserAnalyzed : IItemAnalyzed
    {

        public int Id
        {
            get;
            private set;
        }
        public string Name
        {
            get;
            private set;
        }

        public double XCoord
        {
            get;
            private set;
        }
        public double YCoord
        {
            get;
            private set;
        }

        public UserAnalyzed(ItemToClusterCell user, double xCoord, double yCoord)
        {
            Id = user.Id;
            Name = user.Name;

            XCoord = xCoord;
            YCoord = yCoord;
        }

        public Dictionary<string, double> CalculateOptimalDirections(List<ClusterAnalyzed> allClusters)
        {
            Dictionary<string, double> allClustersDirections = new Dictionary<string, double>();
            foreach (var cluster in allClusters)
            {
                //cluster.Coords
                double x1 = this.XCoord;
                double x2 = cluster.XCoord;
                double y1 = this.YCoord;
                double y2 = cluster.YCoord;

                double d = Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2);

                double distance = Math.Sqrt(d);
                allClustersDirections.Add(cluster.Name, distance);
            }
            //сортируем в порядке убывания
            allClustersDirections = allClustersDirections
                    .OrderByDescending(elem => elem.Value)
                    .ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
            return allClustersDirections;
        }
    }
}
