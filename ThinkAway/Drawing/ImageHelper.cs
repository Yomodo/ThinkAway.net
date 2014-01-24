using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;

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
    public class ImageHelper : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        private Image _image;
        /// <summary>
        /// 
        /// </summary>
        public Image Image
        {
            get { return _image; }
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly Graphics _graphics;

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
        

        /// <summary>
        /// 使用指定的 宽度 , 高度 创建位图
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public ImageHelper(int width, int height)
            : this(new Bitmap(width, height))
        {
        }
        /// <summary>
        /// 使用指定的 Image 创建 ImageHelper 实例
        /// </summary>
        /// <param name="image"></param>
        public ImageHelper(Image image)
        {
            _image = image;
            _graphics = Graphics.FromImage(_image);
        }
        
        /// <summary>
        /// 实例化Bitmap对象
        /// </summary>
        /// <param name="fileName">图片路径</param>
        public ImageHelper(string fileName)
        {
            Image image;
            if (fileName.StartsWith("http://") || fileName.StartsWith("https://"))
            {
                HttpWebRequest request = System.Net.WebRequest.Create(fileName) as HttpWebRequest;
                WebResponse webResponse = request.GetResponse();
                Stream responseStream = webResponse.GetResponseStream();
                image = Image.FromStream(responseStream);
            }
            else
            {
                image = new Bitmap(fileName);
            }
            //如果原图片是索引像素格式之列的，则需要转换
            if (IsPixelFormatIndexed(image.PixelFormat))
            {
                _image = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
                _graphics = Graphics.FromImage(_image);
                _graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                _graphics.SmoothingMode = SmoothingMode.HighQuality;
                _graphics.CompositingQuality = CompositingQuality.HighQuality;
                _graphics.DrawImage(image, 0, 0);
            }
            else
            {
                _image = (Bitmap)image.Clone();
                _graphics = Graphics.FromImage(_image);
            }
        }

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

        /// <summary>
        /// 添加文字水印
        /// </summary>
        /// <param name="point"></param>
        /// <param name="fontColor"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="rotate"></param>
        public void DrawText(string text, Font font, Color fontColor, Point point, float rotate)
        {
            SolidBrush solidBrush = new SolidBrush(fontColor);
            DrawText(text, font, fontColor, point, rotate, solidBrush);
        }

        /// <summary>
        /// 使用 Image 刷子绘制文字
        /// </summary>
        /// <param name="text"> </param>
        /// <param name="font"></param>
        /// <param name="point"> </param>
        /// <param name="rotate"></param>
        /// <param name="fontColor"> </param>
        /// <param name="image"> </param>
        public void DrawText(string text, Font font, Color fontColor, Point point, float rotate, Image image)
        {
            TextureBrush textureBrush = new TextureBrush(image);
            DrawText(text, font, fontColor, point, rotate, textureBrush);
        }

        /// <summary>
        /// 使用刷子绘制文字
        /// </summary>
        /// <param name="text"> </param>
        /// <param name="font"></param>
        /// <param name="point"> </param>
        /// <param name="rotate"></param>
        /// <param name="fontColor"> </param>
        /// <param name="brush"> </param>
        protected void DrawText(string text, Font font, Color fontColor, Point point, float rotate, Brush brush)
        {
            if (Math.Abs(rotate - 0) > 0)
                _graphics.RotateTransform(rotate);
            _graphics.SmoothingMode = SmoothingMode.HighQuality;
            _graphics.DrawString(text, font, brush, point.X, point.Y);
        }

        /// <summary>
        /// 绘制边框
        /// </summary>
        /// <param name="frameColor">边框颜色</param>
        /// <param name="frameWidth">边框宽度</param>
        public void DrawFrame(Color frameColor, int frameWidth)
        {
            double iNewframeWidth = (Convert.ToDouble(frameWidth) / Convert.ToDouble(2));
            using (Bitmap newBitmap = new Bitmap(_image.Width + (2 * frameWidth), _image.Height + (2 * frameWidth)))
            {
                using (Graphics graphics = Graphics.FromImage(newBitmap))
                {
                    graphics.Clear(Color.White);
                    Pen pen = new Pen(frameColor, frameWidth);
                    graphics.DrawRectangle(pen, (float)iNewframeWidth, (float)iNewframeWidth, newBitmap.Width - frameWidth, newBitmap.Height - frameWidth);
                    graphics.DrawImage(this._image, new Rectangle(frameWidth, frameWidth, this._image.Width, this._image.Height));
                    this._image = ((Bitmap)newBitmap.Clone());
                }
            }
        }


        /// <summary>
        /// 绘制3D边框
        /// </summary>
        /// <param name="frameColor">边框颜色</param>
        /// <param name="frameWidth">边框宽度</param>
        public void Draw3DFrame(Color frameColor, int frameWidth)
        {
            double iNewframeWidth = (Convert.ToDouble(frameWidth) / Convert.ToDouble(2));
            using (Bitmap newBitmap = new Bitmap(this._image.Width + (2 * frameWidth), this._image.Height + (2 * frameWidth)))
            {
                using (Graphics graphics = Graphics.FromImage(newBitmap))
                {
                    graphics.Clear(Color.White);
                    Pen pen = new Pen(frameColor, (float)iNewframeWidth);
                    Pen pen1 = new Pen(Color.FromArgb(frameColor.ToArgb() - 10), (float)iNewframeWidth);
                    graphics.DrawRectangle(pen, (float)iNewframeWidth / 2, (float)iNewframeWidth / 2, newBitmap.Width - (float)iNewframeWidth, newBitmap.Height - (float)iNewframeWidth);
                    graphics.DrawRectangle(pen1, 3 * (float)iNewframeWidth / 2, 3 * (float)iNewframeWidth / 2, newBitmap.Width - 3 * (float)iNewframeWidth, newBitmap.Height - 3 * (float)iNewframeWidth);
                    graphics.DrawImage(this._image, 2 * (float)iNewframeWidth, 2 * (float)iNewframeWidth, this._image.Width, this._image.Height);
                    this._image = (Bitmap)newBitmap.Clone();
                }
            }
        }

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
            if ((this._image.Width - left) < right)
            {
                num = this._image.Width - right;
            }
            else
            {
                num = right;
            }
            if ((this._image.Height - top) < bottom)
            {
                num2 = this._image.Height - top;
            }
            else
            {
                num2 = bottom;
            }
            using (Bitmap image = new Bitmap(num, num2))
            {
                image.SetResolution(this._image.HorizontalResolution, this._image.VerticalResolution);
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    graphics.Clear(Color.White);
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.DrawImage(this._image, new Rectangle(0, 0, num, num2), new Rectangle(left, top, num, num2), GraphicsUnit.Pixel);
                    var bitmap = (Bitmap) image.Clone();
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
                using (Graphics graphics = Graphics.FromImage(this._image))
                {
                    graphics.Clear(Color.White);
                    graphics.FillRectangle(new SolidBrush(Color.White), rect);
                    graphics.DrawImage(image, tox, toy);
                    //image = (Bitmap)image.Clone();
                }
            }
        }
        /**/
        /// <summary>
        /// 调整图片大小
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Resize(int width, int height)
        {
            _image = CreateThumbnail(width, height);
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        public Bitmap CreateThumbnail(int width, int height)
        {
            return new Bitmap(_image, width, height);
        }
        
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
            using (Graphics graphics = Graphics.FromImage(this._image))
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
            using (Graphics graphics = Graphics.FromImage(this._image))
            {
                Pen pen = new Pen(linecolor, linewidth);
                graphics.DrawEllipse(pen, r);
                if (fillcolor != Color.Empty)
                {
                    graphics.FillEllipse(new SolidBrush(fillcolor), r);
                }
            }
        }

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

        /// <summary>
        /// 画图上图
        /// </summary>
        /// <param name="img"></param>
        /// <param name="rectangle"></param>
        public void DrawImage(System.Drawing.Image img, Rectangle rectangle)
        {
            _graphics.DrawImage(img, rectangle);
        }

        /// <summary>
        /// 画线
        /// </summary>
        /// <param name="pFrom"></param>
        /// <param name="pTo"></param>
        /// <param name="linewidth"></param>
        /// <param name="linecolor"></param>
        public void DrawLine(Point pFrom, Point pTo, int linewidth, Color linecolor)
        {
            Pen pen = new Pen(linecolor, linewidth);
            _graphics.DrawLine(pen, pFrom, pTo);
        }

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

        /// <summary>
        /// 画多条线
        /// </summary>
        /// <param name="points"></param>
        /// <param name="linewidth"></param>
        /// <param name="linecolor"></param>
        public void DrawLines(Point[] points, int linewidth, Color linecolor)
        {
            Pen pen = new Pen(linecolor, linewidth);
            _graphics.DrawLines(pen, points);
        }

        /// <summary>
        /// 画长方形
        /// </summary>
        /// <param name="r"></param>
        /// <param name="linewidth"></param>
        /// <param name="linecolor"></param>
        /// <param name="fillcolor"></param>
        public void DrawRectangle(Rectangle r, int linewidth, Color linecolor, Color fillcolor)
        {
            Pen pen = new Pen(linecolor, linewidth);
            _graphics.DrawRectangle(pen, r);
            if (fillcolor != Color.Empty)
            {
                _graphics.FillRectangle(new SolidBrush(fillcolor), r);
            }
        }


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


        
        /// <summary>
        /// 获取校正后的长方形
        /// </summary>
        /// <param name="lefttopx"></param>
        /// <param name="lefttopy"></param>
        /// <param name="rightbottomx"></param>
        /// <param name="rightbottomy"></param>
        /// <returns></returns>
        public Rectangle GetRectangle(int lefttopx, int lefttopy, int rightbottomx, int rightbottomy)
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
                image.SetResolution(this._image.HorizontalResolution, this._image.VerticalResolution);
                _graphics.DrawImage(this._image, new Rectangle(1, 1, image.Width, image.Height), srcRect, GraphicsUnit.Pixel);
                return (Bitmap)image.Clone();
            }
        }

        /// <summary>
        /// 图片垂直翻转
        /// </summary>
        public void FlipV()
        {
            this._image.RotateFlip(RotateFlipType.RotateNoneFlipX);
        }

        /// <summary>
        /// 图片水平翻转
        /// </summary>
        public void FlipH()
        {
            this._image.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }
        ///**/
        ///// <summary>
        ///// 图片旋转
        ///// </summary>
        ///// <param name="angle">旋转度数</param>
        //public void Rotate(int angle)
        //{
        //    angle = angle % 360;
        //    if (angle < 0)
        //    {
        //        angle += 360;
        //    }
        //    if (angle >= 270)
        //    {
        //        this._image.RotateFlip(RotateFlipType.Rotate270FlipNone);
        //        angle -= 270;
        //    }
        //    else if (angle >= 180)
        //    {
        //        this._image.RotateFlip(RotateFlipType.Rotate180FlipNone);
        //        angle -= 180;
        //    }
        //    else if (angle >= 90)
        //    {
        //        this._image.RotateFlip(RotateFlipType.Rotate90FlipNone);
        //        angle -= 90;
        //    }
        //    if (angle > 0)
        //    {
        //        double a = (angle * 3.1415926535897931) / 180.0;
        //        int width = Convert.ToInt32((this._image.Height * Math.Sin(a)) + (this._image.Width * Math.Cos(a)));
        //        int height = Convert.ToInt32((this._image.Width * Math.Sin(a)) + (this._image.Height * Math.Cos(a)));
        //        Bitmap image = new Bitmap(width, height * 2);
        //        image.SetResolution(this._image.HorizontalResolution, this._image.VerticalResolution);
        //        Graphics graphics = Graphics.FromImage(image);
        //        graphics.Clear(Color.White);
        //        Matrix matrix = new Matrix(1f, 0f, 0f, -1f, 0f, 0f);
        //        matrix.Translate(0f, this._image.Height, MatrixOrder.Append);
        //        graphics.Transform = matrix;
        //        Matrix matrix2 = new Matrix();
        //        matrix2.RotateAt(-angle, new Point(0, 0), MatrixOrder.Append);
        //        matrix2.Translate(0f, 0f, MatrixOrder.Append);
        //        GraphicsPath path = new GraphicsPath();
        //        Point[] points = new Point[] { new Point(0, this._image.Height), new Point(this._image.Width, this._image.Height), new Point(0, 0) };
        //        path.AddPolygon(points);
        //        path.Transform(matrix2);
        //        PointF[] pathPoints = path.PathPoints;
        //        graphics.DrawImage(this._image, pathPoints);
        //        graphics.ResetTransform();
        //        int num4 = this._image.Height;
        //        this._image = new Bitmap(width, height);
        //        this._image.SetResolution(image.HorizontalResolution, image.VerticalResolution);
        //        using (Graphics newGraphics = Graphics.FromImage(this._image))
        //        {
        //            newGraphics.Clear(Color.White);
        //            newGraphics.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(0, Convert.ToInt32(num4 * (1.0 - Math.Cos(a))), width, height), GraphicsUnit.Pixel);
        //            image.Dispose();
        //            graphics.Dispose();
        //            path.Dispose();
        //        }
        //    }
        //}
        /**/

        /// <summary>
        /// 获取编码信息
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] imageCodecInfos = ImageCodecInfo.GetImageEncoders();
            return imageCodecInfos.FirstOrDefault(info => info.MimeType == mimeType);
        }
       

        /// <summary>
        /// 保存当前图片对象，自动根据后缀名，保存图片。
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (extension != null)
                switch (extension.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                        Save(fileName, ImageFormat.Jpeg);
                        break;
                    case ".gif":
                        Save(fileName,ImageFormat.Gif);
                        break;
                    case ".png":
                        Save(fileName, ImageFormat.Png);
                        break;
                    case ".bmp":
                        Save(fileName, ImageFormat.Bmp);
                        break;
                    default:
                        this._image.Save(fileName);
                        break;
                }
        }

        /// <summary>
        /// 保存图片到文件流中
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="rawFormat"></param>
        public void Save(Stream stream, ImageFormat rawFormat)
        {
            this._image.Save(stream, rawFormat);
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="fileName"> </param>
        /// <param name="format"> </param>
        public void Save(string fileName, ImageFormat format)
        {
            this._image.Save(fileName, format);
        }


        /// <summary>
        /// 保存为jpg图片
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="quantity">图片质量</param>
        public void SaveJpeg(string fileName, int quantity)
        {
            ImageCodecInfo encoderInfo = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters();
            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quantity);
            this._image.Save(fileName, encoderInfo, encoderParams);
            encoderParams.Dispose();
        }

        /// <summary>
        /// 保存为jpg图片
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="quantity">图片质量</param>
        public void SaveJpeg(Stream stream, int quantity)
        {
            ImageCodecInfo encoderInfo = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters();
            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quantity);
            this._image.Save(stream, encoderInfo, encoderParams);
            encoderParams.Dispose();
        }
        
        
        /// <summary>
        /// 释放对象
        /// </summary>
        public void Dispose()
        {
            if (_image != null)
                _image.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}