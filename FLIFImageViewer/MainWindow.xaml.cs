/*
FLIF image viewer

Built on the libflif library.

Copyright 2016, Antti Arekallio, LGPL v3+

 This program is free software: you can redistribute it and/or modify
 it under the terms of the GNU Lesser General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Lesser General Public License
 along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FLIFNetWrapper;

namespace FLIFImageViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".flif";
            dlg.Filter = "Free Lossless Image Format files (*.flif)|*.flif";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                LoadImage(dlg.FileName);
            }
            else
            {
                this.Close();
            }
        }

        private void LoadImage(string filename)
        {
            //Load image
            FLIFNet.FlifDecoder Decoder = new FLIFNet.FlifDecoder();
            Decoder.SetQuality(100);
            Decoder.SetScale(1);
            int res = Decoder.DecodeFile(filename);
            Console.WriteLine(res.ToString());
            FLIFNet.FlifImage Image = Decoder.GetImage(0);

            //Draw
            WriteableBitmap wb = new WriteableBitmap((int)Image.Width, (int)Image.Height, 96, 96, PixelFormats.Bgra32, null);
            Int32Rect _rect = new Int32Rect(0, 0, wb.PixelWidth, wb.PixelHeight);
            int _bytesPerPixel = (wb.Format.BitsPerPixel + 7) / 8;
            int _stride = wb.PixelWidth * _bytesPerPixel;

            wb.WritePixels(_rect, FLIFNet.FlifImage.RGBA2BGRA(Image.ImageData), _stride, 0);
            image.Source = wb;

            this.Width = Image.Width;
            this.Height = Image.Height;
        }
    }
}
