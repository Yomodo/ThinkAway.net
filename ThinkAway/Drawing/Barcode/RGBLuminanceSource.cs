using System.Drawing;
using System;

namespace ThinkAway.Drawing.Barcode
{
    /// <summary>
    /// 
    /// </summary>
    public class RGBLuminanceSource : LuminanceSource
    {

        private readonly sbyte[] _luminances;
        private bool _isRotated;

        override public int Height
        {
            get
            {
                if (!_isRotated)
                    return _height;
                return _width;
            }
        }
        override public int Width
        {
            get
            {
                if (!_isRotated)
                    return _width;
                return _height;
            }
        }
        private readonly int _height;
        private readonly int _width;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="W"></param>
        /// <param name="H"></param>
        public RGBLuminanceSource(byte[] d, int W, int H)
            : base(W, H)
        {
            _width = W;
            _height = H;
            int width = W;
            int height = H;
            // In order to measure pure decoding speed, we convert the entire image to a greyscale array
            // up front, which is the same as the Y channel of the YUVLuminanceSource in the real app.
            _luminances = new sbyte[width * height];
            for (int y = 0; y < height; y++)
            {
                int offset = y * width;
                for (int x = 0; x < width; x++)
                {
                    int r = d[offset * 3 + x * 3];
                    int g = d[offset * 3 + x * 3 + 1];
                    int b = d[offset * 3 + x * 3 + 2];
                    if (r == g && g == b)
                    {
                        // Image is already greyscale, so pick any channel.
                        _luminances[offset + x] = (sbyte)r;
                    }
                    else
                    {
                        // Calculate luminance cheaply, favoring green.
                        _luminances[offset + x] = (sbyte)((r + g + g + b) >> 2);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="W"></param>
        /// <param name="H"></param>
        /// <param name="is8Bit"></param>
        public RGBLuminanceSource(byte[] d, int W, int H,bool is8Bit)
            : base(W, H)
        {
            _width = W;
            _height = H;
            _luminances = new sbyte[W * H];
            System.Buffer.BlockCopy(d,0, _luminances,0, W * H);
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="W"></param>
        /// <param name="H"></param>
        /// <param name="is8Bit"></param>
        /// <param name="Region"></param>
        public RGBLuminanceSource(byte[] d, int W, int H, bool is8Bit,Rectangle Region)
            : base(W, H)
        {
            _width = Region.Width;
            _height = Region.Height;
            //luminances = Red.Imaging.Filters.CropArea(d, W, H, Region);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="W"></param>
        /// <param name="H"></param>
        public RGBLuminanceSource(Bitmap d, int W, int H)
            : base(W, H)
        {
            int width = _width = W;
            int height = _height = H;
            // In order to measure pure decoding speed, we convert the entire image to a greyscale array
            // up front, which is the same as the Y channel of the YUVLuminanceSource in the real app.
            _luminances = new sbyte[width * height];
            //if (format == PixelFormat.Format8bppIndexed)
            {
                for (int y = 0; y < height; y++)
                {
                    int offset = y * width;
                    for (int x = 0; x < width; x++)
                    {
                        Color c = d.GetPixel(x, y);
                        _luminances[offset + x] = (sbyte)(c.R << 16 | c.G << 8 | c.B);
                    }
                }
            }
        }
        override public sbyte[] getRow(int y, sbyte[] row)
        {
            if (_isRotated == false)
            {
                int width = Width;
                if (row == null || row.Length < width)
                {
                    row = new sbyte[width];
                }
                for (int i = 0; i < width; i++)
                    row[i] = _luminances[y * width + i];
                //System.arraycopy(luminances, y * width, row, 0, width);
                return row;
            }
            else
            {
                int width = _width;
                int height = _height;
                if (row == null || row.Length < height)
                {
                    row = new sbyte[height];
                }
                for (int i = 0; i < height; i++)
                    row[i] = _luminances[i * width + y];
                //System.arraycopy(luminances, y * width, row, 0, width);
                return row;
            }
        }
        public override sbyte[] Matrix
        {
            get { return _luminances; }
        }

        public override LuminanceSource rotateCounterClockwise()
        {
            _isRotated = true;
            return this;
        }
        public override bool RotateSupported
        {
            get
            {
                return true;
            }

        }
    }
}
