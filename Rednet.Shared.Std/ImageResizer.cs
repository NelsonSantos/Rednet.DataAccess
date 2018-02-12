using System;
using System.Collections.Generic;
using System.Text;

using System;
using System.IO;

// Usings je Platform

#if __IOS__
using System.Drawing;
using UIKit;
using CoreGraphics;
#endif


#if DROID
using Android.Graphics;
#endif

#if WINDOWS_PHONE
using Microsoft.Phone;
using System.Windows.Media.Imaging;
#endif

namespace Rednet.Shared
{
    public static class ImageResizer
    {
        public static byte[] ResizeImage(byte[] imageData, float width, float height, int quality)
        {
#if __IOS__
            return ResizeImageIOS(imageData, width, height);
#endif
#if DROID
            return ResizeImageAndroid(imageData, width, height, quality);
#endif
#if WINDOWS_PHONE
            return ResizeImageWinPhone(imageData, width, height);
#endif
            return null;
        }

        //
#if __IOS__

        public static byte[] ResizeImageIOS(byte[] imageData, float width, float height)
        {
            // Load the bitmap
            UIImage originalImage = ImageFromByteArray(imageData);
            //
            var Hoehe = originalImage.Size.Height;
            var Breite = originalImage.Size.Width;
            //
            nfloat ZielHoehe = 0;
            nfloat ZielBreite = 0;
            //

            if (Hoehe > Breite) // Höhe (71 für Avatar) ist Master
            {
                ZielHoehe = height;
                nfloat teiler = Hoehe / height;
                ZielBreite = Breite / teiler;
            }
            else // Breite (61 for Avatar) ist Master
            {
                ZielBreite = width;
                nfloat teiler = Breite / width;
                ZielHoehe = Hoehe / teiler;
            }
            //
            width = (float)ZielBreite;
            height = (float)ZielHoehe;
            //
            UIGraphics.BeginImageContext(new SizeF(width, height));
            originalImage.Draw(new RectangleF(0, 0, width, height));
            var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            //
            var bytesImagen = resizedImage.AsJPEG().ToArray();
            resizedImage.Dispose();
            return bytesImagen;
        }

        //
        public static UIKit.UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            //
            UIKit.UIImage image;
            try
            {
                image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
            }
            catch (Exception e)
            {
                Console.WriteLine("Image load failed: " + e.Message);
                return null;
            }
            return image;
        }
#endif
        //

#if DROID
        public static byte[] ResizeImageAndroid(byte[] imageData, float width, float height, int quality)
        {
            // Load the bitmap 
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            //
            float ZielHoehe = 0;
            float ZielBreite = 0;
            //
            var Hoehe = originalImage.Height;
            var Breite = originalImage.Width;
            //
            if (Hoehe > Breite) // Höhe (71 für Avatar) ist Master
            {
                ZielHoehe = height;
                float teiler = Hoehe / height;
                ZielBreite = Breite / teiler;
            }
            else // Breite (61 für Avatar) ist Master
            {
                ZielBreite = width;
                float teiler = Breite / width;
                ZielHoehe = Hoehe / teiler;
            }
            //
            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)ZielBreite, (int)ZielHoehe, false);
            // 
            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, quality, ms);
                return ms.ToArray();
            }
        }
#endif
        //

#if WINDOWS_PHONE
        public static byte[] ResizeImageWinPhone(byte[] imageData, float width, float height)
        {
            byte[] resizedData;


            using (MemoryStream streamIn = new MemoryStream(imageData))
            {
                WriteableBitmap bitmap = PictureDecoder.DecodeJpeg(streamIn, (int)width, (int)height);
                //
                float ZielHoehe = 0;
                float ZielBreite = 0;
                //
                float Hoehe = bitmap.PixelHeight;
                float Breite = bitmap.PixelWidth;
                //
                if (Hoehe > Breite) // Höhe (71 für Avatar) ist Master
                {
                    ZielHoehe = height;
                    float teiler = Hoehe / height;
                    ZielBreite = Breite / teiler;
                }
                else // Breite (61 für Avatar) ist Master
                {
                    ZielBreite = width;
                    float teiler = Breite / width;
                    ZielHoehe = Hoehe / teiler;
                }
                //                
                using (MemoryStream streamOut = new MemoryStream())
                {
                    bitmap.SaveJpeg(streamOut, (int)ZielBreite, (int)ZielHoehe, 0, 100);
                    resizedData = streamOut.ToArray();
                }
            }
            return resizedData;
        }
#endif
    }
}
