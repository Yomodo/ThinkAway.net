using System.Drawing;
using ThinkAway.Drawing.Barcode.Common;

namespace ThinkAway.Drawing.Barcode
{
    public class QRHelper
    {
        public string  DecodeQrImage(string fileName)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(fileName);
            Bitmap bmap = new Bitmap(img);
            LuminanceSource source = new RGBLuminanceSource(bmap, bmap.Width, bmap.Height);
            BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
            Result result = new MultiFormatReader().Decode(bitmap);
            return result.Text;
        }

        public System.Drawing.Image MakeQrImage(string content)
        {
            MultiFormatWriter multiFormatWriter = new MultiFormatWriter();
            ByteMatrix byteMatrix = multiFormatWriter.Encode(content, BarcodeFormat.QR_CODE, 350, 350);
            System.Drawing.Image image = ToBitmap(byteMatrix);
            return image;
        }

        public static Bitmap ToBitmap(ByteMatrix matrix)
        {
            int width = matrix.Width;
            int height = matrix.Height;
            Bitmap bmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bmap.SetPixel(x, y,
                                  matrix.GetRenamed(x, y) != -1
                                      ? ColorTranslator.FromHtml("0xFF000000")
                                      : ColorTranslator.FromHtml("0xFFFFFFFF"));
                }
            }
            return bmap;
        }
    }
}
