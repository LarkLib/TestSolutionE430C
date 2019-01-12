using QnyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QnyWeb.Utilities
{
    public class Operation
    {
        internal void ParserFormCollection(FormCollection form)
        {
            var rnIdsString = form["item.RnId"];
            if (string.IsNullOrWhiteSpace(rnIdsString)) return;
            var cDate = DateTime.Parse(form["cDate"]);
            var RnId = rnIdsString.Split(new[] { ',' });
            var rnItemList = new List<ReceivingNote>(RnId.Length);
            var paymentStatus = form["item.PaymentStatus"].Split(new[] { ',' });
            var Supplier = form["item.Supplier"].Split(new[] { ',' });
            var ContrastGrossProfit = form["item.ContrastGrossProfit"].Split(new[] { ',' });
            var ContrastPrice = form["item.ContrastPrice"].Split(new[] { ',' });
            var TotalPrice = form["item.TotalPrice"].Split(new[] { ',' });
            var UnitPrice = form["item.UnitPrice"].Split(new[] { ',' });
            var Quantity = form["item.Quantity"].Split(new[] { ',' });
            var skuid = form["item.skuid"].Split(new[] { ',' });
            for (int i = 0; i < RnId.Length; i++)
            {
                var rnItem = new ReceivingNote();
                //rnItem.cDate = cDate;
                if (!string.IsNullOrWhiteSpace(RnId[i])) rnItem.RnId = new Guid(RnId[i]);
                if (!string.IsNullOrWhiteSpace(paymentStatus[i])) rnItem.PaymentStatus = bool.Parse(paymentStatus[i]);
                if (!string.IsNullOrWhiteSpace(Supplier[i])) rnItem.Supplier = Supplier[i];
                if (!string.IsNullOrWhiteSpace(ContrastGrossProfit[i])) rnItem.ContrastGrossProfit = decimal.Parse(ContrastGrossProfit[i]);
                if (!string.IsNullOrWhiteSpace(ContrastPrice[i])) rnItem.ContrastPrice = decimal.Parse(ContrastPrice[i]);
                if (!string.IsNullOrWhiteSpace(TotalPrice[i])) rnItem.TotalPrice = decimal.Parse(TotalPrice[i]);
                if (!string.IsNullOrWhiteSpace(UnitPrice[i])) rnItem.UnitPrice = decimal.Parse(UnitPrice[i]);
                if (!string.IsNullOrWhiteSpace(Quantity[i])) rnItem.Quantity = decimal.Parse(Quantity[i]);
                if (!string.IsNullOrWhiteSpace(skuid[i])) rnItem.skuId = int.Parse(skuid[i]);
                rnItemList.Add(rnItem);
            }

            using (var dbContext = new ShQnyEntities())
            {
                //dbContext.ReceivingNotes.AddRange(rnItemList.Where(rn => rn.RnId != null || rn.Quantity > 0 || rn.TotalPrice > 0 || rn.UnitPrice > 0));
                var updates = rnItemList.Where(rn => rn.RnId != null);
                foreach (var item in updates)
                {
                    var original = dbContext.ReceivingNotes.Where(rn => rn.RnId == item.RnId).First();
                    var originalPropertyInfos = original.GetType().GetProperties();
                    foreach (var propertyInfo in originalPropertyInfos)
                    {
                        var itemPropertyInfo = item.GetType().GetProperty(propertyInfo.Name);
                        propertyInfo.SetValue(original, itemPropertyInfo.GetValue(item), null);
                    }
                }
                dbContext.ReceivingNotes.AddRange(rnItemList.Where(rn => rn.RnId == null));
                var rows = dbContext.SaveChanges();
            }
        }
    }
}