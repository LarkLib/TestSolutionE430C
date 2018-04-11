using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LarkLab.DemoClass.Libray.DemoFig
{
    public class BaseFig
    {
        public string Name
        {
            get
            {
                return MethodBase.GetCurrentMethod().Name;
            }
        }
    }
}
