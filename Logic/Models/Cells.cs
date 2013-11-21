using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Models
{
    public class UserToEducationLineCell
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        public int EducationLineId { get; set; }
        public string EducationLineName { get; set; }

        public double Value { get; set; }
    }
    public class UserToClusterCell
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        public int ClusterId { get; set; }
        public string ClusterName { get; set; }

        public double Value { get; set; }
    }

    public class EducationLineToClusterCell
    {
        public int EducationLineId { get; set; }
        public string EducationLineName { get; set; }

        public int ClusterId { get; set; }
        public string ClusterName { get; set; }

        public double Value { get; set; }
    }
}
