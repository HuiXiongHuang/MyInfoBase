using InterfaceLibrary;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFSuperRichTextBox;
using WinForms = System.Windows.Forms;

namespace WPFSuperRichTextBox
{
    /// <summary>
    /// EditToolBar.xaml 的交互逻辑
    /// </summary>
    public partial class EditorToolBar : UserControl
    {
        /// <summary>
        /// 用于引用主窗体，节点可以使用此引用调用主窗体所提供的功能
        /// </summary>
        public static IMainWindowFunction _mainWindow = null;
   
        public EditorToolBar()
        {
            InitializeComponent();
            //加载字体下拉列表
             LoadSystemFontsToListBox(cboFontFamilies);
           
        }

        public TextBox InnerFontSizeTextBox
        {
            get { return txtFontSize;}
        }

        #region "变量区"

        private WinForms.OpenFileDialog OpenFileDialog1 = new WinForms.OpenFileDialog();


        private const string ProgramName = "MySuperEditor";

        /// <summary>
        /// 提供对于编辑器功能的基本实现
        /// </summary>
        private RichTextBoxDocumentManager rtfManager 
        {
            get { return new RichTextBoxDocumentManager(_mainWindow.GetRichTextBox()); }
        }

        /// <summary>
        /// 提供打印预览功能
        /// </summary>
     //   private PrintManager printManager = null;


        #endregion
        #region 处理颜色
        private void BtnChnageFontSize_Click(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// 人工设置字体尺寸
        /// </summary>
        private void SetFontSize()
        {
            double size;
            if (Double.TryParse(txtFontSize.Text, out size))
            {
                if (_mainWindow.GetRichTextBox() == null)
                {
                    return;
                }
                _mainWindow.GetRichTextBox().Selection.ApplyPropertyValue(
                        FlowDocument.FontSizeProperty, size);
            }

        }
        private void txtFontSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetFontSize();
                txtFontSize.SelectAll();
            }
        }

        private void btnClearAllProperties_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTextColor_Click(object sender, RoutedEventArgs e)
        {
            if (lstFontColor.SelectedValue != null)
            {
                if (_mainWindow.GetRichTextBox() == null)
                {
                    return;
                }
                _mainWindow.GetRichTextBox().Selection.ApplyPropertyValue(FlowDocument.ForegroundProperty,
                                                      lstFontColor.SelectedValue);
            }
            else
            {
                if (_mainWindow.GetRichTextBox() == null)
                {
                    return;
                }
                //默认为红色
                _mainWindow.GetRichTextBox().Selection.ApplyPropertyValue(FlowDocument.ForegroundProperty,
                                                   Brushes.Red);
            }
        }
       
