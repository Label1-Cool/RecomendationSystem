using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Models
{
    public class EducationLineAnalyzed : IItemAnalyzed
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

        public EducationLineAnalyzed(ItemToClusterCell user, double xCoord, double yCoord)
        {
            Id = user.Id;
            Name = user.Name;

            XCoord = xCoord;
            YCoord = yCoord;
        }
    }
}
