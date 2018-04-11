using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestImageConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var imagePath = AppDomain.CurrentDomain.BaseDirectory;
            var imageOnePath = Path.Combine(imagePath, "one.jpg");
            var imageTwoPath = Path.Combine(imagePath, "two.jpg");
            var imageTwoAddPath = Path.Combine(imagePath, "twoAdd.jpg");
            Bitmap imageOne = new Bitmap(imageOnePath);
            Bitmap imageTwoAdd = new Bitmap(imageTwoAddPath);
            Bitmap imageTwo = new Bitmap(imageTwoPath);
            Bitmap targetMultiplyImage = imageOne.Clone() as Bitmap;
            Bitmap targetSubtractImage = imageTwoAdd.Clone() as Bitmap;
            Bitmap targetAddImage = imageTwoAdd.Clone() as Bitmap;
            var value = 0;
            var factorMultiply = 255;
            var factorSubtract = 255;

            for (int i = 0; i < imageTwo.Width; i++)
            {
                for (int j = 0; j < imageTwo.Height; j++)
                {
                    var rgbOne = imageOne.GetPixel(i, j);
                    var rgbTwoAdd = imageTwoAdd.GetPixel(i, j);
                    var rgbTwo = imageTwo.GetPixel(i, j);
                    value = Math.Max(value, rgbTwoAdd.R - rgbTwo.R);
                    value = Math.Max(value, rgbTwoAdd.G - rgbTwo.G);
                    value = Math.Max(value, rgbTwoAdd.B - rgbTwo.B);
                    var r = (rgbOne.R * rgbTwo.R) / factorMultiply;
                    var g = (rgbOne.R * rgbTwo.R) / factorMultiply;
                    var b = (rgbOne.R * rgbTwo.R) / factorMultiply;
                    var color = Color.FromArgb(r, g, b);
                    targetMultiplyImage.SetPixel(i, j, color);

                    r = (rgbTwoAdd.R - rgbTwo.R + factorSubtract) / 2;
                    g = (rgbTwoAdd.G - rgbTwo.G + factorSubtract) / 2;
                    b = (rgbTwoAdd.B - rgbTwo.B + factorSubtract) / 2;
                    color = Color.FromArgb(r, g, b);
                    targetSubtractImage.SetPixel(i, j, color);

                    r = (rgbTwoAdd.R + rgbTwo.R) / 2;
                    g = (rgbTwoAdd.G + rgbTwo.G) / 2;
                    b = (rgbTwoAdd.B + rgbTwo.B) / 2;
                    color = Color.FromArgb(r, g, b);
                    targetAddImage.SetPixel(i, j, color);
                }
            }
            targetMultiplyImage.Save(Path.Combine(imagePath, "targetMultiply.jpg"), ImageFormat.Jpeg);
            targetSubtractImage.Save(Path.Combine(imagePath, "targetSubtract.jpg"), ImageFormat.Jpeg);
            targetAddImage.Save(Path.Combine(imagePath, "targetAdd.jpg"), ImageFormat.Jpeg);
            Console.WriteLine($"{value},{value / 255m}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