        private void lstFontColor_PreViewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = e.Source as Rectangle;
            if (rect != null)
            {
                lstFontColor.SelectedValue = rect.Fill;
                if (_mainWindow.GetRichTextBox()==null)
                {
                    return;
                }
                _mainWindow.GetRichTextBox().Selection.ApplyPropertyValue(FlowDocument.ForegroundProperty, lstFontColor.SelectedValue);
            }
        }

        private void lstBackgroundColor_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = e.Source as Rectangle;
            if (rect != null)
            {
                if (_mainWindow.GetRichTextBox() == null)
                {
                    return;
                }
                lstBackgroundColor.SelectedValue = rect.Fill;
                _mainWindow.GetRichTextBox().Selection.ApplyPropertyValue(FlowDocument.BackgroundProperty,
                                                    lstBackgroundColor.SelectedValue);
            }
        }
        #endregion
        #region "项目符号"

        private void listMarkerStyleChange(object sender, RoutedEventArgs e)
        {
            if (_mainWindow.GetRichTextBox() == null)
            {
                return;
            }
            List list = GetList();
            switch ((e.Source as MenuItem).Name)
            {
                case "mnuNone":
                case "mnuNone2":
                    EditingCommands.ToggleBullets.Execute(null, _mainWindow.GetRichTextBox());
                    break;
                case "mnuBox":

                    list.MarkerStyle = (TextMarkerStyle)Enum.Parse(typeof(TextMarkerStyle), "Box");
                    break;
                case "mnuSquare":
                    list.MarkerStyle = (TextMarkerStyle)Enum.Parse(typeof(TextMarkerStyle), "Square");
                    break;
                case "mnuDisc":
                    list.MarkerStyle = (TextMarkerStyle)Enum.Parse(typeof(TextMarkerStyle), "Disc");
                    break;
                case "mnuCircle":
                    list.MarkerStyle = (TextMarkerStyle)Enum.Parse(typeof(TextMarkerStyle), "Circle");
                    break;
                case "mnuDecimal":
                    list.MarkerStyle = (TextMarkerStyle)Enum.Parse(typeof(TextMarkerStyle), "Decimal");
                    break;
                case "mnuLowerLatin":
                    list.MarkerStyle = (TextMarkerStyle)Enum.Parse(typeof(TextMarkerStyle), "LowerLatin");
                    break;
                case "mnuUpperLatin":
                    list.MarkerStyle = (TextMarkerStyle)Enum.Parse(typeof(TextMarkerStyle), "UpperLatin");
                    break;
                case "mnuLowerRoman":
                    list.MarkerStyle = (TextMarkerStyle)Enum.Parse(typeof(TextMarkerStyle), "LowerRoman");
                    break;
                case "mnuUpperRoman":
                    list.MarkerStyle = (TextMarkerStyle)Enum.Parse(typeof(TextMarkerStyle), "UpperRoman");
                    break;
                default:
                    break;
            }
            //获取当前选中的字体大小

            Object obj = _mainWindow.GetRichTextBox().Selection.GetPropertyValue(
                                             FlowDocument.FontSizeProperty);
            //保证项目符号与文字大小一致
            if (obj is double)
                list.FontSize = (double)obj;
            _mainWindow.GetRichTextBox().Focus();
        }
        /// <summary>
        /// 获取当前光标所在位置文本，将其设置为FlowDocument的List元素
        /// </summary>
        /// <returns></returns>
        private List GetList()
        {
            if (_mainWindow.GetRichTextBox() == null)
            {
                return null;
            }

            List list = rtfManager.FindListAncestor(_mainWindow.GetRichTextBox().Selection.Start.Parent);
            if (list == null)
            {
                EditingCommands.ToggleBullets.Execute(null, _mainWindow.GetRichTextBox());
                list = rtfManager.FindListAncestor(_mainWindow.GetRichTextBox().Selection.Start.Parent);
            }
            return list;
        }

        private void btnSetBulletList_Click(object sender, RoutedEventArgs e)
        {
            if (_mainWindow.GetRichTextBox() == null)
            {
                return;
            }
            List list = GetList();
            list.MarkerStyle = (TextMarkerStyle)Enum.Parse(typeof(TextMarkerStyle), "Box");
            //获取当前选中的字体大小
            Object obj = _mainWindow.GetRichTextBox().Selection.GetPropertyValue(
                                             FlowDocument.FontSizeProperty);
            //保证项目符号与文字大小一致
            if (obj is double)
                list.FontSize = (double)obj;
            _mainWindow.GetRichTextBox().Focus();
        }

        private void btnSetNumberList_Click(object sender, RoutedEventArgs e)
        {
            if (_mainWindow.GetRichTextBox() == null)
            {
                return;
            }
            List list = rtfManager.FindListAncestor(_mainWindow.GetRichTextBox().Selection.Start.Parent);
            if (list == null)
            {
                EditingCommands.ToggleBullets.Execute(null, _mainWindow.GetRichTextBox());
                list = rtfManager.FindListAncestor(_mainWindow.GetRichTextBox().Selection.Start.Parent);
            }
            list.MarkerStyle = (TextMarkerStyle)Enum.Parse(typeof(TextMarkerStyle), "Decimal");
            //获取当前选中的字体大小

            Object obj = _mainWindow.GetRichTextBox().Selection.GetPropertyValue(
                                             FlowDocument.FontSizeProperty);
            //保证项目符号与文字大小一致
            if (obj is double)
                list.FontSize = (double)obj;
            _mainWindow.GetRichTextBox().Focus();
        }
        #endregion
        #region "处理字体"

        /// <summary>
        /// 使用一个列表框显示系统字体
        /// </summary>
        /// <param name="lst"></param>
        private void LoadSystemFontsToListBox(ComboBox cbo)
        {
            foreach (FontFamily family in Fonts.SystemFontFamilies)
                cbo.Items.Add(WPFSuperRichTextBox.MySuperEditorHelper.GetLocaliteFontName(family));
        }
        private void cboFontFamilies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboFontFamilies.SelectedItem != null)
            {
                if (_mainWindow.GetRichTextBox() == null)
                {
                    return;
                }
                _mainWindow.GetRichTextBox().Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, cboFontFamilies.SelectedItem);
                _mainWindow.GetRichTextBox().Focus();
                e.Handled = true;
            }

        }


        #endregion

    }
}
