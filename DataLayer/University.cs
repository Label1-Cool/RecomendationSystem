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
    
    public partial class University
    {
        public University()
        {
            this.UniversityDepartment = new HashSet<UniversityDepartment>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int Prestige { get; set; }
        public int CityId { get; set; }
    
        public virtual City City { get; set; }
        public virtual ICollection<UniversityDepartment> UniversityDepartment { get; set; }
    }
}
