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
    
    public partial class ParticipationInCompetition
    {
        public int Id { get; set; }
        public string Result { get; set; }
        public int UserId { get; set; }
        public int CompetitionId { get; set; }
    
        public virtual User User { get; set; }
        public virtual Competition Competition { get; set; }
    }
}
