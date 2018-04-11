using LarkLab.DemoClass.Interface;
using LarkLab.DemoClass.Libray.DemoFig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LarkLab.DemoClass.Win
{
    public partial class DemoClassForm : Form
    {
        public DemoClassForm()
        {
            InitializeComponent();
        }

        private void DemoClassForm_Load(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.Load("LarkLab.DemoClass.Libray");
            foreach (var item in assembly.DefinedTypes)
            {
                var type = item.GetInterface("IDiscoverable`1");
                var obj = Activator.CreateInstance(item);
            }
            IDiscoverable<string> serializableFig = new SerializableFig();
            var result = serializableFig.Execut();
        }
    }
}
