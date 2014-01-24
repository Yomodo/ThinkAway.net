using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Controls
{
    public enum TabState
    {
        Actived,
        Deactivated,
        Hover
    }

    public sealed class TabBar : System.Windows.Forms.Control
    {
        /// <summary>
        /// 标签列表
        /// </summary>
        readonly List<TabButton> _tabList = new List<TabButton>();

        /// <summary>
        /// 活动标签栈，栈顶为当前被激活的标签
        /// </summary>
        readonly Stack<TabButton> _activedTabStack = new Stack<TabButton>();

        readonly ToolTip _toolTip = new ToolTip();

        private int _tabMinWidth = 10;
        private int _tabMaxWidth = 150;

        private bool _enableClose;

        public bool EnableClose
        {
            get { return _enableClose; }
            set { _enableClose = value; }
        }


        private Color _activedTabColor = Color.Black;
        private Color _deactivatedTabColor = Color.Black;

        /// <summary>
        /// 获取或设置活动标签的文本色
        /// </summary>
        public Color ActivedTabColor
        {
            get
            {
                return _activedTabColor;
            }
            set
            {
                if (_activedTabColor != value)
                {
                    _activedTabColor = value;
                    foreach (TabButton t in this._tabList)
                    {
                        t.ActiveColor = value;
                    }
                    this.Invalidate(true);
                }
            }
        }

        /// <summary>
        /// 获取或设置非活动标签的文本色
        /// </summary>
        public Color DeactivatedTabColor
        {
            get
            {
                return _deactivatedTabColor;
            }
            set
            {
                if (_deactivatedTabColor != value)
                {
                    _deactivatedTabColor = value;
                    foreach (TabButton t in this._tabList)
                    {
                        t.DeactivatedColor = value;
                    }
                    this.Invalidate(true);
                }
            }
        }


        /// <summary>
        /// 获取或设置标签的最小宽度
        /// </summary>
        [DefaultValue("10")]
        public int TabMinWidth
        {
            get
            {
                return _tabMinWidth;
            }
            set
            {
                if (value > 0)
                {
                    if (_tabMinWidth != value)
                    {
                        _tabMinWidth = value;
                        this.Invalidate(true);
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置标签的最大宽度
        /// </summary>
        [DefaultValue("150")]
        public int TabMaxWidth
        {
            get
            {
                return _tabMaxWidth;
            }
            set
            {
                if (value > 0)
                {
                    if (_tabMaxWidth != value)
                    {
                        _tabMaxWidth = value;
                        this.Invalidate(true);
                    }
                }
            }
        }


        public TabBar()
        {
            this.LoadImage();
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// 获取标签列表
        /// </summary>
        [Browsable(false)]
        public List<TabButton> TabList
        {
            get
            {
                return this._tabList;
            }
        }

        /// <summary>
        /// 获取或设置当前被激活的标签
        /// </summary>
        [Browsable(false)]
        public TabButton ActivedTab
        {
            get
            {
                if (this._activedTabStack.Count > 0)
                {
                    return _activedTabStack.Peek();
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    if (!this.Controls.Contains(value))
                    {
                        this.AddTab(value, true);
                    }
                    else
                    {
                        value.State = TabState.Actived;
                    }
                }
            }
        }


        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="tab">标签</param>
        /// <param name="actived">是否立即激活</param>
        public void AddTab(TabButton tab, bool actived)
        {
            this.AddTab(this._tabList.Count, tab, actived);
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="index">在该位置插入</param>
        /// <param name="tab">标签</param>
        ///<param name="actived">是否立即激活</param>
        public void AddTab(int index, TabButton tab, bool actived)
        {
            index = Math.Min(Math.Max(0, index), this._tabList.Count);

            tab.Selected += TabSelected;
            tab.DoubleClick += TabDoubleClick;
            tab.Disposed += TabDisposed;
            tab.ActiveColor = this._activedTabColor;
            tab.DeactivatedColor = this._deactivatedTabColor;
            tab.MouseEnter += TabMouseEnter;
            tab.MouseLeave += TabMouseLeave;
            this._tabList.Insert(index, tab);
            this.Controls.Add(tab);
            if (index == 0)
            {
                actived = true;
            }
            if (actived)
            {
                tab.State = TabState.Actived;
            }

            this.OnTabAdded(tab, index);
        }

        void TabMouseLeave(object sender, EventArgs e)
        {
            TabButton t = sender as TabButton;
            if (t != null)
            {
                this._toolTip.Hide(t);
            }
        }

        void TabMouseEnter(object sender, EventArgs e)
        {
            TabButton t = sender as TabButton;
            if (t != null && !string.IsNullOrEmpty(t.Text))
            {
                this._toolTip.SetToolTip(t, t.Text);
            }
        }

        public void RemoveTab(TabButton tab)
        {
            this._tabList.Remove(tab);
            this.Controls.Remove(tab);
            this.OnTabRemoved(tab);

            tab.Hide();
            tab.Dispose();
        }

        /// <summary>
        /// 被选择的标签改变时发生
        /// </summary>
        public event EventHandler<TabSelectedEventArgs> SelectedItemChanged;

        /// <summary>
        /// 当有新标签被添加时发生
        /// </summary>
        public event EventHandler<TabAddedEventArgs> TabAdded;

        /// <summary>
        /// 当有标签被移除时发生
        /// </summary>
        public event EventHandler<TabRemovedEventArgs> TabRemoved;

        private void OnSelectedItemChanged(TabSelectedEventArgs arg)
        {
            if (this.SelectedItemChanged != null)
            {
                this.SelectedItemChanged(this, arg);
            }
        }
        private void OnTabAdded(TabButton tab, int index)
        {
            if (this.TabAdded != null)
            {
                this.TabAdded(this, new TabAddedEventArgs(tab, this, index));
            }

            this.Invalidate(true);
        }

        private void OnTabRemoved(TabButton tabRemoved)
        {
            if (this.TabRemoved != null)
            {
                this.TabRemoved(this, new TabRemovedEventArgs(tabRemoved, this));
            }
        }

        void TabDisposed(object sender, EventArgs e)
        {
            TabButton t = sender as TabButton;

            if (t != null)
            {
                this._tabList.Remove(t);
                if (this._activedTabStack.Count > 0 && t.Equals(this._activedTabStack.Peek()))
                {
                    this._activedTabStack.Pop();
                }

                if (this._activedTabStack.Count > 0)
                {
                    while (true)
                    {
                        TabButton t2 = this._activedTabStack.Peek();
                        if (t2 != null && !t2.Disposing)
                        {
                            t2.State = TabState.Actived;
                            break;
                        }
                        this._activedTabStack.Pop();
                        if (this._activedTabStack.Count > 0)
                        {
                            continue;
                        }
                        break;
                    }
                }
                else
                {
                    if (this._tabList.Count > 0)
                    {
                        this._tabList[this._tabList.Count - 1].State = TabState.Actived;
                    }
                }
            }

            #region 确保有一个被选中
            bool hasOneActived = false;
            foreach (TabButton ta in this._tabList)
            {
                if (ta.State == TabState.Actived)
                {
                    hasOneActived = true;
                    break;
                }
            }
            if (!hasOneActived && this._tabList.Count > 0)
            {
                this._tabList[this._tabList.Count - 1].State = TabState.Actived;
            }
            #endregion
        }

        void TabDoubleClick(object sender, EventArgs e)
        {
            if (_enableClose)
            {
                TabButton t = sender as TabButton;
                if (t != null)
                {
                    this.RemoveTab(t);
                }
            }
        }

        void TabSelected(object sender, EventArgs e)
        {
            TabButton t = sender as TabButton;

            for (int i = 0; i < this._tabList.Count; i++)
            {
                if (!this._tabList[i].Equals(t))
                {
                    this._tabList[i].State = TabState.Deactivated;
                }else
                {
                    TabSelectedEventArgs args = new TabSelectedEventArgs(i,t);
                    OnSelectedItemChanged(args);
                }
            }

            this._activedTabStack.Push(t);
        }
        private void LoadImage()
        {
            //this.BackgroundImage = Properties.Resources.TABBAR_BG;
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int x = 0;
            int y = 2;
            int width = Math.Max(this._tabMinWidth, Math.Min((this.Width) / (this._tabList.Count + 1), this._tabMaxWidth));
            int height = this.Height - 2;
            for (int i = 0; i < this._tabList.Count; i++)
            {
                _tabList[i].Location = new Point(x, y);
                _tabList[i].Size = new Size(width, height);
                x += _tabList[i].ClientSize.Width;
            }
            base.OnPaint(e);
        }
    }

    /// <summary>
    /// 标签
    /// </summary>
    public sealed class TabButton : System.Windows.Forms.Control
    {
        public TabButton()
        {
            this.LoadImage();
            this.DoubleBuffered = true;
        }

        public new string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (!base.Text.Equals(value))
                {
                    base.Text = value;
                    this.Invalidate();
                }
            }
        }
        public TabButton(string text)
        {
            this.Text = text;
            this.LoadImage();
            this.DoubleBuffered = true;
        }

        private TabState _state = TabState.Deactivated;
        private System.Drawing.Image _activedBackgroundImage = null;
        private System.Drawing.Image _deactivatedBackgroundImage = null;
        private System.Drawing.Image _mouseHoverBackgroundImage = null;

        private Color _activedColor = Color.Black;
        private Color _deactivatedColor = Color.Black;

        /// <summary>
        /// 获取或设置标签被激活时其文本色
        /// </summary>
        public Color ActiveColor
        {
            get
            {
                return _activedColor;
            }
            set
            {
                if (_activedColor != value)
                {
                    _activedColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// 获取或设置标签未被激活时其文本色
        /// </summary>
        public Color DeactivatedColor
        {
            get
            {
                return _deactivatedColor;
            }
            set
            {
                if (_deactivatedColor != value)
                {
                    _deactivatedColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// 获取或设置当前标签的状态
        /// </summary>
        public TabState State
        {
            get
            {
                return this._state;
            }
            set
            {
                if (this._state != value)
                {
                    this._state = value;
                    if (value == TabState.Actived)
                    {
                        this.OnSelected();
                    }
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// 当标签被选取（激活）时发生
        /// </summary>
        public event EventHandler<EventArgs> Selected;

        private void OnSelected()
        {
            if (this.Selected != null)
            {
                this.Selected(this,EventArgs.Empty);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            switch (this._state)
            {
                case TabState.Actived:
                    this.BackgroundImage = this._activedBackgroundImage;
                    break;
                case TabState.Deactivated:
                    this.BackgroundImage = this._deactivatedBackgroundImage;
                    break;
                case TabState.Hover:
                    this.BackgroundImage = this._mouseHoverBackgroundImage;
                    break;
                default:
                    break;
            }

            //绘制文本
            if (!String.IsNullOrEmpty(this.Text))
            {
                string txt = this.Text;
                Font txtFont = this.Font;
                SizeF txtSize = e.Graphics.MeasureString(this.Text, txtFont);
                float y = (this.Height - txtSize.Height) / 2;
                float x = 10;

                if (txtSize.Width > this.Size.Width - 3 * x)
                {
                    for (int i = this.Text.Length - 1; i >= 0; i--)
                    {
                        txt = this.Text.Substring(0, i);
                        if (e.Graphics.MeasureString(txt, txtFont).Width <= this.Size.Width - 3 * x)
                        {
                            break;
                        }
                    }
                    txt += "...";
                }
                Brush txtBrush = new SolidBrush(this._state == TabState.Actived ? this._activedColor : this.DeactivatedColor);
                e.Graphics.DrawString(txt, txtFont, txtBrush, new PointF(x, y));
            }

            base.OnPaint(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                this.State = TabState.Actived;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (this._state == TabState.Deactivated)
            {
                this._state = TabState.Hover;
                this.Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (this._state != TabState.Actived)
            {
                this._state = TabState.Deactivated;
            }
            this.Invalidate();
        }

        private void LoadImage()
        {
            //this._activedBackgroundImage = Properties.Resources.TAB_ACTIVE;
            //this._deactivatedBackgroundImage = Properties.Resources.TAB_INACTIVE;
            //this._mouseHoverBackgroundImage = Properties.Resources.TAB_HOVER;

            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

    }
    public class TabBarSelectedItemChangedArgs : EventArgs
    {
        private readonly TabButton _oldItem;
        private readonly TabButton _newItem;

        /// <summary>
        /// 获取事件发生之前被选中的项
        /// </summary>
        public TabButton OldItem
        {
            get
            {
                return _oldItem;
            }
        }

        /// <summary>
        /// 获取事件发生之后被选中的项
        /// </summary>
        public TabButton NewItem
        {
            get
            {
                return _newItem;
            }
        }

        public TabBarSelectedItemChangedArgs(TabButton oldItem, TabButton newItem)
        {
            this._oldItem = oldItem;
            this._newItem = newItem;
        }
    }
    public class TabSelectedEventArgs : EventArgs
    {
        private readonly int _index;
        private readonly TabButton _sender;

        public TabSelectedEventArgs(int index, TabButton sender)
        {
            _index = index;
            _sender = sender;
        }


        public int Index
        {
            get { return _index; }
        }

        public TabButton TabButton
        {

            get { return _sender; }
        }
    }
    public class TabAddedEventArgs : EventArgs
    {
        private readonly TabButton _tabAdded;
        private readonly TabBar _sender;
        private readonly int _index;

        /// <summary>
        /// 获取被添加的标签
        /// </summary>
        public TabButton TabAdded
        {
            get
            {
                return _tabAdded;
            }
        }

        /// <summary>
        /// 获取标签被添加到的标签栏
        /// </summary>
        public TabBar Sender
        {
            get
            {
                return _sender;
            }
        }
        /// <summary>
        /// 获取标签被添加后其在标签栏中的索引
        /// </summary>
        public int Index
        {
            get
            {
                return _index;
            }
        }

        public TabAddedEventArgs(TabButton tabAdded, TabBar sender, int index)
        {
            this._tabAdded = tabAdded;
            this._sender = sender;
            this._index = index;
        }
    }

    public class TabRemovedEventArgs : EventArgs
    {
        private readonly TabButton _tabRemoved;
        private readonly TabBar _sender;

        /// <summary>
        /// 获取被移除的标签
        /// </summary>
        public TabButton TabRemoved
        {
            get
            {
                return _tabRemoved;
            }
        }

        /// <summary>
        /// 获取标签被移除时所在的标签栏
        /// </summary>
        public TabBar Sender
        {
            get
            {
                return _sender;
            }
        }

        public TabRemovedEventArgs(TabButton tabRemoved, TabBar sender)
        {
            this._tabRemoved = tabRemoved;
            this._sender = sender;
        }
    }

    [ToolboxBitmap(typeof(System.Windows.Forms.TabControl))]
    public class TabControl : System.Windows.Forms.TabControl
    {
        private bool _mHideTabs = false;

        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
        public bool HideTabs
        {
            get { return _mHideTabs; }
            set
            {
                if (_mHideTabs == value) return;
                _mHideTabs = value;
                if (value == true) this.Multiline = true;
                this.UpdateStyles();
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public new bool Multiline
        {
            get
            {
                if (this.HideTabs) return true;
                return base.Multiline;
            }
            set
            {
                if (this.HideTabs)
                    base.Multiline = true;
                else
                    base.Multiline = value;
            }
        }

        private TabBar _tabBar = null;


        [RefreshProperties(RefreshProperties.Repaint)]
        public TabBar TabBar
        {
            get { return _tabBar; }
            set
            {
                _tabBar = value;
            }
        }
        public void SetTabBar(TabBar tabBar)
        {
            if(_tabBar == null)
            {
                _tabBar = new TabBar();
            }
            this.HideTabs = true;
            foreach(TabPage page in this.TabPages)
            {
                TabButton button = new TabButton();
                button.Text = page.Text;
                button.Tag = page.Tag;
                _tabBar.AddTab(button,false);
            }
            _tabBar.SelectedItemChanged += TabBarSelectedItemChanged;
        }
        public TabControl()
        {
            SetTabBar(_tabBar);
        }

        void TabBarSelectedItemChanged(object sender, TabSelectedEventArgs e)
        {
            this.SelectTab(e.Index);
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                if (this.HideTabs)
                    return new Rectangle(0, 0, Width, Height);
                int tabStripHeight, itemHeight;

                if (this.Alignment <= TabAlignment.Bottom)
                    itemHeight = this.ItemSize.Height;
                else
                    itemHeight = this.ItemSize.Width;

                if (this.Appearance == TabAppearance.Normal)
                    tabStripHeight = 5 + (itemHeight * this.RowCount);
                else
                    tabStripHeight = (3 + itemHeight) * this.RowCount;

                switch (this.Alignment)
                {
                    case TabAlignment.Bottom:
                        return new Rectangle(4, 4, Width - 8, Height - tabStripHeight - 4);
                    case TabAlignment.Left:
                        return new Rectangle(tabStripHeight, 4, Width - tabStripHeight - 4, Height - 8);
                    case TabAlignment.Right:
                        return new Rectangle(4, 4, Width - tabStripHeight - 4, Height - 8);
                    default:
                        return new Rectangle(4, tabStripHeight, Width - 8, Height - tabStripHeight - 4);
                }
            }

        }

    }

}
