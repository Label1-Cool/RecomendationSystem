using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Models
{
    public class ClusterAnalyzed : IItemAnalyzed
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

        public ClusterAnalyzed(ItemToClusterCell user, double xCoord, double yCoord)
        {
            Id = user.Id;
            Name = user.ClusterName;

            XCoord = xCoord;
            YCoord = yCoord;
        }
    }
}
