using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing.Text;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Controls
{

    public class SplitContainer : System.Windows.Forms.SplitContainer
    {
        private CollapsePanel _collapsePanel = CollapsePanel.Panel1;
        private SpliterPanelState _spliterPanelState = SpliterPanelState.Expanded;
        private SplitContainer.ControlPaintEx.ControlState _mouseState;
        private int _lastDistance;
        private int _minSize;
        private HistTest _histTest;
        private readonly object _eventCollapseClick = new object();

        public SplitContainer()
        {
            base.SetStyle(
                ControlStyles.UserPaint | 
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            _lastDistance = base.SplitterDistance;
        }

        public event EventHandler CollapseClick
        {
            add { base.Events.AddHandler(_eventCollapseClick, value); }
            remove { base.Events.RemoveHandler(_eventCollapseClick, value); }
        }

        [DefaultValue(typeof(CollapsePanel), "1")]
        public CollapsePanel CollapsePanels
        {
            get { return _collapsePanel; }
            set
            {
                if (_collapsePanel != value)
                {
                    Expand();
                    _collapsePanel = value;
                }
            }
        }

        protected virtual int DefaultCollapseWidth
        {
            get { return 80; }
        }

        protected virtual int DefaultArrowWidth
        {
            get { return 16; }
        }

        protected Rectangle CollapseRect
        {
            get
            {
                if (_collapsePanel == CollapsePanel.None)
                {
                    return Rectangle.Empty;
                }

                Rectangle rect = base.SplitterRectangle;
                if (base.Orientation == Orientation.Horizontal)
                {
                    rect.X = (base.Width - DefaultCollapseWidth) / 2;
                    rect.Width = DefaultCollapseWidth;
                }
                else
                {
                    rect.Y = (base.Height - DefaultCollapseWidth) / 2;
                    rect.Height = DefaultCollapseWidth;
                }

                return rect;
            }
        }

        internal SpliterPanelState SpliterPanelStates
        {
            get { return _spliterPanelState; }
            set
            {
                if (_spliterPanelState != value)
                {
                    switch (value)
                    {
                        case SpliterPanelState.Expanded:
                            Expand();
                            break;
                        case SpliterPanelState.Collapsed:
                            Collapse();
                            break;

                    }
                    _spliterPanelState = value;
                }
            }
        }

        internal SplitContainer.ControlPaintEx.ControlState MouseState
        {
            get { return _mouseState; }
            set
            {
                if (_mouseState != value)
                {
                    _mouseState = value;
                    base.Invalidate(CollapseRect);
                }
            }
        }

        public void Collapse()
        {
            if (_collapsePanel != CollapsePanel.None &&
                _spliterPanelState == SpliterPanelState.Expanded)
            {
                _lastDistance = base.SplitterDistance;
                if (_collapsePanel == CollapsePanel.Panel1)
                {
                    _minSize = base.Panel1MinSize;
                    base.Panel1MinSize = 0;
                    base.SplitterDistance = 0;
                }
                else
                {
                    int width = base.Orientation == Orientation.Horizontal ?
                        base.Height : base.Width;
                    _minSize = base.Panel2MinSize;
                    base.Panel2MinSize = 0;
                    base.SplitterDistance = width - base.SplitterWidth- base.Padding.Vertical;
                }
                base.Invalidate(base.SplitterRectangle);
            }
        }

        public void Expand()
        {
            if (_collapsePanel != CollapsePanel.None &&
               _spliterPanelState == SpliterPanelState.Collapsed)
            {
                if (_collapsePanel == CollapsePanel.Panel1)
                {
                    base.Panel1MinSize = _minSize;
                }
                else
                {
                    base.Panel2MinSize = _minSize;
                }
                base.SplitterDistance = _lastDistance;
                base.Invalidate(base.SplitterRectangle);
            }
        }

        protected virtual void OnCollapseClick(EventArgs e)
        {
            SpliterPanelStates = _spliterPanelState == SpliterPanelState.Collapsed ? SpliterPanelState.Expanded : SpliterPanelState.Collapsed;

            EventHandler handler = base.Events[_eventCollapseClick] as EventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Panel1Collapsed || base.Panel2Collapsed)
            {
                return;
            }

            Graphics g = e.Graphics;
            Rectangle rect = base.SplitterRectangle;
            bool bHorizontal = base.Orientation == Orientation.Horizontal;

            LinearGradientMode gradientMode = bHorizontal ? 
                LinearGradientMode.Vertical : LinearGradientMode.Horizontal;

            using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(206, 238, 255), Color.FromArgb(105, 200, 254), gradientMode))
            {
                Blend blend = new Blend();
                blend.Positions = new float[] { 0f, .5f, 1f };
                blend.Factors = new float[] { .5F, 1F, .5F };

                brush.Blend = blend;
                g.FillRectangle(brush, rect);
            }

            if (_collapsePanel == CollapsePanel.None)
            {
                return;
            }

            Rectangle arrowRect;
            Rectangle topLeftRect;
            Rectangle bottomRightRect;

            CalculateRect(
                CollapseRect,
                out arrowRect,
                out topLeftRect,
                out bottomRightRect);

            ArrowDirection direction = ArrowDirection.Left;

            switch (_collapsePanel)
            {
                case CollapsePanel.Panel1:
                    if (bHorizontal)
                    {
                        direction =
                            _spliterPanelState == SpliterPanelState.Collapsed ?
                            ArrowDirection.Down : ArrowDirection.Up;
                    }
                    else
                    {
                        direction =
                            _spliterPanelState == SpliterPanelState.Collapsed ?
                            ArrowDirection.Right : ArrowDirection.Left;
                    }
                    break;
                case CollapsePanel.Panel2:
                    if (bHorizontal)
                    {
                        direction =
                            _spliterPanelState == SpliterPanelState.Collapsed ?
                            ArrowDirection.Up : ArrowDirection.Down;
                    }
                    else
                    {
                        direction =
                            _spliterPanelState == SpliterPanelState.Collapsed ?
                            ArrowDirection.Left : ArrowDirection.Right;
                    }
                    break;
            }

            Color foreColor = _mouseState == SplitContainer.ControlPaintEx.ControlState.Hover ?
                Color.FromArgb(21, 66, 139) : Color.FromArgb(80, 136, 228);
          
                RenderHelper.RenderGrid(g, topLeftRect, new Size(3, 3), foreColor);
                RenderHelper.RenderGrid(g, bottomRightRect, new Size(3, 3), foreColor);

                using (Brush brush = new SolidBrush(foreColor))
                {
                    RenderHelper.RenderArrowInternal(
                        g,
                        arrowRect,
                        direction,
                        brush);
                }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //如果鼠标的左键没有按下，重置HistTest
            if (e.Button != MouseButtons.Left)
            {
                _histTest = HistTest.None;
            }

            Rectangle collapseRect = CollapseRect;
            Point mousePoint = e.Location;

            //鼠标在Button矩形里，并且不是在拖动
            if (collapseRect.Contains(mousePoint) &&
                _histTest != HistTest.Spliter)
            {
                base.Capture = false;
                SetCursor(Cursors.Hand);
                MouseState = SplitContainer.ControlPaintEx.ControlState.Hover;
                return;
            }//鼠标在分隔栏矩形里
            if (base.SplitterRectangle.Contains(mousePoint))
            {
                MouseState = SplitContainer.ControlPaintEx.ControlState.Normal;

                //如果已经在按钮按下了鼠标或者已经收缩，就不允许拖动了
                if (_histTest == HistTest.Button ||
                    (_collapsePanel != CollapsePanel.None &&
                     _spliterPanelState == SpliterPanelState.Collapsed))
                {
                    base.Capture = false;
                    base.Cursor = Cursors.Default;
                    return;
                }

                //鼠标没有按下，设置Split光标
                if (_histTest == HistTest.None &&
                    !base.IsSplitterFixed)
                {
                    SetCursor(base.Orientation == Orientation.Horizontal ? Cursors.HSplit : Cursors.VSplit);
                    return;
                }
            }

            MouseState = SplitContainer.ControlPaintEx.ControlState.Normal;

            //正在拖动分隔栏
            if (_histTest == HistTest.Spliter &&
                !base.IsSplitterFixed)
            {
                SetCursor(base.Orientation == Orientation.Horizontal ? Cursors.HSplit : Cursors.VSplit);
                base.OnMouseMove(e);
                return;
            }

            base.Cursor = Cursors.Default;
            base.OnMouseMove(e);
        }
        public static class GraphicsPathHelper
        {
            /// <summary>
            /// 建立带有圆角样式的路径。
            /// </summary>
            /// <param name="rect">用来建立路径的矩形。</param>
            /// <param name="radius"> </param>
            /// <param name="style">圆角的样式。</param>
            /// <param name="correction">是否把矩形长宽减 1,以便画出边框。</param>
            /// <returns>建立的路径。</returns>
            public static GraphicsPath CreatePath(
                Rectangle rect, int radius, RoundStyle style, bool correction)
            {
                GraphicsPath path = new GraphicsPath();
                int radiusCorrection = correction ? 1 : 0;
                switch (style)
                {
                    case RoundStyle.None:
                        path.AddRectangle(rect);
                        break;
                    case RoundStyle.All:
                        path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                        path.AddArc(
                            rect.Right - radius - radiusCorrection,
                            rect.Y,
                            radius,
                            radius,
                            270,
                            90);
                        path.AddArc(
                            rect.Right - radius - radiusCorrection,
                            rect.Bottom - radius - radiusCorrection,
                            radius,
                            radius, 0, 90);
                        path.AddArc(
                            rect.X,
                            rect.Bottom - radius - radiusCorrection,
                            radius,
                            radius,
                            90,
                            90);
                        break;
                    case RoundStyle.Left:
                        path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                        path.AddLine(
                            rect.Right - radiusCorrection, rect.Y,
                            rect.Right - radiusCorrection, rect.Bottom - radiusCorrection);
                        path.AddArc(
                            rect.X,
                            rect.Bottom - radius - radiusCorrection,
                            radius,
                            radius,
                            90,
                            90);
                        break;
                    case RoundStyle.Right:
                        path.AddArc(
                            rect.Right - radius - radiusCorrection,
                            rect.Y,
                            radius,
                            radius,
                            270,
                            90);
                        path.AddArc(
                           rect.Right - radius - radiusCorrection,
                           rect.Bottom - radius - radiusCorrection,
                           radius,
                           radius,
                           0,
                           90);
                        path.AddLine(rect.X, rect.Bottom - radiusCorrection, rect.X, rect.Y);
                        break;
                    case RoundStyle.Top:
                        path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                        path.AddArc(
                            rect.Right - radius - radiusCorrection,
                            rect.Y,
                            radius,
                            radius,
                            270,
                            90);
                        path.AddLine(
                            rect.Right - radiusCorrection, rect.Bottom - radiusCorrection,
                            rect.X, rect.Bottom - radiusCorrection);
                        break;
                    case RoundStyle.Bottom:
                        path.AddArc(
                            rect.Right - radius - radiusCorrection,
                            rect.Bottom - radius - radiusCorrection,
                            radius,
                            radius,
                            0,
                            90);
                        path.AddArc(
                            rect.X,
                            rect.Bottom - radius - radiusCorrection,
                            radius,
                            radius,
                            90,
                            90);
                        path.AddLine(rect.X, rect.Y, rect.Right - radiusCorrection, rect.Y);
                        break;
                    case RoundStyle.BottomLeft:
                        path.AddArc(
                            rect.X,
                            rect.Bottom - radius - radiusCorrection,
                            radius,
                            radius,
                            90,
                            90);
                        path.AddLine(rect.X, rect.Y, rect.Right - radiusCorrection, rect.Y);
                        path.AddLine(
                            rect.Right - radiusCorrection,
                            rect.Y,
                            rect.Right - radiusCorrection,
                            rect.Bottom - radiusCorrection);
                        break;
                    case RoundStyle.BottomRight:
                        path.AddArc(
                            rect.Right - radius - radiusCorrection,
                            rect.Bottom - radius - radiusCorrection,
                            radius,
                            radius,
                            0,
                            90);
                        path.AddLine(rect.X, rect.Bottom - radiusCorrection, rect.X, rect.Y);
                        path.AddLine(rect.X, rect.Y, rect.Right - radiusCorrection, rect.Y);
                        break;
                }
                path.CloseFigure();

                return path;
            }

            public static GraphicsPath CreateTrackBarThumbPath(
                Rectangle rect, ThumbArrowDirection arrowDirection)
            {
                GraphicsPath path = new GraphicsPath();
                PointF centerPoint = new PointF(
                    rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
                float offset = 0;

                switch (arrowDirection)
                {
                    case ThumbArrowDirection.Left:
                    case ThumbArrowDirection.Right:
                        offset = rect.Width / 2f - 4;
                        break;
                    case ThumbArrowDirection.Up:
                    case ThumbArrowDirection.Down:
                        offset = rect.Height / 2f - 4;
                        break;
                }

                switch (arrowDirection)
                {
                    case ThumbArrowDirection.Left:
                        path.AddLine(
                            rect.X, centerPoint.Y, rect.X + offset, rect.Y);
                        path.AddLine(
                            rect.Right, rect.Y, rect.Right, rect.Bottom);
                        path.AddLine(
                            rect.X + offset, rect.Bottom, rect.X, centerPoint.Y);
                        break;
                    case ThumbArrowDirection.Right:
                        path.AddLine(
                            rect.Right, centerPoint.Y, rect.Right - offset, rect.Bottom);
                        path.AddLine(
                            rect.X, rect.Bottom, rect.X, rect.Y);
                        path.AddLine(
                            rect.Right - offset, rect.Y, rect.Right, centerPoint.Y);
                        break;
                    case ThumbArrowDirection.Up:
                        path.AddLine(
                            centerPoint.X, rect.Y, rect.X, rect.Y + offset);
                        path.AddLine(
                            rect.X, rect.Bottom, rect.Right, rect.Bottom);
                        path.AddLine(
                            rect.Right, rect.Y + offset, centerPoint.X, rect.Y);
                        break;
                    case ThumbArrowDirection.Down:
                        path.AddLine(
                             centerPoint.X, rect.Bottom, rect.X, rect.Bottom - offset);
                        path.AddLine(
                            rect.X, rect.Y, rect.Right, rect.Y);
                        path.AddLine(
                            rect.Right, rect.Bottom - offset, centerPoint.X, rect.Bottom);
                        break;
                    case ThumbArrowDirection.LeftRight:
                        break;
                    case ThumbArrowDirection.UpDown:
                        break;
                    case ThumbArrowDirection.None:
                        path.AddRectangle(rect);
                        break;
                }

                path.CloseFigure();
                return path;
            }
        }
        public class InterpolationModeGraphics : IDisposable
        {
            private readonly InterpolationMode _oldMode;
            private readonly Graphics _graphics;

            public InterpolationModeGraphics(Graphics graphics)
                : this(graphics, InterpolationMode.HighQualityBicubic)
            {
            }

            public InterpolationModeGraphics(
                Graphics graphics, InterpolationMode newMode)
            {
                _graphics = graphics;
                _oldMode = graphics.InterpolationMode;
                graphics.InterpolationMode = newMode;
            }

            #region IDisposable 成员

            public void Dispose()
            {
                _graphics.InterpolationMode = _oldMode;
            }

            #endregion
        }
        public static class RegionHelper
        {
            public static void CreateRegion(
                System.Windows.Forms.Control control,
                Rectangle bounds,
                int radius,
                RoundStyle roundStyle)
            {
                using (GraphicsPath path =
                    SplitContainer.GraphicsPathHelper.CreatePath(
                    bounds, radius, roundStyle, true))
                {
                    Region region = new Region(path);
                    path.Widen(Pens.White);
                    region.Union(path);
                    if (control.Region != null)
                    {
                        control.Region.Dispose();
                    }
                    control.Region = region;
                }
            }

            public static void CreateRegion(
                System.Windows.Forms.Control control,
                Rectangle bounds)
            {
                CreateRegion(control, bounds, 8, RoundStyle.All);
            }
        }
        internal class RenderHelper
        {
            internal static void RenderBackgroundInternal(
                Graphics g,
                Rectangle rect,
                Color baseColor,
                Color borderColor,
                Color innerBorderColor,
                RoundStyle style,
                bool drawBorder,
                bool drawGlass,
                LinearGradientMode mode)
            {
                RenderBackgroundInternal(
                    g,
                    rect,
                    baseColor,
                    borderColor,
                    innerBorderColor,
                    style,
                    8,
                    drawBorder,
                    drawGlass,
                    mode);
            }

            internal static void RenderBackgroundInternal(
               Graphics g,
               Rectangle rect,
               Color baseColor,
               Color borderColor,
               Color innerBorderColor,
               RoundStyle style,
               int roundWidth,
               bool drawBorder,
               bool drawGlass,
               LinearGradientMode mode)
            {
                RenderBackgroundInternal(
                     g,
                     rect,
                     baseColor,
                     borderColor,
                     innerBorderColor,
                     style,
                     8,
                     0.45f,
                     drawBorder,
                     drawGlass,
                     mode);
            }

            internal static void RenderBackgroundInternal(
               Graphics g,
               Rectangle rect,
               Color baseColor,
               Color borderColor,
               Color innerBorderColor,
               RoundStyle style,
               int roundWidth,
               float basePosition,
               bool drawBorder,
               bool drawGlass,
               LinearGradientMode mode)
            {
                if (drawBorder)
                {
                    rect.Width--;
                    rect.Height--;
                }

                if (rect.Width == 0 || rect.Height == 0)
                {
                    return;
                }

                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect, Color.Transparent, Color.Transparent, mode))
                {
                    Color[] colors = new Color[4];
                    colors[0] = GetColor(baseColor, 0, 35, 24, 9);
                    colors[1] = GetColor(baseColor, 0, 13, 8, 3);
                    colors[2] = baseColor;
                    colors[3] = GetColor(baseColor, 0, 35, 24, 9);

                    ColorBlend blend = new ColorBlend();
                    blend.Positions = new float[] { 0.0f, basePosition, basePosition + 0.05f, 1.0f };
                    blend.Colors = colors;
                    brush.InterpolationColors = blend;
                    if (style != RoundStyle.None)
                    {
                        using (GraphicsPath path =
                            SplitContainer.GraphicsPathHelper.CreatePath(rect, roundWidth, style, false))
                        {
                            g.FillPath(brush, path);
                        }

                        if (baseColor.A > 80)
                        {
                            Rectangle rectTop = rect;

                            if (mode == LinearGradientMode.Vertical)
                            {
                                rectTop.Height = (int)(rectTop.Height * basePosition);
                            }
                            else
                            {
                                rectTop.Width = (int)(rect.Width * basePosition);
                            }
                            using (GraphicsPath pathTop = SplitContainer.GraphicsPathHelper.CreatePath(
                                rectTop, roundWidth, RoundStyle.Top, false))
                            {
                                using (SolidBrush brushAlpha =
                                    new SolidBrush(Color.FromArgb(128, 255, 255, 255)))
                                {
                                    g.FillPath(brushAlpha, pathTop);
                                }
                            }
                        }

                        if (drawGlass)
                        {
                            RectangleF glassRect = rect;
                            if (mode == LinearGradientMode.Vertical)
                            {
                                glassRect.Y = rect.Y + rect.Height * basePosition;
                                glassRect.Height = (rect.Height - rect.Height * basePosition) * 2;
                            }
                            else
                            {
                                glassRect.X = rect.X + rect.Width * basePosition;
                                glassRect.Width = (rect.Width - rect.Width * basePosition) * 2;
                            }
                            SplitContainer.ControlPaintEx.DrawGlass(g, glassRect, 170, 0);
                        }

                        if (drawBorder)
                        {
                            using (GraphicsPath path =
                                SplitContainer.GraphicsPathHelper.CreatePath(rect, roundWidth, style, false))
                            {
                                using (Pen pen = new Pen(borderColor))
                                {
                                    g.DrawPath(pen, path);
                                }
                            }

                            rect.Inflate(-1, -1);
                            using (GraphicsPath path =
                                SplitContainer.GraphicsPathHelper.CreatePath(rect, roundWidth, style, false))
                            {
                                using (Pen pen = new Pen(innerBorderColor))
                                {
                                    g.DrawPath(pen, path);
                                }
                            }
                        }
                    }
                    else
                    {
                        g.FillRectangle(brush, rect);
                        if (baseColor.A > 80)
                        {
                            Rectangle rectTop = rect;
                            if (mode == LinearGradientMode.Vertical)
                            {
                                rectTop.Height = (int)(rectTop.Height * basePosition);
                            }
                            else
                            {
                                rectTop.Width = (int)(rect.Width * basePosition);
                            }
                            using (SolidBrush brushAlpha =
                                new SolidBrush(Color.FromArgb(128, 255, 255, 255)))
                            {
                                g.FillRectangle(brushAlpha, rectTop);
                            }
                        }

                        if (drawGlass)
                        {
                            RectangleF glassRect = rect;
                            if (mode == LinearGradientMode.Vertical)
                            {
                                glassRect.Y = rect.Y + rect.Height * basePosition;
                                glassRect.Height = (rect.Height - rect.Height * basePosition) * 2;
                            }
                            else
                            {
                                glassRect.X = rect.X + rect.Width * basePosition;
                                glassRect.Width = (rect.Width - rect.Width * basePosition) * 2;
                            }
                            SplitContainer.ControlPaintEx.DrawGlass(g, glassRect, 200, 0);
                        }

                        if (drawBorder)
                        {
                            using (Pen pen = new Pen(borderColor))
                            {
                                g.DrawRectangle(pen, rect);
                            }

                            rect.Inflate(-1, -1);
                            using (Pen pen = new Pen(innerBorderColor))
                            {
                                g.DrawRectangle(pen, rect);
                            }
                        }
                    }
                }
            }

            internal static void RenderArrowInternal(
                Graphics g,
                Rectangle dropDownRect,
                ArrowDirection direction,
                Brush brush)
            {
                Point point = new Point(
                    dropDownRect.Left + (dropDownRect.Width / 2),
                    dropDownRect.Top + (dropDownRect.Height / 2));
                Point[] points;
                switch (direction)
                {
                    case ArrowDirection.Left:
                        points = new Point[] { 
                        new Point(point.X + 1, point.Y - 4), 
                        new Point(point.X + 1, point.Y + 4), 
                        new Point(point.X - 2, point.Y) };
                        break;

                    case ArrowDirection.Up:
                        points = new Point[] { 
                        new Point(point.X - 4, point.Y + 1), 
                        new Point(point.X + 4, point.Y + 1), 
                        new Point(point.X, point.Y - 2) };
                        break;

                    case ArrowDirection.Right:
                        points = new Point[] {
                        new Point(point.X - 2, point.Y - 4), 
                        new Point(point.X - 2, point.Y + 4), 
                        new Point(point.X + 1, point.Y) };
                        break;

                    default:
                        points = new Point[] {
                        new Point(point.X - 4, point.Y - 1), 
                        new Point(point.X + 4, point.Y - 1), 
                        new Point(point.X, point.Y + 2) };
                        break;
                }
                g.FillPolygon(brush, points);
            }

            internal static void RenderAlphaImage(
                Graphics g,
                System.Drawing.Image image,
                Rectangle imageRect,
                float alpha)
            {
                using (ImageAttributes imageAttributes = new ImageAttributes())
                {
                    ColorMap colorMap = new ColorMap();

                    colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                    colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);

                    ColorMap[] remapTable = { colorMap };

                    imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                    float[][] colorMatrixElements = { 
                    new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},       
                    new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},        
                    new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},        
                    new float[] {0.0f,  0.0f,  0.0f,  alpha, 0.0f},        
                    new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}};
                    ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);

                    imageAttributes.SetColorMatrix(
                        wmColorMatrix,
                        ColorMatrixFlag.Default,
                        ColorAdjustType.Bitmap);

                    g.DrawImage(
                        image,
                        imageRect,
                        0,
                        0,
                        image.Width,
                        image.Height,
                        GraphicsUnit.Pixel,
                        imageAttributes);
                }
            }

            internal static void RenderGrid(
                Graphics g,
                Rectangle rect,
                Size pixelsBetweenDots,
                Color outerColor)
            {
                int outerWin32Corlor = ColorTranslator.ToWin32(outerColor);
                IntPtr hdc = g.GetHdc();

                for (int x = rect.X; x <= rect.Right; x += pixelsBetweenDots.Width)
                {
                    for (int y = rect.Y; y <= rect.Bottom; y += pixelsBetweenDots.Height)
                    {
                        SetPixel(hdc, x, y, outerWin32Corlor);
                    }
                }

                g.ReleaseHdc(hdc);
            }

            internal static Color GetColor(
                Color colorBase, int a, int r, int g, int b)
            {
                int a0 = colorBase.A;
                int r0 = colorBase.R;
                int g0 = colorBase.G;
                int b0 = colorBase.B;

                a = a + a0 > 255 ? 255 : Math.Max(0, a + a0);
                r = r + r0 > 255 ? 255 : Math.Max(0, r + r0);
                g = g + g0 > 255 ? 255 : Math.Max(0, g + g0);
                b = b + b0 > 255 ? 255 : Math.Max(0, b + b0);

                return Color.FromArgb(a, r, g, b);
            }

            [System.Runtime.InteropServices.DllImport("gdi32.dll")]
            private static extern uint SetPixel(IntPtr hdc, int X, int Y, int crColor);
        }
        public enum RoundStyle
        {
            /// <summary>
            /// 四个角都不是圆角。
            /// </summary>
            None = 0,
            /// <summary>
            /// 四个角都为圆角。
            /// </summary>
            All = 1,
            /// <summary>
            /// 左边两个角为圆角。
            /// </summary>
            Left = 2,
            /// <summary>
            /// 右边两个角为圆角。
            /// </summary>
            Right = 3,
            /// <summary>
            /// 上边两个角为圆角。
            /// </summary>
            Top = 4,
            /// <summary>
            /// 下边两个角为圆角。
            /// </summary>
            Bottom = 5,
            /// <summary>
            /// 左下角为圆角。
            /// </summary>
            BottomLeft = 6,
            /// <summary>
            /// 右下角为圆角。
            /// </summary>
            BottomRight = 7,
        }
        public class SmoothingModeGraphics : IDisposable
        {
            private readonly SmoothingMode _oldMode;
            private readonly Graphics _graphics;

            public SmoothingModeGraphics(Graphics graphics)
                : this(graphics, SmoothingMode.AntiAlias)
            {
            }

            public SmoothingModeGraphics(Graphics graphics, SmoothingMode newMode)
            {
                _graphics = graphics;
                _oldMode = graphics.SmoothingMode;
                graphics.SmoothingMode = newMode;
            }

            #region IDisposable 成员

            public void Dispose()
            {
                _graphics.SmoothingMode = _oldMode;
            }

            #endregion
        }
        public enum ThumbArrowDirection
        {
            None = 0,
            Left = 1,
            Right = 2,
            Up = 3,
            Down = 4,
            LeftRight = 5,
            UpDown = 6
        }
        internal class TextRenderingHintGraphics : IDisposable
        {
            private readonly Graphics _graphics;
            private readonly TextRenderingHint _oldTextRenderingHint;

            public TextRenderingHintGraphics(Graphics graphics)
                : this(graphics, TextRenderingHint.AntiAlias)
            {
            }

            public TextRenderingHintGraphics(
                Graphics graphics,
                TextRenderingHint newTextRenderingHint)
            {
                _graphics = graphics;
                _oldTextRenderingHint = graphics.TextRenderingHint;
                _graphics.TextRenderingHint = newTextRenderingHint;
            }

            #region IDisposable 成员

            public void Dispose()
            {
                _graphics.TextRenderingHint = _oldTextRenderingHint;
            }

            #endregion
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.Cursor = Cursors.Default;
            MouseState = SplitContainer.ControlPaintEx.ControlState.Normal;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Rectangle collapseRect = CollapseRect;
            Point mousePoint = e.Location;

            if (collapseRect.Contains(mousePoint) ||
                (_collapsePanel != CollapsePanel.None &&
                _spliterPanelState == SpliterPanelState.Collapsed))
            {
                _histTest = HistTest.Button;
                return;
            }

            if (base.SplitterRectangle.Contains(mousePoint))
            {
                _histTest = HistTest.Spliter;
            }

            base.OnMouseDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            base.Invalidate(base.SplitterRectangle);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            base.Invalidate(base.SplitterRectangle);

            Rectangle collapseRect = CollapseRect;
            Point mousePoint = e.Location;

            if (_histTest == HistTest.Button && 
                e.Button == MouseButtons.Left &&
                collapseRect.Contains(mousePoint))
            {
                OnCollapseClick(EventArgs.Empty);
            }
            _histTest = HistTest.None;
        }

        private void SetCursor(Cursor cursor)
        {
            Cursor cursor1 = base.Cursor;
            if (cursor1 != null && cursor1 != cursor)
            {
                base.Cursor = cursor;
            }
        }

        private void CalculateRect(
            Rectangle collapseRect,
            out Rectangle arrowRect,
            out Rectangle topLeftRect,
            out Rectangle bottomRightRect)
        {
            int width;
            if (base.Orientation == Orientation.Horizontal)
            {
                width = (collapseRect.Width - DefaultArrowWidth) / 2;
                arrowRect = new Rectangle(
                    collapseRect.X + width,
                    collapseRect.Y,
                    DefaultArrowWidth,
                    collapseRect.Height);

                topLeftRect = new Rectangle(
                    collapseRect.X,
                    collapseRect.Y + 1,
                    width,
                    collapseRect.Height - 2);

                bottomRightRect = new Rectangle(
                    arrowRect.Right,
                    collapseRect.Y + 1,
                    width,
                    collapseRect.Height - 2);
            }
            else
            {
                width = (collapseRect.Height - DefaultArrowWidth) / 2;
                arrowRect = new Rectangle(
                    collapseRect.X,
                    collapseRect.Y + width,
                    collapseRect.Width,
                    DefaultArrowWidth);

                topLeftRect = new Rectangle(
                    collapseRect.X + 1,
                    collapseRect.Y,
                    collapseRect.Width - 2,
                    width);

                bottomRightRect = new Rectangle(
                    collapseRect.X + 1,
                    arrowRect.Bottom,
                    collapseRect.Width - 2,
                    width);
            }
        }
        /// <summary>
        /// 点击SplitContainer控件收缩按钮时隐藏的Panel。
        /// </summary>
        public enum CollapsePanel
        {
            None = 0,
            Panel1 = 1,
            Panel2 = 2,
        }
        internal enum SpliterPanelState
        {
            Collapsed = 0,
            Expanded = 1,
        }
        private enum HistTest
        {
            None,
            Button,
            Spliter
        }
        public class ControlPaintEx
        {
            private ControlPaintEx() { }

            public static void DrawCheckedFlag(
                Graphics g, Rectangle rect, Color color)
            {
                PointF[] points = new PointF[3];
                points[0] = new PointF(
                    rect.X + rect.Width / 4.5f,
                    rect.Y + rect.Height / 2.5f);
                points[1] = new PointF(
                    rect.X + rect.Width / 2.5f,
                    rect.Bottom - rect.Height / 3f);
                points[2] = new PointF(
                    rect.Right - rect.Width / 4.0f,
                    rect.Y + rect.Height / 4.5f);
                using (Pen pen = new Pen(color, 2F))
                {
                    g.DrawLines(pen, points);
                }
            }

            public static void DrawGlass(
                Graphics g, RectangleF glassRect,
                int alphaCenter, int alphaSurround)
            {
                DrawGlass(g, glassRect, Color.White, alphaCenter, alphaSurround);
            }

            public static void DrawGlass(
               Graphics g,
                RectangleF glassRect,
                Color glassColor,
                int alphaCenter,
                int alphaSurround)
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(glassRect);
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.CenterColor = Color.FromArgb(alphaCenter, glassColor);
                        brush.SurroundColors = new Color[] { 
                        Color.FromArgb(alphaSurround, glassColor) };
                        brush.CenterPoint = new PointF(
                            glassRect.X + glassRect.Width / 2,
                            glassRect.Y + glassRect.Height / 2);
                        g.FillPath(brush, path);
                    }
                }
            }

            public static void DrawBackgroundImage(
                Graphics g,
                System.Drawing.Image backgroundImage,
                Color backColor,
                ImageLayout backgroundImageLayout,
                Rectangle bounds,
                Rectangle clipRect)
            {
                DrawBackgroundImage(
                    g,
                    backgroundImage,
                    backColor,
                    backgroundImageLayout,
                    bounds,
                    clipRect,
                    Point.Empty,
                    RightToLeft.No);
            }

            public static void DrawBackgroundImage(
                Graphics g,
                System.Drawing.Image backgroundImage,
                Color backColor,
                ImageLayout backgroundImageLayout,
                Rectangle bounds,
                Rectangle clipRect,
                Point scrollOffset)
            {
                DrawBackgroundImage(
                    g,
                    backgroundImage,
                    backColor,
                    backgroundImageLayout,
                    bounds,
                    clipRect,
                    scrollOffset,
                    RightToLeft.No);
            }

            public static void DrawBackgroundImage(
                Graphics g,
                System.Drawing.Image backgroundImage,
                Color backColor,
                ImageLayout backgroundImageLayout,
                Rectangle bounds,
                Rectangle clipRect,
                Point scrollOffset,
                RightToLeft rightToLeft)
            {
                if (g == null)
                {
                    throw new ArgumentNullException("g");
                }
                if (backgroundImageLayout == ImageLayout.Tile)
                {
                    using (TextureBrush brush = new TextureBrush(backgroundImage, WrapMode.Tile))
                    {
                        if (scrollOffset != Point.Empty)
                        {
                            Matrix transform = brush.Transform;
                            transform.Translate(scrollOffset.X, scrollOffset.Y);
                            brush.Transform = transform;
                        }
                        g.FillRectangle(brush, clipRect);
                        return;
                    }
                }
                Rectangle rect = CalculateBackgroundImageRectangle(
                    bounds,
                    backgroundImage,
                    backgroundImageLayout);
                if ((rightToLeft == RightToLeft.Yes) &&
                    (backgroundImageLayout == ImageLayout.None))
                {
                    rect.X += clipRect.Width - rect.Width;
                }
                using (SolidBrush brush2 = new SolidBrush(backColor))
                {
                    g.FillRectangle(brush2, clipRect);
                }
                if (!clipRect.Contains(rect))
                {
                    if ((backgroundImageLayout == ImageLayout.Stretch) ||
                        (backgroundImageLayout == ImageLayout.Zoom))
                    {
                        rect.Intersect(clipRect);
                        g.DrawImage(backgroundImage, rect);
                    }
                    else if (backgroundImageLayout == ImageLayout.None)
                    {
                        rect.Offset(clipRect.Location);
                        Rectangle destRect = rect;
                        destRect.Intersect(clipRect);
                        Rectangle rectangle3 = new Rectangle(Point.Empty, destRect.Size);
                        g.DrawImage(
                            backgroundImage,
                            destRect,
                            rectangle3.X,
                            rectangle3.Y,
                            rectangle3.Width,
                            rectangle3.Height,
                            GraphicsUnit.Pixel);
                    }
                    else
                    {
                        Rectangle rectangle4 = rect;
                        rectangle4.Intersect(clipRect);
                        Rectangle rectangle5 = new Rectangle(
                            new Point(rectangle4.X - rect.X, rectangle4.Y - rect.Y),
                            rectangle4.Size);
                        g.DrawImage(
                            backgroundImage,
                            rectangle4,
                            rectangle5.X,
                            rectangle5.Y,
                            rectangle5.Width,
                            rectangle5.Height,
                            GraphicsUnit.Pixel);
                    }
                }
                else
                {
                    ImageAttributes imageAttr = new ImageAttributes();
                    imageAttr.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(
                        backgroundImage,
                        rect,
                        0,
                        0,
                        backgroundImage.Width,
                        backgroundImage.Height,
                        GraphicsUnit.Pixel,
                        imageAttr);
                    imageAttr.Dispose();
                }
            }

            public static void DrawScrollBarTrack(
                Graphics g,
                Rectangle rect,
                Color begin,
                Color end,
                Orientation orientation)
            {
                bool bHorizontal = orientation == Orientation.Horizontal;
                LinearGradientMode mode = bHorizontal ?
                    LinearGradientMode.Vertical : LinearGradientMode.Horizontal;

                Blend blend = new Blend();
                blend.Factors = new float[] { 1f, 0.5f, 0f };
                blend.Positions = new float[] { 0f, 0.5f, 1f };

                DrawGradientRect(
                    g,
                    rect,
                    begin,
                    end,
                    begin,
                    begin,
                    blend,
                    mode,
                    true,
                    false);
            }

            public static void DrawScrollBarThumb(
                Graphics g,
                Rectangle rect,
                Color begin,
                Color end,
                Color border,
                Color innerBorder,
                Orientation orientation,
                bool changeColor)
            {
                if (changeColor)
                {
                    Color tmp = begin;
                    begin = end;
                    end = tmp;
                }

                bool bHorizontal = orientation == Orientation.Horizontal;
                LinearGradientMode mode = bHorizontal ?
                    LinearGradientMode.Vertical : LinearGradientMode.Horizontal;

                Blend blend = new Blend();
                blend.Factors = new float[] { 1f, 0.5f, 0f };
                blend.Positions = new float[] { 0f, 0.5f, 1f };

                if (bHorizontal)
                {
                    rect.Inflate(0, -1);
                }
                else
                {
                    rect.Inflate(-1, 0);
                }

                DrawGradientRoundRect(
                    g,
                    rect,
                    begin,
                    end,
                    border,
                    innerBorder,
                    blend,
                    mode,
                    4,
                    RoundStyle.All,
                    true,
                    true);
            }

            public static void DrawScrollBarArraw(
                Graphics g,
                Rectangle rect,
                Color begin,
                Color end,
                Color border,
                Color innerBorder,
                Color fore,
                Orientation orientation,
                ArrowDirection arrowDirection,
                bool changeColor)
            {
                if (changeColor)
                {
                    Color tmp = begin;
                    begin = end;
                    end = tmp;
                }

                bool bHorizontal = orientation == Orientation.Horizontal;
                LinearGradientMode mode = bHorizontal ?
                    LinearGradientMode.Vertical : LinearGradientMode.Horizontal;

                rect.Inflate(-1, -1);

                Blend blend = new Blend();
                blend.Factors = new float[] { 1f, 0.5f, 0f };
                blend.Positions = new float[] { 0f, 0.5f, 1f };

                DrawGradientRoundRect(
                    g,
                    rect,
                    begin,
                    end,
                    border,
                    innerBorder,
                    blend,
                    mode,
                    4,
                    RoundStyle.All,
                    true,
                    true);

                using (SolidBrush brush = new SolidBrush(fore))
                {
                    RenderHelper.RenderArrowInternal(
                        g,
                        rect,
                        arrowDirection,
                        brush);
                }
            }

            public static void DrawScrollBarSizer(
                Graphics g,
                Rectangle rect,
                Color begin,
                Color end)
            {
                Blend blend = new Blend();
                blend.Factors = new float[] { 1f, 0.5f, 0f };
                blend.Positions = new float[] { 0f, 0.5f, 1f };

                DrawGradientRect(
                     g,
                     rect,
                     begin,
                     end,
                     begin,
                     begin,
                     blend,
                     LinearGradientMode.Horizontal,
                     true,
                     false);
            }

            internal static void DrawGradientRect(
                Graphics g,
                Rectangle rect,
                Color begin,
                Color end,
                Color border,
                Color innerBorder,
                Blend blend,
                LinearGradientMode mode,
                bool drawBorder,
                bool drawInnerBorder)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect, begin, end, mode))
                {
                    brush.Blend = blend;
                    g.FillRectangle(brush, rect);
                }

                if (drawBorder)
                {
                    ControlPaint.DrawBorder(
                        g, rect, border, ButtonBorderStyle.Solid);
                }

                if (drawInnerBorder)
                {
                    rect.Inflate(-1, -1);
                    ControlPaint.DrawBorder(
                        g, rect, border, ButtonBorderStyle.Solid);
                }
            }

            internal static void DrawGradientRoundRect(
                Graphics g,
                Rectangle rect,
                Color begin,
                Color end,
                Color border,
                Color innerBorder,
                Blend blend,
                LinearGradientMode mode,
                int radios,
                RoundStyle roundStyle,
                bool drawBorder,
                bool drawInnderBorder)
            {
                using (GraphicsPath path = GraphicsPathHelper.CreatePath(
                    rect, radios, roundStyle, true))
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(
                          rect, begin, end, mode))
                    {
                        brush.Blend = blend;
                        g.FillPath(brush, path);
                    }

                    if (drawBorder)
                    {
                        using (Pen pen = new Pen(border))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }

                if (drawInnderBorder)
                {
                    rect.Inflate(-1, -1);
                    using (GraphicsPath path = GraphicsPathHelper.CreatePath(
                        rect, radios, roundStyle, true))
                    {
                        using (Pen pen = new Pen(innerBorder))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
            }
            public enum ControlState
            {
                /// <summary>
                ///  正常。
                /// </summary>
                Normal,
                /// <summary>
                /// 鼠标进入。
                /// </summary>
                Hover,
                /// <summary>
                /// 鼠标按下。
                /// </summary>
                Pressed,
                /// <summary>
                /// 获得焦点。
                /// </summary>
                Focused,
            }
            internal static Rectangle CalculateBackgroundImageRectangle(
                Rectangle bounds,
                System.Drawing.Image backgroundImage,
                ImageLayout imageLayout)
            {
                Rectangle rectangle = bounds;
                if (backgroundImage != null)
                {
                    switch (imageLayout)
                    {
                        case ImageLayout.None:
                            rectangle.Size = backgroundImage.Size;
                            return rectangle;

                        case ImageLayout.Tile:
                            return rectangle;

                        case ImageLayout.Center:
                            {
                                rectangle.Size = backgroundImage.Size;
                                Size size = bounds.Size;
                                if (size.Width > rectangle.Width)
                                {
                                    rectangle.X = (size.Width - rectangle.Width) / 2;
                                }
                                if (size.Height > rectangle.Height)
                                {
                                    rectangle.Y = (size.Height - rectangle.Height) / 2;
                                }
                                return rectangle;
                            }
                        case ImageLayout.Stretch:
                            rectangle.Size = bounds.Size;
                            return rectangle;

                        case ImageLayout.Zoom:
                            {
                                Size size2 = backgroundImage.Size;
                                float num = bounds.Width / ((float)size2.Width);
                                float num2 = bounds.Height / ((float)size2.Height);
                                if (num >= num2)
                                {
                                    rectangle.Height = bounds.Height;
                                    rectangle.Width = (int)((size2.Width * num2) + 0.5);
                                    if (bounds.X >= 0)
                                    {
                                        rectangle.X = (bounds.Width - rectangle.Width) / 2;
                                    }
                                    return rectangle;
                                }
                                rectangle.Width = bounds.Width;
                                rectangle.Height = (int)((size2.Height * num) + 0.5);
                                if (bounds.Y >= 0)
                                {
                                    rectangle.Y = (bounds.Height - rectangle.Height) / 2;
                                }
                                return rectangle;
                            }
                    }
                }
                return rectangle;
            }
        }
    }
}
