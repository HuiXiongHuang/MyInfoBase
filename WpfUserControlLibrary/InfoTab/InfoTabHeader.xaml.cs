using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFUserControlLibrary.InfoTab
{
    /// <summary>
    /// InfoTabHeader.xaml 的交互逻辑
    /// </summary>
    public partial class InfoTabHeader : StackPanel
    {
        public InfoTabHeader(String headerText)
        {
            InitializeComponent();
            TabHeaderText.Text = headerText;
        }

        public InfoTabHeader()
        {
            InitializeComponent();
            TabHeaderText.Text = "选项卡";
        }
        /// <summary>
        /// 引用包容此对象的TabItem对象
        /// </summary>
       // private TabItem Parent;

        /// <summary>
        /// 代表选项卡的标题文字
        /// </summary>
        public String HeaderText
        {
            get
            {
                return TabHeaderText.Text;
            }
            set
            {
                TabHeaderText.Text = value;
            }
        }
        /// <summary>
        /// 当代表关闭的小图标被点击时，会调用此方法
        /// </summary>
        public Action<TabItem> onClose = null;
        private void imgCloseTabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (onClose != null)
            {
                TabItem p = GetParentObject<TabItem>(this);//应为使用了样式导致this的父元素不再直接为TabItem
                onClose(p);
            }
        }

        public T GetParentObject<T>(DependencyObject obj) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                if (parent is T )
                {
                    return (T)parent;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }
    }
}
