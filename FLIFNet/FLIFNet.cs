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

        public class FlifImage
        {
            private byte[] imageData = null;
            private uint width, height;
            private IntPtr d;

            public uint Width { get { return width; } }
            public uint Height { get { return height; } }

            public byte[] ImageData { get { return imageData; } }

            internal FlifImage(IntPtr decoder, uint index)
            {
                IntPtr decoded = flif_decoder_get_image(decoder, new UIntPtr(index));
                d = decoded;

                width = flif_image_get_width(decoded);
                height = flif_image_get_height(decoded);

                byte[] imageBuffer = new byte[height * width * 4];
                byte[] rowBuffer = new byte[width * 4];
                for (int i = 0; i < height; i++)
                {
                    flif_image_read_row_RGBA8(decoded, (uint)i, rowBuffer, rowBuffer.Length);
                    Array.Copy(rowBuffer, 0, imageBuffer, i * rowBuffer.Length, rowBuffer.Length);
                }

                imageData = imageBuffer;
            }

            public void Free()
            {
                if (d != IntPtr.Zero)
                {
                    flif_destroy_image(d);
                    d = IntPtr.Zero;
                }
                
            }
        }

        public class FlifDecoder
        {
            private IntPtr decoder;
            private event FLIFNet.QualityReachedCallback _OnQualityReached = null;

            public event FLIFNet.QualityReachedCallback OnQualityReached
            {
                add
                {
                    _OnQualityReached = value;
                    FLIFNet.flif_decoder_set_callback(decoder, _OnQualityReached);
                }

                remove
                {
                    _OnQualityReached = null;
                    FLIFNet.flif_decoder_set_callback(decoder, _OnQualityReached);
                }
            }

            public FlifDecoder()
            {
                try
                {
                    decoder = FLIFNet.flif_create_decoder();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            
            ~FlifDecoder()
            {
                try
                {
                    if (decoder != IntPtr.Zero)
                    {
                        FLIFNet.flif_destroy_decoder(decoder);
                        decoder = IntPtr.Zero;
                    }
                    
                }
                catch (Exception ex)
                {
                }
            }
            

            public int Abort()
            {
                try
                {
                    return flif_abort_decoder(decoder);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public int DecodeFile(string filename)
            {
                try
                {
                    return flif_decoder_decode_file(decoder, filename);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public int DecodeMemory(string filename, ref byte[] buffer)
            {
                try
                {
                    return flif_decoder_decode_memory(decoder, buffer, buffer.Length);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public void SetQuality(int quality)
            {
                try
                {
                    flif_decoder_set_quality(decoder, quality);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public void SetScale(uint scale)
            {
                try
                {
                    flif_decoder_set_scale(decoder, scale);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public FlifImage GetImage(uint index)
            {
                try
                {
                    return new FlifImage(decoder, index);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public void SetFirstCallbackQuality(int quality)
            {
                try
                {
                    FLIFNet.flif_decoder_set_first_callback_quality(decoder, quality);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        //    public void SetCallback()
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int QualityReachedCallback(Int32 quality, Int64 bytes_read);

        //FLIF decoder
        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr flif_create_decoder();

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern Int32 flif_decoder_num_images(IntPtr decoder);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern Int32 flif_decoder_num_loops(IntPtr decoder);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_destroy_decoder(IntPtr decoder);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern Int32 flif_abort_decoder(IntPtr decoder);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern Int32 flif_decoder_decode_file(IntPtr decoder, string filename);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern Int32 flif_decoder_decode_memory(IntPtr decoder, byte[] buffer, Int32 buffer_size_bytes);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_decoder_set_quality(IntPtr decoder, Int32 quality);    

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_decoder_set_scale(IntPtr decoder, UInt32 scale);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_decoder_set_crc_check(IntPtr decoder, Int32 crc_check);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_decoder_set_resize(IntPtr decoder, UInt32 width, UInt32 height);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr flif_decoder_get_image(IntPtr decoder, UIntPtr  index);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_decoder_set_callback(IntPtr decoder, [MarshalAs(UnmanagedType.FunctionPtr)] QualityReachedCallback report_status_callback);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_decoder_set_first_callback_quality(IntPtr decoder, Int32 quality);

        //FLIF common
        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern UInt32 flif_image_get_width(IntPtr image);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern UInt32 flif_image_get_height(IntPtr image);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_destroy_image(IntPtr image);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_image_read_row_RGBA8(IntPtr image, UInt32 row, byte[] buffer, Int32 buffer_size_bytes);

        //FLIF decoder
        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr flif_create_encoder();

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_encoder_add_image(IntPtr encoder, IntPtr image);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern Int32 flif_encoder_encode_file(IntPtr encoder, string filename);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern Int32 flif_encoder_encode_memory(IntPtr encoder, ref IntPtr buffer, Int32 buffer_size_bytes);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_destroy_encoder(IntPtr encoder);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_encoder_set_interlaced(IntPtr encoder, UInt32 interlaced);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_encoder_set_learn_repeat(IntPtr encoder, UInt32 learn_repeats);



        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_encoder_set_auto_color_buckets(IntPtr encoder, UInt32 acb);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_encoder_set_palette_size(IntPtr encoder, Int32 palette_size);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_encoder_set_lookback(IntPtr encoder, Int32 lookback);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_encoder_set_divisor(IntPtr encoder, Int32 divisor);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_encoder_set_min_size(IntPtr encoder, Int32 min_size);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_encoder_set_split_threshold(IntPtr encoder, Int32 threshold);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_encoder_set_alpha_zero_lossless(IntPtr encoder);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_encoder_set_chance_cutoff(IntPtr encoder, Int32 cutoff);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_encoder_set_chance_alpha(IntPtr encoder, Int32 alpha);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_encoder_set_crc_check(IntPtr encoder, UInt32 crc_check);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_encoder_set_channel_compact(IntPtr encoder, UInt32 plc);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_encoder_set_ycocg(IntPtr encoder, UInt32 ycocg);

        [DllImport("libflif.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void flif_encoder_set_frame_shape(IntPtr encoder, UInt32 frs);


    }


}
