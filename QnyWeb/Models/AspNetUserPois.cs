//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QnyWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AspNetUserPois
    {
        public string UserId { get; set; }
        public int poiId { get; set; }
    
        public virtual PoiList PoiList { get; set; }
    }
}
