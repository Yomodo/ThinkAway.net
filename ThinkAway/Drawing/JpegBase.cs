using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;
using System.Linq;

/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Drawing
{
    /**/
    /// <summary>
    /// ASPJpegBase
    /// Author : Jolly
    /// </summary>
    public class JpegBase : IDisposable
    {
        /// <summary>
        /// 会产生graphics异常的PixelFormat
        /// </summary>
        private static readonly PixelFormat[] IndexedPixelFormats = {
                                                                        PixelFormat.Undefined,
                                                                        PixelFormat.DontCare,
                                                                        PixelFormat.Format16bppArgb1555,
                                                                        PixelFormat.Format1bppIndexed,
                                                                        PixelFormat.Format4bppIndexed,
                                                                        PixelFormat.Format8bppIndexed
                                                                    };
        /**/
        /// <summary>
        /// 图片宽度--生成缩略图时使用
        /// </summary>
        public int Width { get; set; }
        /**/
        /// <summary>
        /// 图片高度--生成缩略图时使用
        /// </summary>
        public int Height { get; set; }
        /**/
        /// <summary>
        /// Bitmap对象
        /// </summary>
        public Bitmap CurrentBitmap
        {
            get { if (_oCurrentImage == null) throw new NullReferenceException("CurrentBitmap is null!"); return _oCurrentImage; }
        }
        private Bitmap _oCurrentImage;

        /**/
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public JpegBase()
        {
        }
        /**/
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sImagePath">图片路径</param>
        public JpegBase(string sImagePath)
        {
            _oCurrentImage = new Bitmap(sImagePath);
            //如果原图片是索引像素格式之列的，则需要转换
            if (IsPixelFormatIndexed(_oCurrentImage.PixelFormat))
            {
                using (Bitmap bmp = new Bitmap(_oCurrentImage.Width, _oCurrentImage.Height, PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.DrawImage(_oCurrentImage, 0, 0);
                    }
                    _oCurrentImage = (Bitmap)bmp.Clone();
                }
            }
        }
        /**/
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="oImage">Image对象</param>
        public JpegBase(System.Drawing.Image oImage)
        {
            _oCurrentImage = new Bitmap(oImage);
        }
        /**/
        /// <summary>
        /// 实例化Bitmap对象
        /// </summary>
        /// <param name="sImagePath">图片路径</param>
        public void Open(string sImagePath)
        {
            _oCurrentImage = new Bitmap(sImagePath);
        }
        /**/
        /// <summary>
        /// 判断图片的PixelFormat 是否在 引发异常的 PixelFormat 之中
        /// 无法从带有索引像素格式的图像创建graphics对象
        /// </summary>
        /// <param name="imgPixelFormat">原图片的PixelFormat</param>
        /// <returns></returns>
        private static bool IsPixelFormatIndexed(PixelFormat imgPixelFormat)
        {
            return IndexedPixelFormats.Contains(imgPixelFormat);
        }

        /**/

        /// <summary>
        /// 添加文字水印
        /// </summary>
        /// <param name="p"></param>
        /// <param name="fontcolor"></param>
        /// <param name="sText"></param>
        /// <param name="font"></param>
        /// <param name="rotate"></param>
        public void DrawText(string sText, Font font, Color fontcolor, Point p, float rotate)
        {
            using (Graphics graphics = Graphics.FromImage(this.CurrentBitmap))
            {
                if (Math.Abs(rotate - 0) > 0)
                    graphics.RotateTransform(rotate);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawString(sText, font, new SolidBrush(fontcolor), p.X, p.Y);
            }
        }
        /**/

        /// <summary>
        /// 添加文字水印-文字带背景
        /// </summary>
        /// <param name="p"></param>
        /// <param name="fontcolor"></param>
        /// <param name="sText"></param>
        /// <param name="font"></param>
        /// <param name="rotate"></param>
        /// <param name="sImagePath"></param>
        public void DrawText(string sText, Font font, Color fontcolor, Point p, float rotate, string sImagePath)
        {
            TextureBrush tb = new TextureBrush(System.Drawing.Image.FromFile(sImagePath));
            using (Graphics graphics = Graphics.FromImage(this.CurrentBitmap))
            {
                if (Math.Abs(rotate - 0) > 0)
                    graphics.RotateTransform(rotate);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawString(sText, font, tb, p.X, p.Y);
            }
        }
        /**/
        /// <summary>
        /// 绘制边框
        /// </summary>
        /// <param name="frameColor">边框颜色</param>
        /// <param name="frameWidth">边框宽度</param>
        public void DrawFrame(Color frameColor, int frameWidth)
        {
            double iNewframeWidth = (Convert.ToDouble(frameWidth) / Convert.ToDouble(2));
            using (Bitmap newBitmap = new Bitmap(this.CurrentBitmap.Width + (2 * frameWidth), this.CurrentBitmap.Height + (2 * frameWidth)))
            {
                using (Graphics graphics = Graphics.FromImage(newBitmap))
                {
                    graphics.Clear(Color.White);
                    Pen pen = new Pen(frameColor, frameWidth);
                    graphics.DrawRectangle(pen, (float)iNewframeWidth, (float)iNewframeWidth, newBitmap.Width - frameWidth, newBitmap.Height - frameWidth);
                    graphics.DrawImage(this.CurrentBitmap, new Rectangle(frameWidth, frameWidth, this.CurrentBitmap.Width, this.CurrentBitmap.Height));
                    this._oCurrentImage = ((Bitmap)newBitmap.Clone());
                }
            }
        }
        /**/
        /// <summary>
        /// 绘制3D边框
        /// </summary>
        /// <param name="frameColor">边框颜色</param>
        /// <param name="frameWidth">边框宽度</param>
        public void Draw3DFrame(Color frameColor, int frameWidth)
        {
            double iNewframeWidth = (Convert.ToDouble(frameWidth) / Convert.ToDouble(2));
            using (Bitmap newBitmap = new Bitmap(this.CurrentBitmap.Width + (2 * frameWidth), this.CurrentBitmap.Height + (2 * frameWidth)))
            {
                using (Graphics graphics = Graphics.FromImage(newBitmap))
                {
                    graphics.Clear(Color.White);
                    Pen pen = new Pen(frameColor, (float)iNewframeWidth);
                    Pen pen1 = new Pen(Color.FromArgb(frameColor.ToArgb() - 10), (float)iNewframeWidth);
                    graphics.DrawRectangle(pen, (float)iNewframeWidth / 2, (float)iNewframeWidth / 2, newBitmap.Width - (float)iNewframeWidth, newBitmap.Height - (float)iNewframeWidth);
                    graphics.DrawRectangle(pen1, 3 * (float)iNewframeWidth / 2, 3 * (float)iNewframeWidth / 2, newBitmap.Width - 3 * (float)iNewframeWidth, newBitmap.Height - 3 * (float)iNewframeWidth);
                    graphics.DrawImage(this.CurrentBitmap, 2 * (float)iNewframeWidth, 2 * (float)iNewframeWidth, this.CurrentBitmap.Width, this.CurrentBitmap.Height);
                    this._oCurrentImage = (Bitmap)newBitmap.Clone();
                }
            }
        }
        /**/
        /// <summary>
        /// 图片切割功能
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="top">top</param>
        /// <param name="right">right</param>
        /// <param name="bottom">bottom</param>
        public void Crop(int left, int top, int right, int bottom)
        {
            int num;
            int num2;
            if ((this.CurrentBitmap.Width - left) < right)
            {
                num = this.CurrentBitmap.Width - right;
            }
            else
            {
                num = right;
            }
            if ((this.CurrentBitmap.Height - top) < bottom)
            {
                num2 = this.CurrentBitmap.Height - top;
            }
            else
            {
                num2 = bottom;
            }
            using (Bitmap image = new Bitmap(num, num2))
            {
                image.SetResolution(this.CurrentBitmap.HorizontalResolution, this.CurrentBitmap.VerticalResolution);
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    graphics.Clear(Color.White);
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.DrawImage(this.CurrentBitmap, new Rectangle(0, 0, num, num2), new Rectangle(left, top, num, num2), GraphicsUnit.Pixel);
                    _oCurrentImage = (Bitmap)image.Clone();
                }
            }
        }
        /**/
        /// <summary>
        /// 图片移动
        /// </summary>
        /// <param name="lefttopx"></param>
        /// <param name="lefttopy"></param>
        /// <param name="rightbottomx"></param>
        /// <param name="rightbottomy"></param>
        /// <param name="tox"></param>
        /// <param name="toy"></param>
        public void Move(int lefttopx, int lefttopy, int rightbottomx, int rightbottomy, int tox, int toy)
        {
            using (Bitmap image = this.Copy(lefttopx, lefttopy, rightbottomx, rightbottomy))
            {
                Rectangle rect = GetRectangle(lefttopx, lefttopy, rightbottomx, rightbottomy);
                using (Graphics graphics = Graphics.FromImage(this.CurrentBitmap))
                {
                    graphics.Clear(Color.White);
                    graphics.FillRectangle(new SolidBrush(Color.White), rect);
                    graphics.DrawImage(image, tox, toy);
                    _oCurrentImage = (Bitmap)image.Clone();
                }
            }
        }
        /**/
        /// <summary>
        /// 调整图片大小
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void ResizeTo(int width, int height)
        {
            this._oCurrentImage = new Bitmap(this.CurrentBitmap, width, height);
        }
        /**/
        /// <summary>
        /// 生成缩略图
        /// </summary>
        public void CreateThumbnail(int iWidth, int iHeight)
        {
            //用指定的大小和格式初始化 Bitmap 类的新实例
            using (Bitmap bitmap = (Bitmap)_oCurrentImage.Clone())
            {
                ResizeTo(iWidth, iHeight);
                //在指定位置并且按指定大小绘制 原图片 对象
                Graphics.FromImage(_oCurrentImage).DrawImage(bitmap, new Rectangle(0, 0, iWidth, iHeight));
            }
        }
        /**/
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="sThumbnailPath">缩略图保存路径</param>
        public void CreateThumbnail(string sThumbnailPath)
        {
            if (Width > 0 && Height > 0 && (Width < _oCurrentImage.Width || Height < _oCurrentImage.Height))
            {
                int width = Width;
                int height = Height;
                double factor;
                if (_oCurrentImage.Width > _oCurrentImage.Height)
                {
                    factor = Convert.ToDouble(width) / Convert.ToDouble(_oCurrentImage.Width);
                    height = Convert.ToInt32(_oCurrentImage.Height * factor);
                }
                else
                {
                    factor = Convert.ToDouble(height) / Convert.ToDouble(_oCurrentImage.Height);
                    width = Convert.ToInt32(_oCurrentImage.Width * factor);
                }
                //用指定的大小和格式初始化 Bitmap 类的新实例
                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                //从指定的 Image 对象创建新 Graphics 对象
                Graphics graphics = Graphics.FromImage(bitmap);
                //清除整个绘图面并以透明背景色填充
                graphics.Clear(Color.Transparent);
                //在指定位置并且按指定大小绘制 原图片 对象
                graphics.DrawImage(_oCurrentImage, new Rectangle(0, 0, width, height));
                try
                {
                    bitmap.Save(sThumbnailPath);
                }
                finally
                {
                    bitmap.Dispose();
                    graphics.Dispose();
                }
            }
        }

        /**/
        /// <summary>
        /// 图片垂直翻转
        /// </summary>
        public void FlipV()
        {
            this.CurrentBitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
        }
        /**/
        /// <summary>
        /// 图片水平翻转
        /// </summary>
        public void FlipH()
        {
            this.CurrentBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }
        /**/
        /// <summary>
        /// 图片旋转
        /// </summary>
        /// <param name="angle">旋转度数</param>
        public void Rotate(int angle)
        {
            angle = angle % 360;
            if (angle < 0)
            {
                angle += 360;
            }
            if (angle >= 270)
            {
                this.CurrentBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                angle -= 270;
            }
            else if (angle >= 180)
            {
                this.CurrentBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                angle -= 180;
            }
            else if (angle >= 90)
            {
                this.CurrentBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                angle -= 90;
            }
            if (angle > 0)
            {
                double a = (angle * 3.1415926535897931) / 180.0;
                int width = Convert.ToInt32((this.CurrentBitmap.Height * Math.Sin(a)) + (this.CurrentBitmap.Width * Math.Cos(a)));
                int height = Convert.ToInt32((this.CurrentBitmap.Width * Math.Sin(a)) + (this.CurrentBitmap.Height * Math.Cos(a)));
                Bitmap image = new Bitmap(width, height * 2);
                image.SetResolution(this.CurrentBitmap.HorizontalResolution, this.CurrentBitmap.VerticalResolution);
                Graphics graphics = Graphics.FromImage(image);
                graphics.Clear(Color.White);
                Matrix matrix = new Matrix(1f, 0f, 0f, -1f, 0f, 0f);
                matrix.Translate(0f, this.CurrentBitmap.Height, MatrixOrder.Append);
                graphics.Transform = matrix;
                Matrix matrix2 = new Matrix();
                matrix2.RotateAt(-angle, new Point(0, 0), MatrixOrder.Append);
                matrix2.Translate(0f, 0f, MatrixOrder.Append);
                GraphicsPath path = new GraphicsPath();
                Point[] points = new Point[] { new Point(0, this.CurrentBitmap.Height), new Point(this.CurrentBitmap.Width, this.CurrentBitmap.Height), new Point(0, 0) };
                path.AddPolygon(points);
                path.Transform(matrix2);
                PointF[] pathPoints = path.PathPoints;
                graphics.DrawImage(this.CurrentBitmap, pathPoints);
                graphics.ResetTransform();
                int num4 = this.CurrentBitmap.Height;
                this._oCurrentImage = new Bitmap(width, height);
                this.CurrentBitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                using (Graphics newGraphics = Graphics.FromImage(this.CurrentBitmap))
                {
                    newGraphics.Clear(Color.White);
                    newGraphics.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(0, Convert.ToInt32(num4 * (1.0 - Math.Cos(a))), width, height), GraphicsUnit.Pixel);
                    image.Dispose();
                    graphics.Dispose();
                    path.Dispose();
                }
            }
        }
        /**/
        /// <summary>
        /// 画弧形
        /// </summary>
        /// <param name="r"></param>
        /// <param name="linewidth"></param>
        /// <param name="linecolor"></param>
        /// <param name="startangle"></param>
        /// <param name="sweepAngle"></param>
        public void DrawArc(Rectangle r, int linewidth, Color linecolor, float startangle, float sweepAngle)
        {
            using (Graphics graphics = Graphics.FromImage(this.CurrentBitmap))
            {
                Pen pen = new Pen(linecolor, linewidth);
                graphics.DrawArc(pen, r, startangle, sweepAngle);
            }
        }
        /**/
        /// <summary>
        /// 画椭圆
        /// </summary>
        /// <param name="r"></param>
        /// <param name="linewidth"></param>
        /// <param name="linecolor"></param>
        /// <param name="fillcolor"></param>
        public void DrawEllipse(Rectangle r, int linewidth, Color linecolor, Color fillcolor)
        {
            using (Graphics graphics = Graphics.FromImage(this.CurrentBitmap))
            {
                Pen pen = new Pen(linecolor, linewidth);
                graphics.DrawEllipse(pen, r);
                if (fillcolor != Color.Empty)
                {
                    graphics.FillEllipse(new SolidBrush(fillcolor), r);
                }
            }
        }
        /**/
        /// <summary>
        /// 画椭圆
        /// </summary>
        /// <param name="lefttop"></param>
        /// <param name="rightbottom"></param>
        /// <param name="linewidth"></param>
        /// <param name="linecolor"></param>
        /// <param name="fillcolor"></param>
        public void DrawEllipse(Point lefttop, Point rightbottom, int linewidth, Color linecolor, Color fillcolor)
        {
            this.DrawEllipse(new Rectangle(lefttop.X, lefttop.Y, rightbottom.X - lefttop.X, rightbottom.Y - lefttop.Y), linewidth, linecolor, fillcolor);
        }
        /**/
        /// <summary>
        /// 画椭圆
        /// </summary>
        /// <param name="lefttopx"></param>
        /// <param name="lefttopy"></param>
        /// <param name="rightbottomx"></param>
        /// <param name="rightbottomy"></param>
        /// <param name="linewidth"></param>
        /// <param name="linecolor"></param>
        /// <param name="fillcolor"></param>
        public void DrawEllipse(int lefttopx, int lefttopy, int rightbottomx, int rightbottomy, int linewidth, Color linecolor, Color fillcolor)
        {
            this.DrawEllipse(new Rectangle(lefttopx, lefttopy, rightbottomx - lefttopx, rightbottomy - lefttopy), linewidth, linecolor, fillcolor);
        }
        /**/
        /// <summary>
        /// 画图上图
        /// </summary>
        /// <param name="img"></param>
        /// <param name="r"></param>
        public void DrawImage(System.Drawing.Image img, Rectangle r)
        {
            Graphics.FromImage(this.CurrentBitmap).DrawImage(img, r);
        }
        /**/
        /// <summary>
        /// 画线
        /// </summary>
        /// <param name="pFrom"></param>
        /// <param name="pTo"></param>
        /// <param name="linewidth"></param>
        /// <param name="linecolor"></param>
        public void DrawLine(Point pFrom, Point pTo, int linewidth, Color linecolor)
        {
            using (Graphics graphics = Graphics.FromImage(this.CurrentBitmap))
            {
                Pen pen = new Pen(linecolor, linewidth);
                graphics.DrawLine(pen, pFrom, pTo);
            }
        }
        /**/
        /// <summary>
        /// 画线
        /// </summary>
        /// <param name="fromx"></param>
        /// <param name="fromy"></param>
        /// <param name="tox"></param>
        /// <param name="toy"></param>
        /// <param name="linewidth"></param>
        /// <param name="linecolor"></param>
        public void DrawLine(int fromx, int fromy, int tox, int toy, int linewidth, Color linecolor)
        {
            this.DrawLine(new Point(fromx, fromy), new Point(tox, toy), linewidth, linecolor);
        }
        /**/
        /// <summary>
        /// 画多条线
        /// </summary>
        /// <param name="points"></param>
        /// <param name="linewidth"></param>
        /// <param name="linecolor"></param>
        public void DrawLines(Point[] points, int linewidth, Color linecolor)
        {
            using (Graphics graphics = Graphics.FromImage(this.CurrentBitmap))
            {
                Pen pen = new Pen(linecolor, linewidth);
                graphics.DrawLines(pen, points);
            }
        }
        /**/
        /// <summary>
        /// 画长方形
        /// </summary>
        /// <param name="r"></param>
        /// <param name="linewidth"></param>
        /// <param name="linecolor"></param>
        /// <param name="fillcolor"></param>
        public void DrawRectangle(Rectangle r, int linewidth, Color linecolor, Color fillcolor)
        {
            using (Graphics graphics = Graphics.FromImage(this.CurrentBitmap))
            {
                Pen pen = new Pen(linecolor, linewidth);
                graphics.DrawRectangle(pen, r);
                if (fillcolor != Color.Empty)
                {
                    graphics.FillRectangle(new SolidBrush(fillcolor), r);
                }
            }
        }
        /**/
        /// <summary>
        /// 画长方形
        /// </summary>
        /// <param name="lefttop"></param>
        /// <param name="rightbottom"></param>
        /// <param name="linewidth"></param>
        /// <param name="linecolor"></param>
        /// <param name="fillcolor"></param>
        public void DrawRectangle(Point lefttop, Point rightbottom, int linewidth, Color linecolor, Color fillcolor)
        {
            this.DrawRectangle(new Rectangle(lefttop.X, lefttop.Y, rightbottom.X - lefttop.X, rightbottom.Y - lefttop.Y), linewidth, linecolor, fillcolor);
        }
        /**/
        /// <summary>
        /// 画长方形
        /// </summary>
        /// <param name="lefttopx"></param>
        /// <param name="lefttopy"></param>
        /// <param name="rightbottomx"></param>
        /// <param name="rightbottomy"></param>
        /// <param name="linewidth"></param>
        /// <param name="linecolor"></param>
        /// <param name="fillcolor"></param>
        public void DrawRectangle(int lefttopx, int lefttopy, int rightbottomx, int rightbottomy, int linewidth, Color linecolor, Color fillcolor)
        {
            this.DrawRectangle(GetRectangle(lefttopx, lefttopy, rightbottomx, rightbottomy), linewidth, linecolor, fillcolor);
        }
        /**/
        /// <summary>
        /// 保存图片到内存中
        /// </summary>
        /// <param name="mms"></param>
        /// <param name="RawFormat"></param>
        public void Save(MemoryStream mms, ImageFormat RawFormat)
        {
            this.CurrentBitmap.Save(mms, RawFormat);
        }
        /**/
        /// <summary>
        /// 保存图片到文件流中
        /// </summary>
        /// <param name="mms"></param>
        /// <param name="RawFormat"></param>
        public void Save(Stream mms, ImageFormat RawFormat)
        {
            this.CurrentBitmap.Save(mms, RawFormat);
        }
        /**/
        /// <summary>
        /// 保存为Gif图片
        /// </summary>
        /// <param name="sSavePath"></param>
        public void SaveGif(string sSavePath)
        {
            this.CurrentBitmap.Save(sSavePath, ImageFormat.Gif);
        }
        /**/
        /// <summary>
        /// 保存为jpg图片
        /// </summary>
        /// <param name="sSavePath"></param>
        /// <param name="iQuantity">图片质量</param>
        public void SaveJpeg(string sSavePath, int iQuantity)
        {
            ImageCodecInfo encoderInfo = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters();
            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, iQuantity);
            this.CurrentBitmap.Save(sSavePath, encoderInfo, encoderParams);
            encoderParams.Dispose();
        }
        /**/

        /// <summary>
        /// 保存为jpg图片
        /// </summary>
        /// <param name="etStream"></param>
        /// <param name="iQuantity">图片质量</param>
        public void SaveJpeg(Stream etStream, int iQuantity)
        {
            ImageCodecInfo encoderInfo = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters();
            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, iQuantity);
            this.CurrentBitmap.Save(etStream, encoderInfo, encoderParams);
            encoderParams.Dispose();
        }

        /**/
        /// <summary>
        /// 保存为jpg图片
        /// </summary>
        /// <param name="sSavePath"></param>
        public void SaveJpeg(string sSavePath)
        {
            this.CurrentBitmap.Save(sSavePath, ImageFormat.Jpeg);
        }
        /**/
        /// <summary>
        /// 保存为png图片
        /// </summary>
        /// <param name="sSavePath"></param>
        public void SavePNG(string sSavePath)
        {
            this.CurrentBitmap.Save(sSavePath, ImageFormat.Png);
        }
        /**/
        /// <summary>
        /// 保存当前图片对象，自动根据后缀名，保存图片。
        /// </summary>
        /// <param name="sSavePath"></param>
        public void Save(string sSavePath)
        {
            if (sSavePath.EndsWith(".jpg"))
            {
                this.SaveJpeg(sSavePath, 80);
            }
            else if (sSavePath.EndsWith(".gif"))
            {
                this.SaveGif(sSavePath);
            }
            else if (sSavePath.EndsWith(".png"))
            {
                this.SavePNG(sSavePath);
            }
            else
            {
                this.CurrentBitmap.Save(sSavePath);
            }
        }
        /**/
        /// <summary>
        /// 复制图片对象
        /// </summary>
        /// <param name="lefttopx"></param>
        /// <param name="lefttopy"></param>
        /// <param name="rightbottomx"></param>
        /// <param name="rightbottomy"></param>
        /// <returns></returns>
        public Bitmap Copy(int lefttopx, int lefttopy, int rightbottomx, int rightbottomy)
        {
            Rectangle srcRect = GetRectangle(lefttopx, lefttopy, rightbottomx, rightbottomy);
            using (Bitmap image = new Bitmap(srcRect.Width, srcRect.Height))
            {
                image.SetResolution(this._oCurrentImage.HorizontalResolution, this._oCurrentImage.VerticalResolution);
                Graphics.FromImage(image).DrawImage(this._oCurrentImage, new Rectangle(1, 1, image.Width, image.Height), srcRect, GraphicsUnit.Pixel);
                return (Bitmap)image.Clone();
            }
        }
        /**/
        /// <summary>
        /// 获取校正后的长方形
        /// </summary>
        /// <param name="lefttopx"></param>
        /// <param name="lefttopy"></param>
        /// <param name="rightbottomx"></param>
        /// <param name="rightbottomy"></param>
        /// <returns></returns>
        public static Rectangle GetRectangle(int lefttopx, int lefttopy, int rightbottomx, int rightbottomy)
        {
            if ((lefttopx >= rightbottomx) && (lefttopy >= rightbottomy))
            {
                return new Rectangle(rightbottomx, rightbottomy, lefttopx - rightbottomx, lefttopy - rightbottomy);
            }
            if ((lefttopx < rightbottomx) && (lefttopy >= rightbottomy))
            {
                return new Rectangle(lefttopx, rightbottomy, rightbottomx - lefttopx, lefttopy - rightbottomy);
            }
            if ((lefttopx >= rightbottomx) && (lefttopy < rightbottomy))
            {
                return new Rectangle(rightbottomx, lefttopy, lefttopx - rightbottomx, rightbottomy - lefttopy);
            }
            return new Rectangle(lefttopx, lefttopy, rightbottomx - lefttopx, rightbottomy - lefttopy);
        }
        /**/
        /// <summary>
        /// 获取编码信息
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            return ImageCodecInfo.GetImageEncoders().FirstOrDefault(info => info.MimeType == mimeType);
        }

        /**/
        /// <summary>
        /// 清空对象
        /// </summary>
        public void Dispose()
        {
            if (_oCurrentImage != null)
                _oCurrentImage.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}