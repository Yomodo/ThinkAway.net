using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing.Printing;
using ThinkAway.Core;
using Convert = System.Convert;
using Margins = System.Drawing.Printing.Margins;

/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Controls
{
    /// <summary>
    /// ListView 的所见即所得的打印支持类。
    /// </summary>
    public class ListView : System.Windows.Forms.ListView
    {
        /// <summary>
        /// 指示是否进行打印预览，默认值为 true
        /// </summary>
        private bool _isPreview;
        /// <summary>
        /// 指示是否总是打印标题，默认值为 true
        /// </summary>
        private bool _isAlwaysPrintHeader;
        /// <summary>
        /// 需要打印的页首字符串
        /// </summary>
        private string _printHeaderString;
        /// <summary>
        /// 页首字体
        /// </summary>
        private Font _headerFont;
        /// <summary>
        /// 正文字体
        /// </summary>
        private Font _bodyFont;
        /// <summary>
        /// 页尾字体
        /// </summary>
        private Font _tailFont;
        /// <summary>
        /// 正文部分的 ColumnHeader 字体，该字体为正文字体的加粗形式
        /// </summary>
        private Font _columnHeaderFont;
        /// <summary>
        /// 打印文档
        /// </summary>
        private PrintDocument _printDoc;
        /// <summary>
        /// 行间距
        /// </summary>
        private int _lineSpace;
        /// <summary>
        /// 打印区域宽度
        /// </summary>
        private int _printWidth;
        /// <summary>
        /// 打印区域长度
        /// </summary>
        private int _printHeight;
        /// <summary>
        ///  打印页数
        /// </summary>
        private int _pageCount;
        /// <summary>
        /// 当前正在打印的页码
        /// </summary>
        private int _curPrintPage;
        /// <summary>
        /// 共有的页数
        /// </summary>
        private int _totalPage;
        /// <summary>
        /// 用户选择的打印起始页
        /// </summary>
        private int _fromPage;
        /// <summary>
        /// 当前正在打印的元素
        /// </summary>
        private int _curPrintItem;
        /// <summary>
        /// 打印页首的区域
        /// </summary>
        private Rectangle _oHeaderRect;
        /// <summary>
        /// 打印正文的区域
        /// </summary>
        private Rectangle _oBodyRect;
        /// <summary>
        /// 打印页尾的区域
        /// </summary>
        private Rectangle _oTailRect;
        /// <summary>
        /// 默认画刷
        /// </summary>
        private readonly Brush _defaultBrush;
        /// <summary>
        /// 默认画笔
        /// </summary>
        private readonly Pen _defaultPen;
        /// <summary>
        ///  设置或获取是否进行打印预览
        /// </summary>
        public bool IsPreview
        {
            get { return _isPreview; }
            set { _isPreview = value; }
        }

        /// <summary>
        /// 设置或获取是否总是打印标题
        /// </summary>
        public bool IsAlwaysPrintHeader
        {
            get { return _isAlwaysPrintHeader; }
            set { _isAlwaysPrintHeader = value; }
        }

        /// <summary>
        /// 设置或获取打印的页首字符串
        /// </summary>
        public string PrintHeaderString
        {
            get { return _printHeaderString; }
            set { if (value != null) _printHeaderString = value; }
        }

        /// <summary>
        /// 设置或获取页首字体
        /// </summary>
        public Font HeaderFont
        {
            set { _headerFont = value; }
            get { return _headerFont; }
        }

        /// <summary>
        /// 设置或获取正文字体
        /// 如果对正文字体的 Bold 属性设置为 true ，则发生 Exception 类型的异常
        /// </summary>
        public Font BodyFont
        {
            set
            {
                if (value == null)
                    return;
                if (value.Bold)
                {
                    throw new Exception("正文字体不能进行 [加粗] 设置.");
                }
                _bodyFont = value;
            }
            get { return _bodyFont; }
        }

        /// <summary>
        /// 设置或获取页尾字体
        /// </summary>
        public Font TailFont
        {
            set { _tailFont = value; }
            get { return _tailFont; }
        }

        /// <summary>
        /// 设置或获取行间距
        /// </summary>
        public int LineSpace
        {
            get { return _lineSpace; }
            set
            {
                if (value < 0)
                {
                    _lineSpace = 0;
                }
                _lineSpace = value;
            }
        }



        private bool elv;

        
        /// <summary>
        /// 初始化打印文档的属性
        /// </summary>
        private void InitPrintDocument()
        {
            _printDoc = new PrintDocument();
            _printDoc.DocumentName = _printHeaderString;
            if (String.IsNullOrEmpty(_printDoc.PrinterSettings.PrinterName))
            {
                throw new Exception("未找到默认打印机.");
            }
            _printDoc.PrintPage += PrintPage;
            _printDoc.BeginPrint += BeginPrint;
            _printDoc.EndPrint += EndPrint;
            /* 设置打印字体 */
            if (_headerFont == null)
            {
                _headerFont = new Font("楷体_GB2312", 24, FontStyle.Bold, GraphicsUnit.World);
            }

            if (_bodyFont == null)
            {
                _bodyFont = new Font("楷体_GB2312", 18, FontStyle.Regular, GraphicsUnit.World);
            }

            _columnHeaderFont = new Font(_bodyFont, FontStyle.Bold);

            if (_tailFont == null)
            {
                _tailFont = new Font("楷体_GB2312", 12, FontStyle.Regular, GraphicsUnit.World);
            }
        }

        /// <summary>
        /// 初始化打印纸张设置
        /// </summary>
        /// <param name="margins"></param>
        /// <returns></returns>
        private bool InitPrintPage(out Margins margins)
        {
            margins = null;

            /* 获取当前 listview 的分辨率 */
            Graphics g = this.CreateGraphics();
            float x = g.DpiX;
            g.Dispose();

            /* 显示纸张设置对话框 */
            PageSetupDialog ps = new PageSetupDialog();
            ps.Document = _printDoc;
            if (ps.ShowDialog() == DialogResult.Cancel)
            {
                return false;
            }
            // 根据用户设置的纸张信息计算打印区域，计算结果的单位为 1/100 Inch
            _printWidth = ps.PageSettings.Bounds.Width - ps.PageSettings.Margins.Left -
                          ps.PageSettings.Margins.Right;
            _printHeight = ps.PageSettings.Bounds.Height - ps.PageSettings.Margins.Top -
                           ps.PageSettings.Margins.Bottom;
            margins = ps.PageSettings.Margins;
            if (_printWidth <= 0 || _printHeight <= 0)
            {
                throw new Exception("纸张设置错误.");
            }

            /* 将当前 listview 的各column的长度和转换为英寸，判断打印范围是否越界 */
            int listViewWidth = 0;
            for (int i = 0; i < this.Columns.Count; i++)
            {
                listViewWidth += this.Columns[i].Width;
            }
            if (Convert.ToInt32(listViewWidth / x * 100) > _printWidth)
            {
                throw new Exception("打印内容超出纸张范围 ！\r\n请尝试调整纸张或纸张边距；\r\n或缩小表格各列的宽度。");
            }
            _printDoc.DefaultPageSettings = ps.PageSettings;
            return true;
        }

        /// <summary>
        /// 初始化打印页码设置
        /// </summary>
        /// <param name="margins"></param>
        /// <returns></returns>
        private bool InitPrintPageNumber(Margins margins)
        {

            /* 计算页数 */
            int headerFontHeight = _headerFont.Height;
            int columnHeaderFontHeight = _columnHeaderFont.Height;
            int bodyFontHeight = _bodyFont.Height;
            int tailFontHeight = _tailFont.Height;

            // 页首区域打印 页首字符串 以及一条横线
            _oHeaderRect = new Rectangle(margins.Left, margins.Top, _printWidth, headerFontHeight + _lineSpace + 3);

            int tailHeight = tailFontHeight + _lineSpace + 3;
            // 页尾区域打印 一条横线，页码和打印时间（在同一行）
            _oTailRect = new Rectangle(margins.Left, margins.Top + _printHeight - tailHeight, _printWidth, tailHeight);

            //正文区域为去掉页首和页尾区域的部分
            _oBodyRect = new Rectangle(margins.Left, _oHeaderRect.Bottom + 2, _printWidth,
                                        _oTailRect.Top - _oHeaderRect.Bottom - 4);

            /* 计算第一页能打印的元素个数 */
            int printItemPerPage = 0;
            int firstPageItem =
                Convert.ToInt32((_oBodyRect.Height - columnHeaderFontHeight - _lineSpace) / (bodyFontHeight + _lineSpace));
            if (firstPageItem >= this.Items.Count)
            {
                // 需打印的元素只有一页
                _pageCount = 1;
            }
            else
            {
                // 需要打印的元素有多页
                printItemPerPage = firstPageItem;
                int leftItems = this.Items.Count - firstPageItem;
                if (_isAlwaysPrintHeader == false)
                {
                    printItemPerPage = (_oBodyRect.Height + _oHeaderRect.Height + 2 - columnHeaderFontHeight -
                                        _lineSpace) / (bodyFontHeight + _lineSpace);
                }
                if (leftItems % printItemPerPage == 0)
                {
                    _pageCount = leftItems / printItemPerPage + 1;
                }
                else
                {
                    _pageCount = leftItems / printItemPerPage + 2;
                }
            }
            _totalPage = _pageCount;

            /* 显示打印对话框 */
            PrintDialog pd = new PrintDialog();
            pd.Document = _printDoc;
            pd.PrinterSettings.MinimumPage = 1;
            pd.PrinterSettings.MaximumPage = _pageCount;
            pd.PrinterSettings.FromPage = 1;
            pd.PrinterSettings.ToPage = _pageCount;
            pd.AllowPrintToFile = false;
            pd.AllowSelection = _pageCount > 1;
            pd.AllowSomePages = true;

            if (pd.ShowDialog() == DialogResult.OK)
            {
                _pageCount = pd.PrinterSettings.ToPage - pd.PrinterSettings.FromPage + 1;

                if (pd.PrinterSettings.FromPage > 1)
                {
                    _curPrintItem = firstPageItem + (pd.PrinterSettings.FromPage - 2) * printItemPerPage;
                }
                else
                {
                    _curPrintItem = 0;
                }
                _fromPage = pd.PrinterSettings.FromPage;
                _printDoc.DocumentName = _pageCount.ToString(CultureInfo.InvariantCulture);
                _printDoc.PrinterSettings = pd.PrinterSettings;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 启动 ListView 的打印工作
        /// </summary>
        public virtual void DoPrint()
        {
            if (this.Items.Count <= 0)
            {
                throw new Exception("没有需要打印的元素.");
            }
            InitPrintDocument();

            Margins margins;
            if (InitPrintPage(out margins) == false)
                return;
            if (InitPrintPageNumber(margins) == false)
                return;

            if (_isPreview == false)
            {
                _printDoc.Print();
            }
            else
            {
                PrintPreviewDialog pd = new PrintPreviewDialog();
                pd.Document = _printDoc;
                pd.PrintPreviewControl.Zoom = 1.0;
                pd.WindowState = FormWindowState.Maximized;
                pd.ShowInTaskbar = true;
                pd.ShowDialog();
            }
        }

        /// <summary>
        /// 打印页首
        /// </summary>
        /// <param name="g"></param>
        protected virtual void PrintPageHeader(Graphics g)
        {
            RectangleF textRect = new RectangleF(_oHeaderRect.X, _oHeaderRect.Y, _oHeaderRect.Width,
                                                 _oHeaderRect.Height - 3);
            CenterText(g, _printHeaderString, _headerFont, _defaultBrush, textRect);
            g.DrawLine(_defaultPen, _oHeaderRect.X, _oHeaderRect.Bottom - 2, _oHeaderRect.Right, _oHeaderRect.Bottom - 2);
        }

        /// <summary>
        /// 打印正文
        /// </summary>
        /// <param name="g"></param>
        protected virtual void PrintPageBody(Graphics g)
        {
            Rectangle textRect = _oBodyRect;
            if (_isAlwaysPrintHeader == false && _curPrintPage != 1)
            {
                textRect = new Rectangle(_oHeaderRect.X, _oHeaderRect.Y, _oHeaderRect.Width,
                                         _oHeaderRect.Height + 2 + _oBodyRect.Height);
            }

            /* 打印标题,也就是 columnHeader */
            int columnHeaderFontHeight = _columnHeaderFont.Height;
            int bodyFontHeight = _bodyFont.Height;
            Rectangle columnHeaderRect = new Rectangle(textRect.X, textRect.Y, textRect.Width,
                                                       columnHeaderFontHeight + _lineSpace);
            DrawColumnHeader(g, _columnHeaderFont, _defaultBrush, columnHeaderRect);

            /* 打印元素 */
            int itemHeight = bodyFontHeight + _lineSpace;
            int yPos = columnHeaderRect.Bottom;
            int printItemPerPage = (textRect.Height - columnHeaderRect.Height) / (bodyFontHeight + _lineSpace);

            for (int i = 0; (i < printItemPerPage) && (_curPrintItem < this.Items.Count); i++)
            {
                Rectangle itemRect = new Rectangle(textRect.X, yPos, textRect.Width, itemHeight);
                DrawItem(g, _bodyFont, _defaultBrush, itemRect);
                _curPrintItem++;
                yPos += itemHeight;
            }
        }

        /// <summary>
        /// 打印页尾
        /// </summary>
        /// <param name="g"></param>
        protected virtual void PrintPageTail(Graphics g)
        {
            g.DrawLine(_defaultPen, _oTailRect.X, _oTailRect.Top + 1, _oTailRect.Right, _oTailRect.Top + 1);

            RectangleF textRect = new RectangleF(_oTailRect.X, _oTailRect.Y + 3, _oTailRect.Width, _oTailRect.Height - 3);
            string text = string.Format("第 {0} 页  共 {1} 页", _fromPage, _totalPage);
            _fromPage++;
            CenterText(g, text, _tailFont, _defaultBrush, textRect);

            string time = string.Format("打印时间：{0}   ", DateTime.Now.ToLongDateString());
            RightText(g, time, _tailFont, _defaultBrush, textRect);
        }

        /// <summary>
        /// 打印一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.PageUnit = GraphicsUnit.Inch;
            e.Graphics.PageScale = .01f;

            if (_isAlwaysPrintHeader)
            {
                PrintPageHeader(e.Graphics);
            }
            else
            {
                if (_curPrintPage == 1)
                {
                    PrintPageHeader(e.Graphics);
                }
            }

            PrintPageBody(e.Graphics);
            PrintPageTail(e.Graphics);

            e.HasMorePages = _curPrintPage != _pageCount;
            _curPrintPage++;

        }

        /// <summary>
        /// 打印前的初始化设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeginPrint(object sender, PrintEventArgs e)
        {
            _curPrintPage = 1;
        }

        /// <summary>
        /// 打印结束后的资源释放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void EndPrint(object sender, PrintEventArgs e)
        {
        }

        /// <summary>
        /// 居中显示文本信息
        /// </summary>
        /// <param name="g"></param>
        /// <param name="t"></param>
        /// <param name="f"></param>
        /// <param name="b"></param>
        /// <param name="rect"></param>
        private void CenterText(Graphics g, string t, Font f, Brush b, RectangleF rect)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            g.DrawString(t, f, b, rect, sf);
        }

        /// <summary>
        /// 居右显示文本信息
        /// </summary>
        /// <param name="g"></param>
        /// <param name="t"></param>
        /// <param name="f"></param>
        /// <param name="b"></param>
        /// <param name="rect"></param>
        private void RightText(Graphics g, string t, Font f, Brush b, RectangleF rect)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            sf.LineAlignment = StringAlignment.Center;
            g.DrawString(t, f, b, rect, sf);
        }

        /// <summary>
        /// 居左显示文本信息
        /// </summary>
        /// <param name="g"></param>
        /// <param name="t"></param>
        /// <param name="f"></param>
        /// <param name="b"></param>
        /// <param name="rect"></param>
        protected void LeftText(Graphics g, string t, Font f, Brush b, RectangleF rect)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            g.DrawString(t, f, b, rect, sf);
        }

        /// <summary>
        /// 打印 listview 的 columnheader
        /// </summary>
        /// <param name="g"></param>
        /// <param name="f"></param>
        /// <param name="b"></param>
        /// <param name="rect"></param>
        private new void DrawColumnHeader(Graphics g, Font f, Brush b, Rectangle rect)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            Point location = new Point(rect.Location.X, rect.Location.Y);

            for (int i = 0; i < this.Columns.Count; i++)
            {
                Size itemSize = new Size(this.Columns[i].Width, rect.Height);
                Rectangle itemRect = new Rectangle(location, itemSize);

                g.DrawRectangle(_defaultPen, itemRect);
                g.DrawString(this.Columns[i].Text, f, b, itemRect, sf);

                location.X += this.Columns[i].Width;
            }
        }
        /// <summary>
        /// 打印 listview 的 item
        /// </summary>
        /// <param name="g"></param>
        /// <param name="f"></param>
        /// <param name="b"></param>
        /// <param name="rect"></param>
        private new void DrawItem(Graphics g, Font f, Brush b, Rectangle rect)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;

            Point location = new Point(rect.Location.X, rect.Location.Y);

            for (int i = 0; i < this.Columns.Count; i++)
            {
                Size itemSize = new Size(this.Columns[i].Width, rect.Height);
                Rectangle itemRect = new Rectangle(location, itemSize);

                g.DrawRectangle(_defaultPen, itemRect);
                g.DrawString(this.Items[_curPrintItem].SubItems[i].Text, f, b, itemRect, sf);

                location.X += this.Columns[i].Width;
            }
        }

         #region 变量
        /// <summary>
        /// 当前显示列表
        /// </summary>
        private System.Windows.Forms.Control _currentCtrl;

        /// <summary>
        /// 存储需要显示的控件列表
        /// </summary>
        private readonly SortedList<int, object> _mCtrls;
        /// <summary>
        /// 全局被选中的列索引
        /// </summary>
        int _columnIndex = 99999;

        #endregion
        #region 方法
        /// <summary>
        /// 添加需要显示的控件
        /// </summary>
        /// <param name="ctr">显示的控件(目前只支持textbox和combox)</param>
        /// <param name="columnIndex">在listview中的列索引</param>
        public void AddDisControl(System.Windows.Forms.Control ctr, int columnIndex)
        {
            if (_mCtrls.ContainsKey(columnIndex))
            {
                //MessageBox.Show("已经包含所选的" + ctr.ToString());
                return;
            }
            ctr.Visible = true;
            _mCtrls.Add(columnIndex, ctr);
            this.Controls.Add(ctr);
        }
        private void EditItem(int index, ListViewItem.ListViewSubItem sb)
        {
            if (this.SelectedItems.Count <= 0)
            {
                return;
            }
            int currentSelectColumnIndex = _mCtrls.IndexOfKey(index);
            if (currentSelectColumnIndex == -1)
            {
                return;
            }
            _currentCtrl = (System.Windows.Forms.Control)_mCtrls.Values[currentSelectColumnIndex];
            ListViewItem item = this.SelectedItems[0];
            Rectangle rect = sb.Bounds;
            Rectangle _rect = new Rectangle(rect.Right - this.Columns[index].Width, rect.Top, this.Columns[index].Width, rect.Height);
            _currentCtrl.Bounds = _rect;
            _currentCtrl.BringToFront();
            _currentCtrl.Text = item.SubItems[index].Text;
            _currentCtrl.TextChanged += CtrTextChanged;
            _currentCtrl.Leave += CtrLeave;

            _currentCtrl.Visible = true;
            _currentCtrl.Tag = item;
            _currentCtrl.Select();
        }
        #endregion
        #region 构造函数
        public ListView()
        {
            _isPreview = true;
            _isAlwaysPrintHeader = true;
            _printHeaderString = "";
            _headerFont = null;
            _tailFont = null;
            _bodyFont = null;
            _columnHeaderFont = null;
            _printDoc = null;

            _printWidth = 0;
            _printHeight = 0;
            _pageCount = 0;
            _curPrintPage = 1;
            _fromPage = 1;
            _lineSpace = 0;
            _curPrintItem = 0;

            _defaultBrush = Brushes.Black;
            _defaultPen = new Pen(_defaultBrush, 1);




            //
            _mCtrls = null;
            _currentCtrl = null;
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
             _mCtrls = new SortedList<int,object>();
         }
        #endregion
        #region 重载事件
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                Point tmpPoint = this.PointToClient(Cursor.Position);
                ListViewItem item = this.GetItemAt(tmpPoint.X, tmpPoint.Y);
                if (item != null)
                {
                    ListViewItem.ListViewSubItem sb = item.GetSubItemAt(tmpPoint.X, tmpPoint.Y);
                    _columnIndex = item.SubItems.IndexOf(sb);
                    if (tmpPoint.X > this.Columns[0].Width && tmpPoint.X < this.Width)
                    {
                        EditItem(_columnIndex, sb);
                    }
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (this._currentCtrl != null)
            {
                this._currentCtrl.Visible = false;
            }
            base.OnSelectedIndexChanged(e);
        }

        protected override void OnClick(EventArgs e)
        {
            if (this.SelectedItems.Count == 0)
            {
                return;
            }
            Point tmpPoint = this.PointToClient(Cursor.Position);
            ListViewItem item = this.GetItemAt(tmpPoint.X, tmpPoint.Y);
            if (this.SelectedItems.Count != 0 && item == null)
            {
                item = this.SelectedItems[0];
            }
            if (item != null)
            {
                ListViewItem.ListViewSubItem sb = item.GetSubItemAt(tmpPoint.X, tmpPoint.Y);
                _columnIndex = item.SubItems.IndexOf(sb);
                if (tmpPoint.X > this.Columns[0].Width && tmpPoint.X < this.Width)
                {
                    EditItem(_columnIndex, sb);
                }
            }
            base.OnDoubleClick(e);
        }

        protected override void WndProc(ref   Message m)
        {
            if (m.Msg == 0x115 )
            {
                if (_currentCtrl != null)
                {
                    _currentCtrl.Visible = false;
                }
            }
            if ( m.Msg == 0x114)
            {
                if (_currentCtrl != null)
                {
                    _currentCtrl.Visible = false;
                }
            }

            if ((m.Msg == 15) && !this.elv)
            {
                Win32API.SetWindowTheme(Handle, "explorer", null);
                Win32API.SendMessage(Handle, 0x1036, 0x10000, 0x10000);
                this.elv = true;
            }

            base.WndProc(ref   m);
        }
        #endregion
        #region 控件事件
        private void CtrLeave(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                (control).TextChanged -= CtrTextChanged;
                (sender as System.Windows.Forms.Control).Visible = false;
            }
        }

        public void CtrSelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem listViewItem = ((System.Windows.Forms.Control) sender).Tag as ListViewItem;
            if (listViewItem != null)
            {
                (listViewItem).SubItems[_columnIndex].Text = ((System.Windows.Forms.Control)sender).Text;
            }
        }

        private void CtrTextChanged(object sender, EventArgs e)
        {
            ListViewItem listViewItem = ((System.Windows.Forms.Control) sender).Tag as ListViewItem;
            if (listViewItem != null)
            {
                (listViewItem).SubItems[_columnIndex].Text = ((System.Windows.Forms.Control)sender).Text;
            }
        }

        #endregion
        #region listview进度条显示
        //C# listview进度条显示
        private Color _mProgressColor = Color.Red;

        public Color ProgressColor
        {
            get
            {
                return this._mProgressColor;
            }
            set
            {
                this._mProgressColor = value;
            }
        }

        private Color _mProgressTextColor = Color.Black;

        public Color ProgressTextColor
        {
            get
            {
                return _mProgressTextColor;
            }
            set
            {
                _mProgressTextColor = value;
            }
        }
        //C# listview进度条显示
        public int ProgressColumIndex
        {
            set
            {
                _progressIndex = value;
            }
            get
            {
                return _progressIndex;
            }
        }

        int _progressIndex = -1;

        /// <summary>   
        /// 检查是否可以转化为一个浮点数   
        /// </summary>   
        const string Numberstring = "0123456789.";
        private static bool CheckIsFloat(IEnumerable<char> s)
        {
            //NOTE:NET2.0
            //C# listview进度条显示
            foreach (char c in s)
            {
                if (Numberstring.IndexOf(c) > -1) return false;
            }
            return true;
        }

        //C# listview进度条显示

        protected override void OnDrawColumnHeader(
        DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            base.OnDrawColumnHeader(e);
        }

        protected override void OnDrawSubItem(
        DrawListViewSubItemEventArgs e)
        {
            if (e.ColumnIndex != this._progressIndex)
            {   //C# listview进度条显示
                e.DrawDefault = true;
                base.OnDrawSubItem(e);
            }
            else
            {
                if (CheckIsFloat(e.Item.SubItems[e.ColumnIndex].Text))
                //判断当前subitem文本是否可以转为浮点数   
                {
                    float per = float.Parse(e.Item.
                    SubItems[e.ColumnIndex].Text);
                    if (per >= 1.0f)
                    {
                        per = per / 100.0f;
                    }
                    Rectangle rect = new Rectangle(e.Bounds.X,
                     e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                    DrawProgress(rect, per, e.Graphics);
                }
            }
        }
        

        ///绘制进度条列的subitem  

        private void DrawProgress(Rectangle rect, float percent, Graphics g)
        {
            if (rect.Height > 2 && rect.Width > 2)
            {
                if ((rect.Top > 0 && rect.Top < this.Height) && (rect.Left > this.Left && rect.Left < this.Width))
                {
                    //绘制进度   
                    int width = (int)(rect.Width * percent);
                    Rectangle newRect = new Rectangle(rect.Left + 1,
                    rect.Top + 1, width - 2, rect.Height - 2);
                    using (Brush tmpb =
                    new SolidBrush(this._mProgressColor))
                    {
                        g.FillRectangle(tmpb, newRect);
                    }

                    newRect = new Rectangle(rect.Left +
                     1, rect.Top + 1, rect.Width - 2,
                     rect.Height - 2);
                    g.DrawRectangle(Pens.RoyalBlue, newRect);

                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.EllipsisCharacter;

                    newRect = new Rectangle(rect.Left + 1,
                    rect.Top + 1, rect.Width - 2,
                    rect.Height - 2);

                    using (Brush b =
                    new SolidBrush(_mProgressTextColor))
                    {
                        g.DrawString(
                        percent.ToString("p1"), this.Font, b, newRect, sf);
                    }
                }
            }
            //C# listview进度条显示
        }
        #endregion
    }
}
