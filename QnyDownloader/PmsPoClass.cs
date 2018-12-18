using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QnyDownloader
{
    public class SkuItem
    {
        public long skuId { get; set; }
        public string categoryName { get; set; }
        public string spuName { get; set; }
        public string skuMallCode { get; set; }
        public string skuCode { get; set; }
        public int storageTemperatureLevel { get; set; }
        public int guaranteePeriod { get; set; }
        public string skuSpec { get; set; }
        public string skuCostPrice { get; set; }
        public string skuDictUnitName { get; set; }
        public string skuBoxQuantity { get; set; }
        public string prePoAmount { get; set; }
        public string poAmount { get; set; }
        public long productionDate { get; set; }
        public string sumPrePoPrice { get; set; }
        public string sumPoPrice { get; set; }
        public string unitId { get; set; }
        public int packageType { get; set; }
        public string categoryId { get; set; }
        public string tax { get; set; }
        public int guaranteePeriodType { get; set; }
        public string availableQuantity { get; set; }
        public string availablePoPrice { get; set; }
    }

    public class PmsPoDetailItem
    {
        public string supplierId { get; set; }
        public string supplierName { get; set; }
        public string poNo { get; set; }
        public long cTime { get; set; }
        public string poiId { get; set; }
        public long preArrivalTime { get; set; }
        public long arrivalTime { get; set; }
        public List<SkuItem> skuList { get; set; }
        public string creator { get; set; }
        public int poiType { get; set; }
        public string supplierCode { get; set; }
        public string supplierPrimaryContactPhone { get; set; }
        public string supplierPrimaryContactName { get; set; }
        public string poiName { get; set; }
        public string poiAddress { get; set; }
        public string poiServicePhone { get; set; }
        public int status { get; set; }
        public string poiContactName { get; set; }
        public int skuPriceType { get; set; }
        public string remark { get; set; }
        public int supplyType { get; set; }
        public string areaId { get; set; }
        public object areaName { get; set; }
    }

    public class PmsPoDetailRoot
    {
        public int code { get; set; }
        public string msg { get; set; }
        public PmsPoDetailItem data { get; set; }
    }

    public class PmsPoItem
    {
        public long id { get; set; }
        public string poNo { get; set; }
        public string supplierId { get; set; }
        public string poiId { get; set; }
        public long preArrivalTime { get; set; }
        public long arrivalTime { get; set; }
        public int status { get; set; }
        public string categoryName { get; set; }
        public int totalSku { get; set; }
        public string totalPrepoAmount { get; set; }
        public long expairTime { get; set; }
        public int skuPriceType { get; set; }
        public string creator { get; set; }
        public long ctime { get; set; }
        public long utime { get; set; }
        public string @operator { get; set; }
    }

    public class PmsPoSummary
    {
        public List<PmsPoItem> pmsPoList { get; set; }
        public int total { get; set; }
    }

    public class PmsPoRoot
    {
        public int code { get; set; }
        public string msg { get; set; }
        public PmsPoSummary data { get; set; }
    }

    public class SmsTemplateParam
    {
        //您有新采购单(${poNo})，下单人mis账号(${creator})，收货方(${poiName})，约定到货(${preArrivalTime})，请于小象系统中确认。
        //创建人:${creator},类别:${categoryName},订单号:${poNo},收货:${poiName}
        public string poNo { get; set; }
        //public string preArrivalTime { get; set; }
        public string creator { get; set; }
        public string poiName { get; set; }
        public string categoryName { get; set; }
    }
}
