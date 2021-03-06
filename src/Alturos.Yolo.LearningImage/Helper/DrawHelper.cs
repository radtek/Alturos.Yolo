﻿using Alturos.Yolo.LearningImage.Model;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Alturos.Yolo.LearningImage.Helper
{
    public static class DrawHelper
    {
        public static readonly Size ImageSize = new Size(1024, 576);

        public static Image DrawBoxes(AnnotationImage image, List<ObjectClass> objectClasses)
        {
            var colorCodes = GetColorCodes();

            var items = image.BoundingBoxes;

            var originalBitmap = new Bitmap(image.FilePath);
            var bitmap = new Bitmap(originalBitmap, ImageSize);
            originalBitmap.Dispose();

            using (var canvas = Graphics.FromImage(bitmap))
            {
                foreach (var item in items)
                {
                    var width = item.Width * bitmap.Width;
                    var height = item.Height * bitmap.Height;
                    var x = (item.CenterX * bitmap.Width) - (width / 2);
                    var y = (item.CenterY * bitmap.Height) - (height / 2);

                    var color = ColorTranslator.FromHtml(colorCodes[item.ObjectIndex]);
                    using (var pen = new Pen(color, 3))
                    {
                        canvas.DrawRectangle(pen, x, y, width, height);

                        var objectClass = objectClasses?.FirstOrDefault(o => o.Id == item.ObjectIndex);
                        if (objectClass != null)
                        {
                            using (var brush = new SolidBrush(color))
                            using (var bgBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 255)))
                            using (var font = new Font("Arial", 20))
                            {
                                var text = $"{objectClass.Id} {objectClass.Name}";
                                var point = new PointF(x + 4, y + 4);
                                var size = canvas.MeasureString(text, font);

                                canvas.FillRectangle(bgBrush, point.X, point.Y, size.Width, size.Height);
                                canvas.DrawString(text, font, brush, point);
                            }
                        }
                    }
                }

                canvas.Flush();
            }

            return bitmap;
        }

        private static string[] GetColorCodes()
        {
            return new string[] { "#E3330E", "#48E10F", "#D40FE1", "#24ECE3", "#EC2470" };
        }
    }
}
