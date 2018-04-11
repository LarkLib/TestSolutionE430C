using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LarkLab.DemoClass.Interface
{
    public interface IDiscoverable<T>
    {
        string Name { get; }
        string Description { get; }
        string Code { get; }
        T Execut();
    }
}
