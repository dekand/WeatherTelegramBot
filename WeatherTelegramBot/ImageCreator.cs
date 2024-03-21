using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using WeatherTelegramBot.Models;

namespace WeatherTelegramBot
{
    internal class ImageCreator
    {
        public static readonly string iconImagePath = DirPath.imgDir + "icons\\";

        public static Stream ImageCreation(Stream stream,string cityName, YaWeather weather)
        {
            string temp = weather.DisplayTemp();
            //шрифты
            var fontH1 = GetFontWithSize(64);
            var fontH2 = GetFontWithSize(36);
            var fontH3 = GetFontWithSize(24);
            var brush = new SolidBrush(Color.White); //цвет шрифта
            var color = Color.FromArgb(60, 128, 228); //цвет фона

            (float strWidth1, float strHeight1) = GetSizeFromString(cityName, fontH2);
            (float strWidth2, float strHeight2) = GetSizeFromString(temp, fontH1);
            float[] xy = [20 + strWidth1, strHeight1 + 40];
            //создание поля
            Bitmap bmpImage = new Bitmap(1, 1);
            bmpImage = new Bitmap(bmpImage, new Size(500, 320));
            Graphics objGraphics = Graphics.FromImage(bmpImage);

            objGraphics.Clear(color);
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            //отрисовка иконки и текста
            try
            { objGraphics.DrawImage(Image.FromFile(iconImagePath + weather.Icon + ".png"), new PointF(308, xy[1] - 35)); }
            catch(Exception e) { Console.WriteLine(e.Message); }
            objGraphics.DrawString(cityName, fontH2, brush, 20, 40);
            objGraphics.DrawString(temp, fontH1, brush, ((xy[0]) / 2) + 10 - strWidth2 / 2, xy[1]);

            xy[1] += strHeight2;
            (strWidth2, strHeight2) = GetSizeFromString(weather.Condition, fontH3);

            float x = strWidth2 < strWidth1 ? ((xy[0]) / 2) - strWidth2 / 2 : 20; //если ширина weather.Condition больше названия города, то выравнивание по левому краю
            objGraphics.DrawString(weather.Condition, fontH3, brush, x, xy[1]);
            objGraphics.DrawString(weather.DisplayWeatherInfo(), fontH3, brush, 20, xy[1] + strHeight2 + 20);

            objGraphics.Flush();

            bmpImage.Save(stream, ImageFormat.Png);
            stream.Position = 0;
            return stream;
        }

        public static Font GetFontWithSize(float size) => new Font("Arial", size, FontStyle.Bold, GraphicsUnit.Pixel);

        public static (float, float) GetSizeFromString(string str, Font font)
        {
            Graphics graphics = Graphics.FromImage(new Bitmap(1, 1));
            var v = graphics.MeasureString(str, font);
            return ((float)v.Width, (float)v.Height);
        }
    }
}