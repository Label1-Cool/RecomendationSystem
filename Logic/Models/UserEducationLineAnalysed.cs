using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Models
{
    public class UserEducationLineAnalysed
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

        public UserEducationLineAnalysed(UserToEducationLineCell userToEducationLineCell, double xCoord, double yCoord)
        {
            Id = userToEducationLineCell.UserId;
            Name = userToEducationLineCell.UserName;

            XCoord = xCoord;
            YCoord = yCoord;
        }

        public Dictionary<string, double> CalculateOptimalDirections(List<EducationLineToUserAnalysed> allEducationLines)
        {
            Dictionary<string, double> allClustersDirections = new Dictionary<string, double>();
            foreach (var educationLine in allEducationLines)
            {
                //cluster.Coords
                double x1 = this.XCoord;
                double x2 = educationLine.XCoord;
                double y1 = this.YCoord;
                double y2 = educationLine.YCoord;

                double d = Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2);

                double distance = Math.Sqrt(d);
                allClustersDirections.Add(educationLine.Name, distance);
            }
            //сортируем в порядке убывания
            allClustersDirections = allClustersDirections
                    .OrderByDescending(elem => elem.Value)
                    .ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
            return allClustersDirections;
        }
    }
}
