using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Controls
{
	/// <summary>
	/// Summary description for ScrollingTextControl.
	/// </summary>
	[
	ToolboxBitmapAttribute(typeof(ScrollingText), ""),
	DefaultEvent("TextClicked")
	]
	public class ScrollingText : System.Windows.Forms.Control
	{
		private readonly Timer _timer;							// Timer for text animation.
		private float _staticTextPos;				// The running x pos of the text
		private float _yPos;							// The running y pos of the text
		private ScrollDirection _scrollDirection = ScrollDirection.RightToLeft;				// The direction the text will scroll
		private ScrollDirection _currentDirection = ScrollDirection.LeftToRight;				// Used for text bouncing 
		private VerticleTextPosition _verticleTextPosition = VerticleTextPosition.Center;	// Where will the text be vertically placed
		private int _scrollPixelDistance = 2;			// How far the text scrolls per timer event
		private bool _showBorder = true;					// Show a border or not
		private bool _stopScrollOnMouseOver;		// Flag to stop the scroll if the user mouses over the text
		private bool _scrollOn = true;					// Internal flag to stop / start the scrolling of the text
		private Brush _foregroundBrush;			// Allow the user to set a custom Brush to the text Font
		private Brush _backgroundBrush;			// Allow the user to set a custom Brush to the background
		private Color _borderColor = Color.Black;		// Allow the user to set the color of the control border
		private RectangleF _lastKnownRect;				// The last known position of the text

		public ScrollingText()
		{
			// Setup default properties for ScrollingText control
			InitializeComponent();
			
			//This turns off internal double buffering of all custom GDI+ drawing
			this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);

			//setup the timer object
			_timer = new Timer();
		    _timer.Interval = 25;
		    _timer.Enabled = false;
		    _timer.Tick += Tick;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				//Make sure our brushes are cleaned up
				if (_foregroundBrush != null)
					_foregroundBrush.Dispose();

				//Make sure our brushes are cleaned up
				if (_backgroundBrush != null)
					_backgroundBrush.Dispose();

				//Make sure our timer is cleaned up
				if (_timer != null)
					_timer.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			//ScrollingText			
			this.Name = "ScrollingText";
			this.Size = new System.Drawing.Size(216, 40);
			this.Click += new System.EventHandler(this.ScrollingText_Click);					
		}
		#endregion
	
		//Controls the animation of the text.
		private void Tick(object sender, EventArgs e)
		{
			//update rectangle to include where to paint for new position			
			//lastKnownRect.X -= 10;
			//lastKnownRect.Width += 20;			
			_lastKnownRect.Inflate(10, 5);

			//create region based on updated rectangle
			Region updateRegion = new Region(_lastKnownRect);			
			
			//repaint the control			
			Invalidate(updateRegion);
			Update();
		}

		//Paint the ScrollingTextCtrl.
		protected override void OnPaint(PaintEventArgs pe)
		{
			//Console.WriteLine(pe.ClipRectangle.X + ",  " + pe.ClipRectangle.Y + ",  " + pe.ClipRectangle.Width + ",  " + pe.ClipRectangle.Height);

			//Paint the text to its new position
			DrawScrollingText(pe.Graphics);

			//pass on the graphics obj to the base Control class
			base.OnPaint(pe);
		}

		//Draw the scrolling text on the control		
		public void DrawScrollingText(Graphics canvas)
		{
			//measure the size of the string for placement calculation
            SizeF stringSize = canvas.MeasureString(this.Text, this.Font);
		
			//Calculate the begining x position of where to paint the text
			if (_scrollOn)
			{
				CalcTextPosition(stringSize);	
			}

			//Clear the control with user set BackColor
			if (_backgroundBrush != null)
			{
				canvas.FillRectangle(_backgroundBrush, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
			}
			else
				canvas.Clear(this.BackColor);					

			// Draw the border
			if (_showBorder)
				using (Pen borderPen = new Pen(_borderColor))
					canvas.DrawRectangle(borderPen, 0, 0, this.ClientSize.Width-1, this.ClientSize.Height-1);					

			// Draw the text string in the bitmap in memory
			if (_foregroundBrush == null)					
			{
				using (Brush tempForeBrush = new System.Drawing.SolidBrush(this.ForeColor))
                    canvas.DrawString(this.Text, this.Font, tempForeBrush, _staticTextPos, _yPos);	
			}
			else
                canvas.DrawString(this.Text, this.Font, _foregroundBrush, _staticTextPos, _yPos);						

			_lastKnownRect = new RectangleF(_staticTextPos, _yPos, stringSize.Width, stringSize.Height);			
			EnableTextLink(_lastKnownRect);
		}

		private void CalcTextPosition(SizeF stringSize)
		{
			switch (_scrollDirection)
			{
				case ScrollDirection.RightToLeft:
					if (_staticTextPos < (-1 * (stringSize.Width)))
						_staticTextPos = this.ClientSize.Width - 1;
					else
						_staticTextPos -= _scrollPixelDistance;
					break;
				case ScrollDirection.LeftToRight:
					if (_staticTextPos > this.ClientSize.Width)
						_staticTextPos = -1 * stringSize.Width;
					else
						_staticTextPos += _scrollPixelDistance;
					break;
				case ScrollDirection.Bouncing:
					if (_currentDirection == ScrollDirection.RightToLeft)
					{
						if (_staticTextPos < 0)
							_currentDirection = ScrollDirection.LeftToRight;
						else
							_staticTextPos -= _scrollPixelDistance;						
					}
					else if (_currentDirection == ScrollDirection.LeftToRight)
					{
						if (_staticTextPos > this.ClientSize.Width - stringSize.Width)
							_currentDirection = ScrollDirection.RightToLeft;
						else
							_staticTextPos += _scrollPixelDistance;						
					}
					break;
			}				

			//Calculate the vertical position for the scrolling text				
			switch (_verticleTextPosition)
			{
				case VerticleTextPosition.Top:
					_yPos = 2;
					break;
				case VerticleTextPosition.Center:
// ReSharper disable PossibleLossOfFraction
					_yPos = (this.ClientSize.Height / 2) - (stringSize.Height / 2);
// ReSharper restore PossibleLossOfFraction
					break;
				case VerticleTextPosition.Botom:
					_yPos = this.ClientSize.Height - stringSize.Height;
					break;
			}
		}
        public void Stop()
        {
            _staticTextPos = 0;
            this._timer.Stop();
        }
        public void Start()
        {
            this._timer.Start();
        }

	    #region Mouse over, text link logic
		private void EnableTextLink(RectangleF textRect)
		{
			Point curPt = this.PointToClient(Cursor.Position);

			//if (curPt.X > textRect.Left && curPt.X < textRect.Right
			//	&& curPt.Y > textRect.Top && curPt.Y < textRect.Bottom)			
			if (textRect.Contains(curPt))
			{
				//Stop the text of the user mouse's over the text
				if (_stopScrollOnMouseOver)
					_scrollOn = false;				

				this.Cursor = Cursors.Hand;									
			}
			else
			{
				//Make sure the text is scrolling if user's mouse is not over the text
				_scrollOn = true;
				
				this.Cursor = Cursors.Default;
			}
		}		

		private void ScrollingText_Click(object sender, System.EventArgs e)
		{
			//Trigger the text clicked event if the user clicks while the mouse 
			//is over the text.  This allows the text to act like a hyperlink
			if (this.Cursor == Cursors.Hand)
				OnTextClicked(this, new EventArgs());
		}

		public delegate void TextClickEventHandler(object sender, EventArgs args);	
		public event TextClickEventHandler TextClicked;

		private void OnTextClicked(object sender, EventArgs args)
		{
			//Call the delegate
			if (TextClicked != null)
				TextClicked(sender, args);
		}
		#endregion
		

		#region Properties
		[
		Browsable(true),
		CategoryAttribute("Scrolling Text"),
		Description("The timer interval that determines how often the control is repainted")
		]
		public int TextScrollSpeed
		{
			set
			{
				if(value > 0)
				{
                    _timer.Interval = value;
				}
			}
			get
			{
				return _timer.Interval;
			}
		}

		[
		Browsable(true),
		CategoryAttribute("Scrolling Text"),
		Description("How many pixels will the text be moved per Paint")
		]
		public int TextScrollDistance
		{
			set
			{
				_scrollPixelDistance = value;
			}
			get
			{
				return _scrollPixelDistance;
			}
		}

		[
		Browsable(false),
		CategoryAttribute("Scrolling Text"),
		Description("The text that will scroll accros the control")
		]
		public string ScrollText
		{
			set
			{
				Text = value;
				this.Invalidate();
				this.Update();
			}
			get
			{
                return Text;
			}
		}

		[
		Browsable(true),
		CategoryAttribute("Scrolling Text"),
		Description("What direction the text will scroll: Left to Right, Right to Left, or Bouncing")
		]
		public ScrollDirection ScrollDirection
		{
			set
			{
				_scrollDirection = value;
			}
			get
			{
				return _scrollDirection;
			}
		}

		[
		Browsable(true),
		CategoryAttribute("Scrolling Text"),
		Description("The verticle alignment of the text")
		]
		public VerticleTextPosition VerticleTextPosition
		{
			set
			{
				_verticleTextPosition = value;
			}
			get
			{
				return _verticleTextPosition;
			}
		}

		[
		Browsable(true),
		CategoryAttribute("Scrolling Text"),
		Description("Turns the border on or off")
		]
		public bool ShowBorder
		{
			set
			{
				_showBorder = value;
			}
			get
			{
				return _showBorder;
			}
		}

		[
		Browsable(true),
		CategoryAttribute("Scrolling Text"),
		Description("The color of the border")
		]
		public Color BorderColor
		{
			set
			{
				_borderColor = value;
			}
			get
			{
				return _borderColor;
			}
		}

		[
		Browsable(true),
		CategoryAttribute("Scrolling Text"),
		Description("Determines if the text will stop scrolling if the user's mouse moves over the text")
		]
		public bool StopScrollOnMouseOver
		{
			set
			{
				_stopScrollOnMouseOver = value;
			}
			get
			{
				return _stopScrollOnMouseOver;
			}
		}

		[
		Browsable(false)
		]
		public Brush ForegroundBrush
		{
			set
			{
				_foregroundBrush = value;
			}
			get
			{
				return _foregroundBrush;
			}
		}
		
		[
		ReadOnly(true)
		]
		public Brush BackgroundBrush
		{
			set
			{
				_backgroundBrush = value;
			}
			get
			{
				return _backgroundBrush;
			}
		}
		#endregion		
	}


	
	public enum ScrollDirection
	{
		RightToLeft
		,LeftToRight
		,Bouncing
	}

	public enum VerticleTextPosition
	{
		Top
		,Center
		,Botom
	}
}
