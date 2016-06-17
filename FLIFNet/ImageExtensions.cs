using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static FLIFNetWrapper.FLIFNet;

namespace FLIFNetWrapper
{
    public static class ImageExtensions
    {
        public static WriteableBitmap GetWritableBitmap(this FlifImage Image)
        {
            WriteableBitmap wb = new WriteableBitmap((int)Image.Width, (int)Image.Height, 96, 96, PixelFormats.Bgra32, null);
            Int32Rect _rect = new Int32Rect(0, 0, wb.PixelWidth, wb.PixelHeight);
            int _bytesPerPixel = (wb.Format.BitsPerPixel + 7) / 8;
            int _stride = wb.PixelWidth * _bytesPerPixel;

            wb.WritePixels(_rect, ImageExtensions.RGBA2BGRA(Image.ImageData), _stride, 0);
            return wb;
        }

        public static byte[] RGBA2BGRA(byte[] imageData)
        {
            byte[] buffer = new byte[imageData.Length];
            for (int i = 0; i < imageData.Length; i += 4)
            {
                buffer[i] = imageData[i + 2];
                buffer[i + 1] = imageData[i + 1];
                buffer[i + 2] = imageData[i];
                buffer[i + 3] = imageData[i + 3];
            }
            return buffer;
        }
    }
}
