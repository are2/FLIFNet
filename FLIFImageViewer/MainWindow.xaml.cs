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
            string[] args = Environment.GetCommandLineArgs();
            string filename = null;
            if (args.Length > 1)
            {
                filename = args[1];
            }
            else
            {
                // Create OpenFileDialog 
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".flif";
                dlg.Filter = "Free Lossless Image Format files (*.flif)|*.flif";

                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    filename = dlg.FileName;
                }
                else
                {
                    this.Close();
                }
            }

            LoadImage(filename);
        }

        private void LoadImage(string filename)
        {
            //Decode file
            FLIFNet.FlifDecoder Decoder = new FLIFNet.FlifDecoder();
            Decoder.SetQuality(100);
            Decoder.SetScale(1);
            int res = Decoder.DecodeFile(filename);
            if (res == 0)
            {
                MessageBox.Show("File decoding error!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Get image
            FLIFNet.FlifImage Image = Decoder.GetImage(0);

            //Draw
            image.Source = Image.GetWritableBitmap();

            //Resize window
            this.Width = Image.Width;
            this.Height = Image.Height;
            this.Left = 0;
            this.Top = 0;
        }
    }
}
