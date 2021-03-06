//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class Weight
    {
        public int Id { get; set; }
        public double Coefficient { get; set; }
        public int ClusterId { get; set; }
        public Nullable<int> SchoolTypeId { get; set; }
        public Nullable<int> SchoolDisciplineId { get; set; }
        public Nullable<int> SectionId { get; set; }
        public Nullable<int> HobbieId { get; set; }
        public Nullable<int> CompetitionId { get; set; }
        public Nullable<int> DisciplineId { get; set; }
    
        public virtual Cluster Cluster { get; set; }
        public virtual SchoolType SchoolType { get; set; }
        public virtual SchoolDiscipline SchoolDiscipline { get; set; }
        public virtual Section Section { get; set; }
        public virtual Hobbie Hobbie { get; set; }
        public virtual Competition Competition { get; set; }
        public virtual Discipline Discipline { get; set; }
    }
}
