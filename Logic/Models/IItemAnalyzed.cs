using System;
using System.Collections.Generic;
namespace Logic.Models
{
    interface IItemAnalyzed
    {
        Dictionary<string, double> CalculateOptimalDirections(List<ClusterAnalyzed> allClusters);
        KeyValuePair<double, double> Coords { get; }
        KeyValuePair<int, string> Info { get; }
    }
}
