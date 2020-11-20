using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using System.Drawing.Imaging;
using System.Drawing;

namespace FtpApp.Models.Utilities
{
    public class Converter
    {/// <summary>
     /// A less mathmatical approach to ASCII to Binary conversion
     /// /*https://www.fluxbytes.com/csha*/rp/convert-string-to-binary-and-binary-to-string-in-c/
     /// </summary>
     /// <param name="text">String to convert</param>
     /// <returns>Binary encoded string</returns>
        public static string StringToBinary(string text)
        {
            StringBuilder sb = new StringBuilder();
       
            foreach (char c in text.ToCharArray())
            {
                //Convert the char to base 2 and pad the output with 0
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }


        /// <summary>
        /// Convert a Binary text string to a Text string
        /// </summary>
        /// <param name="text">Binary encoded string</param>
        /// <returns>Text string</returns>
        public static string BinaryToString(string text)
        {
            List<byte> bytes = new List<byte>();

            for (int i = 0; i < text.Length; i += 8)
            {
                bytes.Add(Convert.ToByte(text.Substring(i, 8), 2));
            }
            return Encoding.ASCII.GetString(bytes.ToArray());
        }




     


        /// <summary>
        /// An approach to ASCII to Hexadecimal conversion using ToString("X2")
        /// </summary>
        /// <param name="data">String to convert</param>
        /// <returns></returns>
        public static string StringToHex2(string data)
        {
            StringBuilder sb = new StringBuilder();

            byte[] bytearray = Encoding.ASCII.GetBytes(data);

            foreach (byte bytepart in bytearray)
            {
                sb.Append(bytepart.ToString("X2"));
            }

            return sb.ToString().ToUpper();
        }


        /// <summary>
        /// Converts a Hexadecimal string to ASCII string
        /// </summary>
        /// <param name="hexString">Hexadecimal string</param>
        /// <returns>ASCII string</returns>
        public static string HexToString(string hexString)
        {
            if (hexString == null || (hexString.Length & 1) == 1)
            {
                throw new ArgumentException();
            }
            var sb = new StringBuilder();
            for (var i = 0; i < hexString.Length; i += 2)
            {
                var hexChar = hexString.Substring(i, 2);
                sb.Append((char)Convert.ToByte(hexChar, 16));
            }
            return sb.ToString();
        }



        /// <summary>
        /// A more mathmatical approach to ASCII to Binary conversion
        /// https://forums.asp.net/t/1713174.aspx?How+to+convert+ASCII+value+to+binary+value+using+c+net
        /// </summary>
        /// <param name="data">String to convert</param>
        /// <returns>Binary encoded string</returns>
        public static string StringToBinary2(string data)
        {
            string converted = string.Empty;
            // convert string to byte array
            byte[] bytes = Encoding.ASCII.GetBytes(data);

            for (int i = 0; i < bytes.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    converted += (bytes[i] & 0x80) > 0 ? "1" : "0";
                    bytes[i] <<= 1;
                }
            }

            return converted;
        
        }


        /// <summary>
        /// Converts an Image Byte Array to an Image Object
        /// </summary>
        /// <param name="byteArrayIn">Array of Image Bytes</param>
        /// <returns>Image Object</returns>
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            Image returnImage = null;

            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
                ms.Write(byteArrayIn, 0, byteArrayIn.Length);
                returnImage = Image.FromStream(ms, true);
            }
            catch { }

            return returnImage;
        }

        /// <summary>
        /// Converts an Image object to Base64
        /// </summary>
        /// <param name="image">An Image object</param>
        /// <param name="format">The format of the image (JPEG, BMP, etc.)</param>
        /// <returns>Base64 encoded string representation of an Image</returns>
        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        } 


        /// <summary>
        /// Converts a Base64 encoded string to an Image
        /// </summary>
        /// <param name="base64String">Base64 encoded Image string</param>
        /// <returns>Decoded Image</returns>
        public static Image Base64ToImage(string base64String)
        {
            try
            {
                // Convert Base64 String to byte[]
                byte[] imageBytes = Convert.FromBase64String(base64String.Trim());
                var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

                // Convert byte[] to Image
                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms, true);
                return image;
            }
            catch (Exception e)
            {

            }

            //Something went wrong in the Base64 string
            return null;
        }




    }
}
