using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Models
{
    public class ClusterAnalyzed
    {
        public string Name { get; private set; }
        public KeyValuePair<double, double> Coords { get; private set; }

        public ClusterAnalyzed(string name, KeyValuePair<double, double> coords)
        {
            Name = name;
            Coords = coords;
        }
    }
}
