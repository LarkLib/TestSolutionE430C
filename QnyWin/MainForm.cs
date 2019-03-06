using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QnyWin
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var data = new[] {"aaa","bbb","ccc" };
            DataTable dt = new DataTable();//创建表
            dt.Columns.Add("Id", typeof(Int32));//添加列
            dt.Columns.Add("Name", typeof(String));
            dt.Columns.Add("Age", typeof(Int32));
            dt.Rows.Add(new object[] { 1, "张三", 20 });//添加行
            dt.Rows.Add(new object[] { 2, "李四", 25 });
            dt.Rows.Add(new object[] { 3, "王五", 30 });
            RnDataGridView.DataSource = dt;
            RnDataGridView.Columns[0].ReadOnly = true;
        }
    }
}
