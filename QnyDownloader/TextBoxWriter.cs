using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QnyDownloader
{
    public class TextBoxWriter : System.IO.TextWriter
    {
        TextBox txtBox;
        delegate void VoidAction();

        public TextBoxWriter(TextBox box)
        {
            txtBox = box; //transfer the enternal TextBox in
        }

        public override void Write(char value)
        {
            //base.Write(value);//still output to Console
            VoidAction action = delegate
            {
                txtBox.AppendText(value.ToString());
            };
            txtBox.BeginInvoke(action);
        }

        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
