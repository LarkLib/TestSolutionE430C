using StockWebApplication45.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace StockWebApplication45.Utility
{
    public class Operation
    {
        private static string ConnectionString = ConfigurationManager.ConnectionStrings["StockEntities"].ConnectionString;

        internal IList<StockEntity> GetStockEntityList()
        {
            var list = new List<StockEntity>();


            using (var connection = new SqlConnection(ConnectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = $"select * from StockDeploy order by date desc,class,code";
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new StockEntity()
                    {
                        Date = reader.GetDateTime(0).ToString("yyyy/MM/dd"),
                        Code = reader.GetString(1),
                        BuyPrice = reader.GetDecimal(2),
                        LastPrice = reader.GetDecimal(3),
                        ReturnRate = reader.IsDBNull(4) ? (Decimal?)null : reader.GetDecimal(4),
                        SaleDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5).ToString("yyyy/MM/dd"),
                        Category = reader.GetInt32(6),
                        Status = reader.GetString(7),
                        Class = reader.GetInt32(8)
                    });
                }
                connection.Close();
            }
            return list;
        }
    }
}