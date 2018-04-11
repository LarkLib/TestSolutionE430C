using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockDataImport
{
    class Program
    {
        static void Main(string[] args)
        {
            var operation = new ImportStockOperation();
            operation.ImportData();
        }
    }
}
