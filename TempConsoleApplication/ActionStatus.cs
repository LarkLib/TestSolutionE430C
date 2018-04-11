using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempConsoleApplication
{
    internal enum ActionStatus
    {
        Succeeded = 1,
        Faulted = -1,
        Initialized = 0,
        SucceededWithNoData = 2
    }
}
