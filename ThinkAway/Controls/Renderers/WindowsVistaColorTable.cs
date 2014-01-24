using System.Drawing;

namespace ThinkAway.Controls.Renderers
{
    public class WindowsVistaColorTable
    {
        #region Fields

        #endregion

        #region Ctor

        public WindowsVistaColorTable()
        {
            BackgroundNorth = Color.Black;
            BackgroundSouth = Color.Black;

            GlossyEffectNorth = Color.FromArgb(217, 0x68, 0x7C, 0xAC);
            GlossyEffectSouth = Color.FromArgb(74, 0xAA, 0xB5, 0xD0);

            BackgroundBorder = Color.FromArgb(0x85, 0x85, 0x87);
            BackgroundGlow = Color.FromArgb(0x43, 0x53, 0x7A);

            Text = Color.White;

            ButtonOuterBorder = Color.FromArgb(0x75, 0x7D, 0x95);
            ButtonInnerBorder = Color.FromArgb(0xBF, 0xC4, 0xCE);
            ButtonInnerBorderPressed = Color.FromArgb(0x4b, 0x4b, 0x4b);
            ButtonBorder = Color.FromArgb(0x03, 0x07, 0x0D);
            ButtonFillNorth = Color.FromArgb(85, Color.White);
            ButtonFillSouth = Color.FromArgb(1, Color.White);
            ButtonFillNorthPressed = Color.FromArgb(150, Color.Black);
            ButtonFillSouthPressed = Color.FromArgb(100, Color.Black);

            Glow = Color.FromArgb(0x30, 0x73, 0xCE);
            DropDownArrow = Color.White;

            MenuHighlight = Color.FromArgb(0xA8, 0xD8, 0xEB);
            MenuHighlightNorth = Color.FromArgb(25, MenuHighlight);
            MenuHighlightSouth = Color.FromArgb(102, MenuHighlight);
            MenuBackground = Color.FromArgb(0xF1, 0xF1, 0xF1);
            MenuDark = Color.FromArgb(0xE2, 0xE3, 0xE3);
            MenuLight = Color.White;

            SeparatorNorth = BackgroundSouth;
            SeparatorSouth = GlossyEffectNorth;

            MenuText = Color.Black;

            CheckedGlow = Color.FromArgb(0x57, 0xC6, 0xEF);
            CheckedGlowHot = Color.FromArgb(0x70, 0xD4, 0xFF);
            CheckedButtonFill = Color.FromArgb(0x18, 0x38, 0x9E);
            CheckedButtonFillHot = Color.FromArgb(0x0F, 0x3A, 0xBF);

        }


        #endregion

        #region Properties

        private Color _checkedGlowHot;
        public Color CheckedGlowHot
        {
            get { return _checkedGlowHot; }
            set { _checkedGlowHot = value; }
        }


        private Color _checkedButtonFillHot;
        public Color CheckedButtonFillHot
        {
            get { return _checkedButtonFillHot; }
            set { _checkedButtonFillHot = value; }
        }


        private Color _checkedButtonFill;
        public Color CheckedButtonFill
        {
            get { return _checkedButtonFill; }
            set { _checkedButtonFill = value; }
        }


        private Color _checkedGlow;
        public Color CheckedGlow
        {
            get { return _checkedGlow; }
            set { _checkedGlow = value; }
        }


        private Color _menuText;
        public Color MenuText
        {
            get { return _menuText; }
            set { _menuText = value; }
        }


        private Color _separatorNorth;
        public Color SeparatorNorth
        {
            get { return _separatorNorth; }
            set { _separatorNorth = value; }
        }


        private Color _separatorSouth;
        public Color SeparatorSouth
        {
            get { return _separatorSouth; }
            set { _separatorSouth = value; }
        }


        private Color _menuLight;
        public Color MenuLight
        {
            get { return _menuLight; }
            set { _menuLight = value; }
        }


        private Color _menuDark;
        public Color MenuDark
        {
            get { return _menuDark; }
            set { _menuDark = value; }
        }


        private Color _menuBackground;
        public Color MenuBackground
        {
            get { return _menuBackground; }
            set { _menuBackground = value; }
        }


        private Color _menuHighlightSouth;
        public Color MenuHighlightSouth
        {
            get { return _menuHighlightSouth; }
            set { _menuHighlightSouth = value; }
        }


        private Color _menuHighlightNorth;
        public Color MenuHighlightNorth
        {
            get { return _menuHighlightNorth; }
            set { _menuHighlightNorth = value; }
        }


