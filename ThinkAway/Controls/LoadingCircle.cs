using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Controls
{
    public class LoadingCircle : System.Windows.Forms.Control
    {
        #region 常数

        private const double NumberOfDegreesInCircle = 360;
        private const double NumberOfDegreesInHalfCircle = NumberOfDegreesInCircle / 2;
        private const int DefaultInnerCircleRadius = 8;
        private const int DefaultOuterCircleRadius = 10;
        private const int DefaultNumberOfSpoke = 10;
        private const int DefaultSpokeThickness = 4;
        private readonly Color _defaultColor = Color.DarkGray;

        private const int MacOsxInnerCircleRadius = 5;
        private const int MacOsxOuterCircleRadius = 11;
        private const int MacOsxNumberOfSpoke = 12;
        private const int MacOsxSpokeThickness = 2;

        private const int FireFoxInnerCircleRadius = 6;
        private const int FireFoxOuterCircleRadius = 7;
        private const int FireFoxNumberOfSpoke = 9;
        private const int FireFoxSpokeThickness = 4;

        private const int Ie7InnerCircleRadius = 8;
        private const int Ie7OuterCircleRadius = 9;
        private const int Ie7NumberOfSpoke = 24;
        private const int Ie7SpokeThickness = 4;

        #endregion

        #region 枚举

        public enum StylePresets
        {
            MacOsx,
            Firefox,
            Ie7,
            Custom
        }

        #endregion

        #region 局部变量

        private readonly Timer _mTimer;
        private bool _mIsTimerActive;
        private int _mNumberOfSpoke;
        private int _mSpokeThickness;
        private int _mProgressValue;
        private int _mOuterCircleRadius;
        private int _mInnerCircleRadius;
        private PointF _mCenterPoint;
        private Color _mColor;
        private Color[] _mColors;
        private double[] _mAngles;
        private StylePresets _mStylePreset;

        #endregion

        #region 属性

        /// <summary>
        /// 获取和设置控件高亮色
        /// </summary>
        /// <value>高亮色</value>
        [TypeConverter("System.Drawing.ColorConverter"),
        Category("LoadingCircle"),
        Description("获取和设置控件高亮色")]
        public Color Color
        {
            get
            {
                return _mColor;
            }
            set
            {
                _mColor = value;

                GenerateColorsPallet();
                Invalidate();
            }
        }

        /// <summary>
        /// 获取和设置外围半径
        /// </summary>
        /// <value>外围半径</value>
        [Description("获取和设置外围半径"),
         Category("LoadingCircle")]
        public int OuterCircleRadius
        {
            get
            {
                if (_mOuterCircleRadius == 0)
                    _mOuterCircleRadius = DefaultOuterCircleRadius;

                return _mOuterCircleRadius;
            }
            set
            {
                _mOuterCircleRadius = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 获取和设置内圆半径
        /// </summary>
        /// <value>内圆半径</value>
        [Description("获取和设置内圆半径"),
         Category("LoadingCircle")]
        public int InnerCircleRadius
        {
            get
            {
                if (_mInnerCircleRadius == 0)
                    _mInnerCircleRadius = DefaultInnerCircleRadius;

                return _mInnerCircleRadius;
            }
            set
            {
                _mInnerCircleRadius = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 获取和设置辐条数量
        /// </summary>
        /// <value>辐条数量</value>
        [Description("获取和设置辐条数量"),
        Category("LoadingCircle")]
        public int NumberSpoke
        {
            get
            {
                if (_mNumberOfSpoke == 0)
                    _mNumberOfSpoke = DefaultNumberOfSpoke;

                return _mNumberOfSpoke;
            }
            set
            {
                if (_mNumberOfSpoke != value && _mNumberOfSpoke > 0)
                {
                    _mNumberOfSpoke = value;
                    GenerateColorsPallet();
                    GetSpokesAngles();

                    Invalidate();
                }
            }
        }

        /// <summary>
        /// 获取和设置一个布尔值，表示当前控件
        ///   是否激活。
        /// </summary>
        /// <value><c>true</c> 表示激活；否则，为<c>false</c>。</value>
        [Description("获取和设置一个布尔值，表示当前控件是否激活。"),
        Category("LoadingCircle")]
        public bool Active
        {
            get
            {
                return _mIsTimerActive;
            }
            set
            {
                _mIsTimerActive = value;
                ActiveTimer();
            }
        }

        /// <summary>
        /// 获取和设置辐条粗细程度。
        /// </summary>
        /// <value>辐条粗细值</value>
        [Description("获取和设置辐条粗细程度。"),
        Category("LoadingCircle")]
        public int SpokeThickness
        {
            get
            {
                if (_mSpokeThickness <= 0)
                    _mSpokeThickness = DefaultSpokeThickness;

                return _mSpokeThickness;
            }
            set
            {
                _mSpokeThickness = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 获取和设置旋转速度。
        /// </summary>
        /// <value>旋转速度</value>
        [Description("获取和设置旋转速度。"),
        Category("LoadingCircle")]
        public int RotationSpeed
        {
            get
            {
                return _mTimer.Interval;
            }
            set
            {
                if (value > 0)
                    _mTimer.Interval = value;
            }
        }

        /// <summary>
        /// 快速设置预定义风格。
        /// </summary>
        /// <value>风格的值</value>
        [Category("LoadingCircle"),
        Description("快速设置预定义风格。"),
         DefaultValue(typeof(StylePresets), "Custom")]
        public StylePresets StylePreset
        {
            get { return _mStylePreset; }
            set
            {
                _mStylePreset = value;

                switch (_mStylePreset)
                {
                    case StylePresets.MacOsx:
                        SetCircleAppearance(MacOsxNumberOfSpoke,
                            MacOsxSpokeThickness, MacOsxInnerCircleRadius,
                            MacOsxOuterCircleRadius);
                        break;
                    case StylePresets.Firefox:
                        SetCircleAppearance(FireFoxNumberOfSpoke,
                            FireFoxSpokeThickness, FireFoxInnerCircleRadius,
                            FireFoxOuterCircleRadius);
                        break;
                    case StylePresets.Ie7:
                        SetCircleAppearance(Ie7NumberOfSpoke,
                            Ie7SpokeThickness, Ie7InnerCircleRadius,
                            Ie7OuterCircleRadius);
                        break;
                    case StylePresets.Custom:
                        SetCircleAppearance(DefaultNumberOfSpoke,
                            DefaultSpokeThickness,
                            DefaultInnerCircleRadius,
                            DefaultOuterCircleRadius);
                        break;
                }
            }
        }

        #endregion

        #region 构造函数及事件处理
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        public LoadingCircle()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            _mColor = _defaultColor;

            GenerateColorsPallet();
            GetSpokesAngles();
            GetControlCenterPoint();

            _mTimer = new Timer();
            _mTimer.Tick += ATimerTick;
            ActiveTimer();

            this.Resize += LoadingCircleResize;
        }

        void LoadingCircleResize(object sender, EventArgs e)
        {
            GetControlCenterPoint();
        }

        void ATimerTick(object sender, EventArgs e)
        {
            _mProgressValue = ++_mProgressValue % _mNumberOfSpoke;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_mNumberOfSpoke > 0)
            {
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

                int intPosition = _mProgressValue;
                for (int intCounter = 0; intCounter < _mNumberOfSpoke; intCounter++)
                {
                    intPosition = intPosition % _mNumberOfSpoke;
                    DrawLine(e.Graphics,
                             GetCoordinate(_mCenterPoint, _mInnerCircleRadius, _mAngles[intPosition]),
                             GetCoordinate(_mCenterPoint, _mOuterCircleRadius, _mAngles[intPosition]),
                             _mColors[intCounter], _mSpokeThickness);
                    intPosition++;
                }
            }

            base.OnPaint(e);
        }

        #endregion

        #region 局部方法

        private Color Darken(Color objColor, int intPercent)
        {
            int intRed = objColor.R;
            int intGreen = objColor.G;
            int intBlue = objColor.B;
            return Color.FromArgb(intPercent, Math.Min(intRed, byte.MaxValue), Math.Min(intGreen, byte.MaxValue), Math.Min(intBlue, byte.MaxValue));
        }

        private void GenerateColorsPallet()
        {
            _mColors = GenerateColorsPallet(_mColor, Active, _mNumberOfSpoke);
        }

        private Color[] GenerateColorsPallet(Color objColor, bool blnShadeColor, int intNbSpoke)
        {
            Color[] objColors = new Color[NumberSpoke];

            byte bytIncrement = (byte)(byte.MaxValue / NumberSpoke);

            byte PERCENTAGE_OF_DARKEN = 0;

            for (int intCursor = 0; intCursor < NumberSpoke; intCursor++)
            {
                if (blnShadeColor)
                {
                    if (intCursor == 0 || intCursor < NumberSpoke - intNbSpoke)
                        objColors[intCursor] = objColor;
                    else
                    {
                        PERCENTAGE_OF_DARKEN += bytIncrement;

                        if (PERCENTAGE_OF_DARKEN > byte.MaxValue)
                            PERCENTAGE_OF_DARKEN = byte.MaxValue;

                        objColors[intCursor] = Darken(objColor, PERCENTAGE_OF_DARKEN);
                    }
                }
                else
                    objColors[intCursor] = objColor;
            }

            return objColors;
        }

        private void GetControlCenterPoint()
        {
            _mCenterPoint = GetControlCenterPoint(this);
        }

        private PointF GetControlCenterPoint(System.Windows.Forms.Control objControl)
        {
// ReSharper disable PossibleLossOfFraction
            if (objControl != null) return new PointF(objControl.Width / 2, objControl.Height / 2 - 1);
// ReSharper restore PossibleLossOfFraction
            return new PointF();
        }

        private void DrawLine(Graphics objGraphics, PointF objPointOne, PointF objPointTwo,Color objColor, int intLineThickness)
        {
            using (Pen objPen = new Pen(new SolidBrush(objColor), intLineThickness))
            {
                objPen.StartCap = LineCap.Round;
                objPen.EndCap = LineCap.Round;
                objGraphics.DrawLine(objPen, objPointOne, objPointTwo);
            }
        }

        private PointF GetCoordinate(PointF objCircleCenter, int intRadius, double _dblAngle)
        {
            double dblAngle = Math.PI * _dblAngle / NumberOfDegreesInHalfCircle;

            return new PointF(objCircleCenter.X + intRadius * (float)Math.Cos(dblAngle),
                              objCircleCenter.Y + intRadius * (float)Math.Sin(dblAngle));
        }

        private void GetSpokesAngles()
        {
            _mAngles = GetSpokesAngles(NumberSpoke);
        }

        private double[] GetSpokesAngles(int intNumberSpoke)
        {
            double[] Angles = new double[intNumberSpoke];
            double dblAngle = NumberOfDegreesInCircle / intNumberSpoke;

            for (int shtCounter = 0; shtCounter < intNumberSpoke; shtCounter++)
                Angles[shtCounter] = (shtCounter == 0 ? dblAngle : Angles[shtCounter - 1] + dblAngle);

            return Angles;
        }

        private void ActiveTimer()
        {
            if (_mIsTimerActive)
                _mTimer.Start();
            else
            {
                _mTimer.Stop();
                _mProgressValue = 0;
            }

            GenerateColorsPallet();
            Invalidate();
        }

        #endregion

        #region 全局方法

        /// <summary>
        /// 获取适合控件区域的矩形大小。
        /// </summary>
        /// <param name="proposedSize">The custom-sized area for a control.</param>
        /// <returns>
        /// An ordered pair of type <see cref="T:System.Drawing.Size"></see> representing the width and height of a rectangle.
        /// </returns>
        public override Size GetPreferredSize(Size proposedSize)
        {
            proposedSize.Width =
                (_mOuterCircleRadius + _mSpokeThickness) * 2;

            return proposedSize;
        }
        /// <summary>
        /// 设置控件的外观
        /// </summary>
        /// <param name="numberSpoke">条数</param>
        /// <param name="spokeThickness">粗细</param>
        /// <param name="innerCircleRadius">内圆半径</param>
        /// <param name="outerCircleRadius">外圆半径</param>
        public void SetCircleAppearance(int numberSpoke, int spokeThickness,
            int innerCircleRadius, int outerCircleRadius)
        {
            NumberSpoke = numberSpoke;
            SpokeThickness = spokeThickness;
            InnerCircleRadius = innerCircleRadius;
            OuterCircleRadius = outerCircleRadius;

            Invalidate();
        }

        #endregion
    }
}