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
    
    public partial class PmsPoDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PmsPoDetail()
        {
            this.SkuList = new HashSet<Sku>();
        }
    
        public Nullable<int> supplierId { get; set; }
        public string supplierName { get; set; }
        public string poNo { get; set; }
        public Nullable<long> cTime { get; set; }
        public Nullable<int> poiId { get; set; }
        public Nullable<long> preArrivalTime { get; set; }
        public Nullable<long> arrivalTime { get; set; }
        public string creator { get; set; }
        public Nullable<int> poiType { get; set; }
        public string supplierCode { get; set; }
        public string supplierPrimaryContactPhone { get; set; }
        public string supplierPrimaryContactName { get; set; }
        public string poiName { get; set; }
        public string poiAddress { get; set; }
        public string poiServicePhone { get; set; }
        public int status { get; set; }
        public string poiContactName { get; set; }
        public Nullable<int> skuPriceType { get; set; }
        public string remark { get; set; }
        public Nullable<int> supplyType { get; set; }
        public Nullable<int> areaId { get; set; }
        public string areaName { get; set; }
        public Nullable<System.DateTime> cTime2 { get; set; }
        public Nullable<System.DateTime> preArrivalTime2 { get; set; }
        public Nullable<System.DateTime> arrivalTime2 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sku> SkuList { get; set; }
        public virtual Status StatusItem { get; set; }
    }
}
