//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class DepartmentLinesRequirement
    {
        public int Id { get; set; }
        public int Requirement { get; set; }
        public int DepartmentEducationLineId { get; set; }
        public int DisciplineId { get; set; }
    
        public virtual DepartmentEducationLine DepartmentEducationLine { get; set; }
        public virtual Discipline Discipline { get; set; }
    }
}