        private Color _menuHighlight;
        public Color MenuHighlight
        {
            get { return _menuHighlight; }
            set { _menuHighlight = value; }
        }

        private Color _dropDownArrow;

        /// <summary>
        /// Gets or sets the color for the dropwown arrow
        /// </summary>
        public Color DropDownArrow
        {
            get { return _dropDownArrow; }
            set { _dropDownArrow = value; }
        }


        private Color _buttonFillSouthPressed;

        /// <summary>
        /// Gets or sets the south color of the button fill when pressed
        /// </summary>
        public Color ButtonFillSouthPressed
        {
            get { return _buttonFillSouthPressed; }
            set { _buttonFillSouthPressed = value; }
        }

        private Color _buttonFillSouth;

        /// <summary>
        /// Gets or sets the south color of the button fill
        /// </summary>
        public Color ButtonFillSouth
        {
            get { return _buttonFillSouth; }
            set { _buttonFillSouth = value; }
        }

        private Color _buttonInnerBorderPressed;

        /// <summary>
        /// Gets or sets the color of the inner border when pressed
        /// </summary>
        public Color ButtonInnerBorderPressed
        {
            get { return _buttonInnerBorderPressed; }
            set { _buttonInnerBorderPressed = value; }
        }

        private Color _glow;

        /// <summary>
        /// Gets or sets the glow color
        /// </summary>
        public Color Glow
        {
            get { return _glow; }
            set { _glow = value; }
        }

        private Color _buttonFillNorth;

        /// <summary>
        /// Gets or sets the buttons fill color
        /// </summary>
        public Color ButtonFillNorth
        {
            get { return _buttonFillNorth; }
            set { _buttonFillNorth = value; }
        }

        private Color _buttonFillNorthPressed;

        /// <summary>
        /// Gets or sets the buttons fill color when pressed
        /// </summary>
        public Color ButtonFillNorthPressed
        {
            get { return _buttonFillNorthPressed; }
            set { _buttonFillNorthPressed = value; }
        }

        private Color _buttonInnerBorder;

        /// <summary>
        /// Gets or sets the buttons inner border color
        /// </summary>
        public Color ButtonInnerBorder
        {
            get { return _buttonInnerBorder; }
            set { _buttonInnerBorder = value; }
        }

        private Color _buttonBorder;

        /// <summary>
        /// Gets or sets the buttons border color
        /// </summary>
        public Color ButtonBorder
        {
            get { return _buttonBorder; }
            set { _buttonBorder = value; }
        }

        private Color _buttonOuterBorder;

        /// <summary>
        /// Gets or sets the buttons outer border color
        /// </summary>
        public Color ButtonOuterBorder
        {
            get { return _buttonOuterBorder; }
            set { _buttonOuterBorder = value; }
        }

        private Color _text;

        /// <summary>
        /// Gets or sets the color of the text
        /// </summary>
        public Color Text
        {
            get { return _text; }
            set { _text = value; }
        }

        private Color _backgroundGlow;

        /// <summary>
        /// Gets or sets the background glow color
        /// </summary>
        public Color BackgroundGlow
        {
            get { return _backgroundGlow; }
            set { _backgroundGlow = value; }
        }

        private Color _backgroundBorder;

        /// <summary>
        /// Gets or sets the color of the background border
        /// </summary>
        public Color BackgroundBorder
        {
            get { return _backgroundBorder; }
            set { _backgroundBorder = value; }
        }

        private Color _backgroundNorth;

        /// <summary>
        /// Background north part
        /// </summary>
        public Color BackgroundNorth
        {
            get { return _backgroundNorth; }
            set { _backgroundNorth = value; }
        }

        private Color _backgroundSouth;

        /// <summary>
        /// Background south color
        /// </summary>
        public Color BackgroundSouth
        {
            get { return _backgroundSouth; }
            set { _backgroundSouth = value; }
        }

        private Color _glossyEffectNorth;

        /// <summary>
        /// Gets ors sets the glossy effect north color
        /// </summary>
        public Color GlossyEffectNorth
        {
            get { return _glossyEffectNorth; }
            set { _glossyEffectNorth = value; }
        }

        private Color _glossyEffectSouth;

        /// <summary>
        /// Gets or sets the glossy effect south color
        /// </summary>
        public Color GlossyEffectSouth
        {
            get { return _glossyEffectSouth; }
            set { _glossyEffectSouth = value; }
        }

        #endregion
    }
}
