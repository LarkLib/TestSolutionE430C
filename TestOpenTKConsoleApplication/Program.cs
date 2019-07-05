using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TestOpenTKConsoleApplication
{
    class Program:GameWindow
    {
        static void Main(string[] args)
        {
            var game = new Program();
            game.Run(60.0);
        }
    }
}
