using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace we_ing
{
    partial class Form1
    {
        public static Bitmap ConvertToFormat(Image image)
        {
            Bitmap copy = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
            using (Graphics gr = Graphics.FromImage(copy))
            {
                gr.DrawImage(image, new Rectangle(0, 0, copy.Width, copy.Height));
            }
            return copy;
        }


        private Bitmap 색변환(Bitmap bitmap, float gamma = 20.0f, float contrast = 1.8f, float threshold = 99)
        {
            var regen = ApplyGamma(bitmap, gamma, contrast, threshold); // cyan RGB 0/255/255 & Yellow RGB 255/255/0
            MemoryStream ms = new MemoryStream();
            //to4bit(regen, ms);
            ms.Seek(0, SeekOrigin.Begin);

            //   File.WriteAllBytes("이미지로드테스트.bmp", ms.ToArray());
            return ConvertToFormat(regen);
        }

        private Bitmap ApplyGamma(Bitmap bmp0, float gamma, float contrast, float threshold)
        {

            Bitmap bmp1 = new Bitmap(bmp0.Width, bmp0.Height);
            using (Graphics g = Graphics.FromImage(bmp1))
            {
                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                        {
                    new float[] {contrast, 0, 0, 0, 0},
                    new float[] {0,contrast, 0, 0, 0},
                    new float[] {0, 0, contrast, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                        });


                ImageAttributes attributes = new ImageAttributes();
                if (threshold != 99)
                    attributes.SetThreshold(threshold, ColorAdjustType.Bitmap);
                attributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default,
                                                       ColorAdjustType.Bitmap);
                attributes.SetGamma(gamma, ColorAdjustType.Bitmap);
                g.DrawImage(bmp0, new Rectangle(0, 0, bmp0.Width, bmp0.Height),
                            0, 0, bmp0.Width, bmp0.Height, GraphicsUnit.Pixel, attributes);
            }
            return bmp1;
        }
    }
}
