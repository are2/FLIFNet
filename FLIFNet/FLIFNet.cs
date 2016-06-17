/*
libflif C# wrapper

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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FLIFNetWrapper
{
    public static class FLIFNet
    {
        public readonly static Version LibraryVersion = new Version(0, 1);

        internal struct FLIF_DECODER
        {
            Int32 quality;
            UInt32 scale;
            delegate void callback();
            Int32 first_quality;
            UInt32 rw;
            UInt32 rh;
            Int32 crc_check;
        }

        public class FlifImage
        {
            private byte[] imageData = null;
            private uint width, height;

            public uint Width { get { return width; } }
            public uint Height { get { return height; } }

            public byte[] ImageData { get { return imageData; } }

            internal FlifImage(out FLIF_DECODER decoder, uint index)
            {
                IntPtr decoded = flif_decoder_get_image(out decoder, new UIntPtr(index));
                
                width = flif_image_get_width(decoded);
                height = flif_image_get_height(decoded);

                byte[] imageBuffer = new byte[height * width * 4];
                byte[] rowBuffer = new byte[width * 4];
                for (int i = 0; i < height; i++)
                {
                    flif_image_read_row_RGBA8(decoded, (uint)i, rowBuffer, new UIntPtr((uint)rowBuffer.Length));
                    Array.Copy(rowBuffer, 0, imageBuffer, i * rowBuffer.Length, rowBuffer.Length);
                }
                flif_destroy_image(decoded);

                imageData = imageBuffer;
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

        public class FlifDecoder
        {
            private FLIFNet.FLIF_DECODER decoder;

            public FlifDecoder()
            {
                try
                {
                    decoder = FLIFNet.flif_create_decoder();
                }
                catch (Exception)
                {
                    throw;
                }
                
            }

            ~FlifDecoder()
            {
                try
                {
                    Abort();
                    FLIFNet.flif_destroy_decoder(out decoder);
                }
                catch (Exception)
                {

                    throw;
                }
                
            }

            public int Abort()
            {
                try
                {
                    return flif_abort_decoder(out decoder);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public int DecodeFile(string filename)
            {
                try
                {
                    return flif_decoder_decode_file(out decoder, filename);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public int DecodeMemory(string filename, ref byte[] buffer)
            {
                try
                {
                    return flif_decoder_decode_memory(out decoder, out buffer, new UIntPtr((uint)buffer.Length));
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public void SetQuality(int quality)
            {
                try
                {
                    flif_decoder_set_quality(out decoder, quality);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public void SetScale(uint scale)
            {
                try
                {
                    flif_decoder_set_scale(out decoder, scale);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public FlifImage GetImage(uint index)
            {
                try
                {
                    return new FlifImage(out decoder, index);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        //FLIF decoder
        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern FLIF_DECODER flif_create_decoder();

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_destroy_decoder(out FLIF_DECODER decoder);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern Int32 flif_abort_decoder(out FLIF_DECODER decoder);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern Int32 flif_decoder_decode_file(out FLIF_DECODER decoder, string filename);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern Int32 flif_decoder_decode_memory(out FLIF_DECODER decoder, out byte[] buffer, UIntPtr buffer_size_bytes);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_decoder_set_quality(out FLIF_DECODER decoder, Int32 quality);    

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_decoder_set_scale(out FLIF_DECODER decoder, UInt32 scale);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr flif_decoder_get_image(out FLIF_DECODER decoder, UIntPtr  index);


        //FLIF common
        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern UInt32 flif_image_get_width(IntPtr image);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern UInt32 flif_image_get_height(IntPtr image);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_destroy_image(IntPtr image);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_image_read_row_RGBA8(IntPtr image, UInt32 row, byte[] buffer, UIntPtr buffer_size_bytes);
    }


}
