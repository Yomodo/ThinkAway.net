using System.Drawing;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Controls
{
    /// <summary>
    /// 
    /// </summary>
	public class LedDigital : System.Windows.Forms.Control
	{
        private int _scaler;
        private SolidBrush _sbb;
        private SolidBrush _sbs;
        private Color _foreColor;
        private Color _backColor;
        private System.Windows.Forms.Control _container;
        private readonly Graphics _graphics;
	    private readonly Point[][] _digital =
	        {
                    //第一个坐标点
	            new Point[]
	                {
	                    new Point(3, 2),
	                    new Point(5, 4),
	                    new Point(13, 4),
	                    new Point(15, 2),
	                    new Point(13, 0),
	                    new Point(5, 0)
	                },
                    //第二个坐标点
	            new Point[]
	                {
	                    new Point(16, 3),
	                    new Point(14, 5),
	                    new Point(14, 13),
	                    new Point(16, 15),
	                    new Point(18, 13),
	                    new Point(18, 5)
	                },
                    //第三个坐标点
	            new Point[]
	                {
	                    new Point(16, 17),
	                    new Point(14, 19),
	                    new Point(14, 27),
	                    new Point(16, 29),
	                    new Point(18, 27),
	                    new Point(18, 19)
	                },
                    //第四个坐标点
	            new Point[]
	                {
	                    new Point(5, 28),
	                    new Point(3, 30),
	                    new Point(5, 32),
	                    new Point(13, 32),
	                    new Point(15, 30),
	                    new Point(13, 28)
	                },
                    //第五个坐标点
	            new Point[]
	                {
	                    new Point(2, 17),
	                    new Point(0, 19),
	                    new Point(0, 27),
	                    new Point(2, 29),
	                    new Point(4, 27),
	                    new Point(4, 19)
	                },
                    //第六个坐标点
	            new Point[]
	                {
	                    new Point(2, 3),
	                    new Point(0, 5),
	                    new Point(0, 13),
	                    new Point(2, 15),
	                    new Point(4, 13),
	                    new Point(4, 5)
	                },
                    //第七个坐标点
	            new Point[]
	                {
	                    new Point(5, 14),
	                    new Point(3, 16),
	                    new Point(5, 18),
	                    new Point(13, 18),
	                    new Point(15, 16),
	                    new Point(13, 14)
	                }
	        };

        /// <summary>
        /// 
        /// </summary>
        public override Color ForeColor
        {
            set
            {
                _foreColor = value;
                _sbs = new SolidBrush(_foreColor);
            }
        }
		/// <summary>
		/// 
		/// </summary>
		public override Color BackColor
		{
            set
            {
                _backColor = value;
                _sbb = new SolidBrush(_backColor);
            }
		}
        /// <summary>
        /// 
        /// </summary>
        public System.Windows.Forms.Control DigitalContainer
        {
            set { _container = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="scaler"></param>
        public LedDigital(System.Windows.Forms.Control control) : this(control, 1)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="scaler"></param>
        public LedDigital(System.Windows.Forms.Control control, int scaler)
		{
		    _container = control;
		    _backColor = control.BackColor;
		    _foreColor = control.ForeColor;

            Scale(scaler);
            _graphics = Graphics.FromHwnd(_container.Handle);//_container.CreateGraphics();
            _graphics.Clear(_backColor);
            _sbb = new SolidBrush(_backColor);
            _sbs = new SolidBrush(_foreColor);
		}

        private void Scale(int x, Point[] p)
        {
            for (int i = 0; i < p.Length; i++)
            {
                p[i].X *= x;
                p[i].Y *= x;
            }
        }

        private void Scale(int scaler)
        {
            _scaler = scaler;
            foreach(Point[] part in _digital)
            {
                Scale(scaler,part);
            }
        }
        private void NextChar(int offset)
        {
            foreach (Point[] p in _digital)
            {
                for (int i = 0; i < p.Length; i++)
                {
                    p[i].X += offset;
                }
            }
        }

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="str"></param>
	    public void Show(string str)
        {
            foreach (char chr in str)
            {
                Show(chr);
            }
        }
	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="chr"></param>
	    public void Show(char chr)
        {
            switch (chr)
            {
                case '0':
                case 'O':
                    DrawDigtial(0,1,2,3,4,5);
                    break;
                case '1':
                    DrawDigtial(1,2);
                    break;
                case '2':
                    DrawDigtial(0,1,3,4,6);
                    break;
                case '3':
                    DrawDigtial(0,1,2,3,6);
                    break;
                case '4':
                    DrawDigtial(1,2,5,6);
                    break;
                case '5':
                case 's':
                case 'S':
                    DrawDigtial(0,2,3,5,6);
                    break;
                case '6':
                    DrawDigtial(0,2,3,4,5,6);
                    break;
                case '7':
                    DrawDigtial(0,1,2);
                    break;
                case '8':
                    DrawDigtial(0,1,2,3,4,5,6);
                    break;
                case '9':
                    DrawDigtial(0,1,2,3,5,6);
                    break;
                case 'a':
                case 'A':
                    DrawDigtial(0,1,2,4,5,6);
                    break;
                case 'b':
                    break;
                case 'c':
                    DrawDigtial(3, 4, 6);
                    break;
                case 'C':
                    DrawDigtial(0, 3, 4, 5);
                    break;
                case 'd':
                case 'D':
                    break;
                case 'e':
                case 'E':
                    DrawDigtial(0, 3,4, 5, 6);
                    break;
                case 'f':
                case 'F':
                    DrawDigtial(0, 4, 5, 6);
                    break;
                case 'g':
                case 'G':
                    break;
                case 'h':
                    DrawDigtial(2, 4, 5, 6);
                    break;
                case 'H':
                    DrawDigtial(1, 2, 4, 5, 6);
                    break;
                case 'i':
                case 'I':
                    break;
                case 'j':
                case 'J':
                    //DrawDigtial(0, 1, 2, 4, 5, 6);
                    break;
                case 'k':
                case 'K':
                    break;
                case 'l':
                case 'L':
                    DrawDigtial(3, 4, 5);
                    break;
                case 'm':
                case 'M':
                    break;
                case 'n':
                    DrawDigtial(2, 4, 6);
                    break;
                case 'N':
                    DrawDigtial(0, 1, 2, 4, 5);
                    break;
                case 'o':
                    DrawDigtial(2, 3, 4, 6);
                    break;
                case 'p':
                case 'P':
                    DrawDigtial(0, 1, 4, 5, 6);
                    break;
                case 'q':
                case 'Q':
                    DrawDigtial(0, 1, 2, 5, 6);
                    break;
                case 'r':
                case 'R':
                    DrawDigtial(4, 6);
                    break;
                case 't':
                case 'T':

                    break;
                case 'u':
                    DrawDigtial(2,3, 4);
                    break;
                case 'U':
                    DrawDigtial(1, 2,3, 4, 5);
                    break;
                case 'v':
                case 'V':
                    break;
                case 'w':
                case 'W':
                    break;
                case 'x':
                case 'X':
                    break;
                case 'y':
                case 'Y':
                case 'z':
                case 'Z':
                    break;
                case '-':
                    DrawDigtial(6);
                    break;
                case '=':
                    DrawDigtial(3,6);
                    break;
                case '_':
                    DrawDigtial(3);
                    break;
                default:
                    DrawDigtial(0, 1, 2,3, 4, 5, 6);
                    break;
            }
            
        }
        private void DrawDigtial(params int[] dot)
        {
            foreach (Point[] part in _digital)
            {
                _graphics.FillPolygon(_sbb, part);
            }
            foreach(int d in dot)
            {
                _graphics.FillPolygon(_sbs, _digital[d]);
            }
            NextChar(20 * _scaler);
        }
	}
}
