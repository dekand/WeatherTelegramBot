using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using WeatherTelegramBot.Models;

namespace WeatherTelegramBot
{
    internal class ImageCreator
    {
        private string str;
        private Font font;
        private Brush brush;
        private float x;
        private float y;

        private ImageCreator(string str, Font font, float x, float y)
        {
            this.str = str;
            this.font = font;
            this.brush = new SolidBrush(Color.White);
            this.x = x;
            this.y = y;
        }

        private void PrintString(Graphics g)
        {
            g.DrawString(str, font, brush, x, y);
        }

        private static readonly string iconImagePath = DirPath.imgDir + "icons\\";

        public static Stream ImageCreation(Stream stream, string cityName, YaWeather weather)
        {
            string temp = weather.DisplayTemp();
            //шрифты
            var fontH1 = GetFontWithSize(64);
            var fontH2 = GetFontWithSize(36);
            var fontH3 = GetFontWithSize(24);

            float strWidth1 = GetWidthFromString(cityName, fontH2);
            float strWidth2 = GetWidthFromString(temp, fontH1);
            float x = 20 + strWidth1;

            Bitmap bmpImage = new Bitmap(500, 320);
            var objGraphics = CreateGraphics(bmpImage, Color.FromArgb(60, 128, 228));

            var strCollection = new List<ImageCreator>() {
                new ImageCreator(cityName, fontH2, 20, 40),
                new ImageCreator(temp, fontH1, ((x) / 2) + 10 - strWidth2 / 2, 85)
            };

            DrawIcon(objGraphics, weather.Icon);

            strWidth2 = GetWidthFromString(weather.Condition, fontH3);
            x = strWidth2 < strWidth1 ? ((x) / 2) - strWidth2 / 2 : 20; //если ширина weather.Condition больше названия города, то выравнивание по левому краю
            strCollection.Add(new ImageCreator(weather.Condition, fontH3, x, 165));
            strCollection.Add(new ImageCreator(weather.DisplayWeatherInfo(), fontH3, 20, 215));

            DrawAndFlush(objGraphics, strCollection);
            return SaveImageInStream(bmpImage, stream);
        }

        private static void DrawAndFlush(Graphics g, List<ImageCreator> strCollection)
        {
            foreach (ImageCreator str in strCollection)
            {
                str.PrintString(g);
            }
            g.Flush();
        }

        private static Font GetFontWithSize(float size) => new Font("Arial", size, FontStyle.Bold, GraphicsUnit.Pixel);

        private static Graphics CreateGraphics(Bitmap image, Color color)
        {
            Graphics g = Graphics.FromImage(image);
            g.Clear(color);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            return g;
        }

        private static float GetWidthFromString(string str, Font font)
        {
            Graphics graphics = Graphics.FromImage(new Bitmap(1, 1));
            var v = graphics.MeasureString(str, font);
            return (float)v.Width;
        }

        private static void DrawIcon(Graphics g, string iconName)
        {
            try { g.DrawImage(Image.FromFile(iconImagePath + iconName + ".png"), new PointF(308, 50)); }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        private static Stream SaveImageInStream(Bitmap image, Stream stream)
        {
            image.Save(stream, ImageFormat.Png);
            stream.Position = 0;
            return stream;
        }
    }
}