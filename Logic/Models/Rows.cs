using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Models
{
    public class EducationLineAndRequirementRow
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public List<int> Requirements { get; set; }
    }
    public class UserAndDistancesRow
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<UserToEducationLineCell> Cells { get; set; }
    }
}
