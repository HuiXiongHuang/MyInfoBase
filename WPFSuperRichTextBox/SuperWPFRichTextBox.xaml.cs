using InterfaceLibrary;
using System;
//using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SystemLibrary;
using WinForms = System.Windows.Forms;

namespace WPFSuperRichTextBox
{
    /// <summary>
    /// SuperWPFRichTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class SuperWPFRichTextBox : UserControl
    {

        /// <summary>
        /// 用于引用主窗体，节点可以使用此引用调用主窗体所提供的功能（比如显示信息）
        /// </summary>
        public static IMainWindowFunction _mainWindow = null;
        public SuperWPFRichTextBox()
        {
            InitializeComponent();
            Init();
            //设置默认为非粗体
            this.FontWeight = FontWeights.Normal;

        }

        /// <summary>
        /// 获取当前RichTextBox所显示内容的纯文本字串
        /// </summary>
        public String Text
        {
            get
            {
                return rtfManager.Text;

            }
            set
            {
                rtfManager.Text = value;
            }

        }
        /// <summary>
        /// 提取当前RichTextBox所显示内容的RTF字串
        /// </summary>
        public String Rtf
        {
            get
            {
                return rtfManager.RTF;
            }
            set
            {
                rtfManager.RTF = value;
            }
        }
        /// <summary>
        /// 当保存文件时所调用的外接函数
        /// </summary>
        public Action OnSaveDocument;

        /// <summary>
        /// 获取内部包容的RichTextBox控件引用
        /// </summary>
        public RichTextBox EmbedRichTextBox
        {
            get
            {
                return RichTextBox1;
            }
        }

  
        public EditorToolBar EditorTool
        {
            get;
            set;
        }

        #region "变量区"

        private WinForms.OpenFileDialog OpenFileDialog1 = new WinForms.OpenFileDialog();


        private const string ProgramName = "MySuperEditor";

        /// <summary>
        /// 提供对于编辑器功能的基本实现
        /// </summary>
        public RichTextBoxDocumentManager rtfManager = null;

        /// <summary>
        /// 提供打印预览功能
        /// </summary>
        public PrintManager printManager = null;


        #endregion

        #region "系统初始化区"
        /// <summary>
        /// 系统初始化
        /// </summary>
        private void Init()
        {
            rtfManager = new RichTextBoxDocumentManager(RichTextBox1);
            printManager = new PrintManager(RichTextBox1);

            OpenFileDialog1.Filter = "所有可处理类型的文件|*.xaml;*.xamlpackage;*.rtf;*.txt|FlowDocument的纯XAML文件(*.xaml)|*.xaml|FlowDocument的XAML包文件(*.xamlpackage)|*.xamlpackage|RTF文件(*.rtf)|*.rtf|纯文本文件(*.txt)|*.txt|任意扩展名文件(*.*)|*.*";

        
            //设置默认字体大小
            RichTextBox1.FontSize = SystemConfig.configArgus.RichTextEditorDefaultFontSize;
            //显示当前字体尺寸
            //txtFontSize.Text = RichTextBox1.FontSize.ToString();
            //设定块元素间距，即回车后两段之间的距离
            RichTextBox1.Document.LineHeight = 2;

            RichTextBox1.AutoWordSelection = false;
            //必须要将MouseLeftDown事件用代码加入，否则，无法激发！
            RichTextBox1.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(RichTextBox1_MouseLeftButtonDown), true);

           

        }

        /// <summary>
        /// 当文本发生改变时调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRichTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            ShowFontSize();
        }


        #endregion
        
        #region "RichTextBox事件处理"

        private void SynchronizeBold()
        {

        }

        private void SynchronzieItalic()
        {
            Object obj = RichTextBox1.Selection.GetPropertyValue(TextElement.FontStyleProperty);

        }

        private void SychronzieUnderline()
        {

        }

        private void SynchronzieFontFamilies()
        {
            Object obj = null;
            //显示当前字体
            if (RichTextBox1.Selection.IsEmpty)  //如果当前没选中文字
            {
                TextRange nearestChar = rtfManager.GetPrevChar(RichTextBox1.CaretPosition);
                if ((nearestChar != null) && MySuperEditorHelper.IsTwoByteChineseChar(nearestChar.Text))
                    //是汉字,获取离光标最近字符的字体
                    obj = nearestChar.GetPropertyValue(TextElement.FontFamilyProperty);
                else
                    //不是汉字,或者文档为空直接调用RichTextBox默认功能实现
                    obj = RichTextBox1.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            }
            else //如果当前选中文字
                obj = RichTextBox1.Selection.GetPropertyValue(TextElement.FontFamilyProperty);

         


        }


        /// <summary>
        /// 同步对齐方式按钮
        /// </summary>
        private void SynchronzieTextAligmentButton()
        {
            object obj = RichTextBox1.Selection.GetPropertyValue(
                                        Paragraph.TextAlignmentProperty);
            if (obj != null && obj is TextAlignment)
                SwitchStatusOfTextAlignButtons((TextAlignment)obj);

        }

        /// <summary>
        /// 同步上下标按钮状态
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private void SynchronizeSuperscriptAndSubscriptButton()
        {

        }
        #endregion

        #region "文件部分功能"



        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnOpenDocument(object sender, ExecutedRoutedEventArgs args)
        {
            WinForms.DialogResult ret = OpenFileDialog1.ShowDialog();
            if (ret == WinForms.DialogResult.OK)
            {
                rtfManager.LoadOrInsertFile(false, OpenFileDialog1.FileName);


            }
        }

        /// <summary>
        /// 插入文档
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnInsertDocument(object sender, ExecutedRoutedEventArgs args)
        {
            WinForms.DialogResult ret = OpenFileDialog1.ShowDialog();
            if (ret == WinForms.DialogResult.OK)
            {
                rtfManager.LoadOrInsertFile(true, OpenFileDialog1.FileName);

            }
        }

        /// <summary>
        /// 保存文档
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSave(object sender, ExecutedRoutedEventArgs e)
        {

            if (OnSaveDocument != null)
            {
                OnSaveDocument();
            }

        }
        /// <summary>
        /// 导出选择的部分到文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExportSelectionToFile(object sender, ExecutedRoutedEventArgs e)
        {
            rtfManager.SaveToFile(false);
        }

        void CanExportSelectionToFile(object sender, CanExecuteRoutedEventArgs args)
        {
            if (RichTextBox1 != null)
                args.CanExecute = !RichTextBox1.Selection.IsEmpty;
        }


        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnInsertImage(object sender, ExecutedRoutedEventArgs e)
        {
            rtfManager.InsertImageToRichTextBox();
        }

        #endregion

        #region "编辑部分功能"

        #region "处理删除"
        void CanDelete(object sender, CanExecuteRoutedEventArgs args)
        {
            if (RichTextBox1 != null)
                args.CanExecute = !RichTextBox1.Selection.IsEmpty;
        }
        void OnDelete(object sender, ExecutedRoutedEventArgs args)
        {
            if (RichTextBox1 != null)
                RichTextBox1.Selection.Text = "";
        }

        #endregion

        #region "处理剪切"
        //void CanMyCut(object sender, CanExecuteRoutedEventArgs args)
        //{

        //    if (RichTextBox1 != null)
        //        args.CanExecute = !RichTextBox1.Selection.IsEmpty;
        //}
        void OnMyCut(object sender, ExecutedRoutedEventArgs args)
        {
            if (RichTextBox1 != null)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    RichTextBox1.Selection.Save(mem, DataFormats.Rtf);
                    String rtf = Encoding.UTF8.GetString(mem.ToArray());
                    Clipboard.SetData(DataFormats.Rtf, rtf);
                    RichTextBox1.Selection.Text = "";
                }

            }


        }
        #endregion


        #region "查找与替换"



        private winFindAndReplace winFR = null;
        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnFind(object sender, ExecutedRoutedEventArgs args)
        {
            if (winFR == null || winFR.IsLoaded == false)
                winFR = new winFindAndReplace(RichTextBox1);

            winFR.Show();
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //关闭查找窗体
            if (winFR != null)
            {
                winFR.CloseTag = true;
                winFR.Close();
            }
        }
        #endregion



        #region "处理按钮状态切换"
        private void SwitchStatusOfTextAlignButtons(TextAlignment value)
        {
            //btnAlignCenter.IsChecked = false;
            //btnAlignRight.IsChecked = false;
            //btnAllignJustify.IsChecked = false;
            //btnAlignLeft.IsChecked = false;
            //switch (value)
            //{
            //    case TextAlignment.Center:
            //        btnAlignCenter.IsChecked = true;
            //        break;
            //    case TextAlignment.Justify:
            //        btnAllignJustify.IsChecked = true;
            //        break;
            //    case TextAlignment.Left:
            //        btnAlignLeft.IsChecked = true;
            //        break;
            //    case TextAlignment.Right:
            //        btnAlignRight.IsChecked = true;
            //        break;
            //    default:
            //        break;
            //}

        }
        #endregion

        #region "处理字体家族"

        /// <summary>
        /// 使用一个列表框显示系统字体
        /// </summary>
        /// <param name="lst"></param>
        private void LoadSystemFontsToListBox(ComboBox cbo)
        {

        }
        private void cboFontFamilies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        #endregion


        #region "字体尺寸"

        /// <summary>
        /// 人工设置字体尺寸
        /// </summary>
        private void SetFontSize()
        {
            double size;
            if (Double.TryParse(_mainWindow.GetFontSizeTextBox().Text, out size))
            {
                RichTextBox1.Selection.ApplyPropertyValue(
                        FlowDocument.FontSizeProperty, size);
            }

        }

        /// <summary>
        /// 根据当前选中文字在文本框中显示尺寸
        /// </summary>
        private void ShowFontSize()
        {
            //显示当前字体尺寸
            Object obj = RichTextBox1.Selection.GetPropertyValue(
                                             FlowDocument.FontSizeProperty);

            if (obj is double)
            {

                _mainWindow.GetFontSizeTextBox().Text = string.Format("{0}", Convert.ToInt32(obj));
            }
            else
                _mainWindow.GetFontSizeTextBox().Text = RichTextBox1.FontSize.ToString();
        }



        private void OnSetTextFontSize(object sender, ExecutedRoutedEventArgs e)
        {
            SetFontSize();
            _mainWindow.GetFontSizeTextBox().SelectAll();
        }
        private void OnSetTextFontSize(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetFontSize();
                _mainWindow.GetFontSizeTextBox().SelectAll();
            }
        }


        #endregion



        #region "上标和下标"
        /// <summary>
        /// 设置上标
        /// </summary>
        private void OnSetSuperscript(object sender, ExecutedRoutedEventArgs args)
        {
            SetSuperscriptOrSubscript(BaselineAlignment.Superscript);
        }

        private void CanSetSuperscript(object sender, CanExecuteRoutedEventArgs args)
        {
            if (RichTextBox1 != null)
                args.CanExecute = !(RichTextBox1.Selection.IsEmpty);
            else
                args.CanExecute = false;

        }
        private void CanSetSubscript(object sender, CanExecuteRoutedEventArgs args)
        {
            if (RichTextBox1 != null)
                // args.CanExecute = !(String.IsNullOrEmpty(RichTextBox1.Selection.Text));
                args.CanExecute = !(RichTextBox1.Selection.IsEmpty);
            else
                args.CanExecute = false;
        }
        /// <summary>
        /// 设置下标
        /// </summary>
        private void OnSetSubscript(object sender, ExecutedRoutedEventArgs args)
        {
            SetSuperscriptOrSubscript(BaselineAlignment.Subscript);
        }
        /// <summary>
        /// 根据当前选中文本的上标或下标状态设置或取消上下标，同时同步按钮状态
        /// </summary>
        /// <param name="value"></param>
        private void SetSuperscriptOrSubscript(BaselineAlignment value)
        {
            BaselineAlignment curValue = rtfManager.GetSelectionBaselineAlignment();

            if (curValue != value)
            {
                rtfManager.SetSelectionBaselineAlignment(value);
                
            }
            else
            {
                rtfManager.SetSelectionBaselineAlignment(BaselineAlignment.Baseline);
               
            }
        }

        #endregion
        /// <summary>
        /// 将选中的文本还原到默认格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClearAllProperties(object sender, ExecutedRoutedEventArgs e)
        {
            if (RichTextBox1.Selection.IsEmpty)
                return;
            RichTextBox1.Selection.ClearAllProperties();
        }
     


        #endregion

        #region "打印"
        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            // Close();
        }

        private void OnPrint(object sender, ExecutedRoutedEventArgs e)
        {
            printManager.Print();
        }

        private void OnPrintPreview(object sender, ExecutedRoutedEventArgs e)
        {
            printManager.PrintPreview();
        }
        #endregion



        private void OnPaste(object sender, ExecutedRoutedEventArgs e)
        {
            //提取剪贴板数据
            IDataObject dataObject = Clipboard.GetDataObject();
            //检查数据是否为设备无关位图
            if (dataObject != null && dataObject.GetData(DataFormats.Dib) != null)
            {
                //将Dib转换为Bitmap
                Stream imageData = Clipboard.GetDataObject().GetData(DataFormats.Dib) as Stream;
                System.Drawing.Bitmap bmp = MySuperEditorHelper.CreateBitmapFromDib(imageData);
                //创建WPF的Image控件，并将其插入到当前光标位置
                System.Windows.Controls.Image imageControl = new System.Windows.Controls.Image();
                imageControl.Source = MySuperEditorHelper.BitmapToImageSource(bmp);
                imageControl.Width = bmp.Width;
                imageControl.Height = bmp.Height;
                //如果当前有选中的，则先清空它，之后再插入图片，这样一来，就实现了“替换”的效果
                if (String.IsNullOrEmpty(RichTextBox1.Selection.Text) == false)
                    RichTextBox1.Selection.Text = "";
                InlineUIContainer container = new InlineUIContainer(imageControl, RichTextBox1.Selection.Start);
            }
            else
            {
                RichTextBox1.Paste();
            }


            if (Clipboard.ContainsData(DataFormats.Bitmap))
            {
                //如果粘贴的是图片，则设置为左对齐（因为默认情况下，粘贴图片是居中对齐）
                EditingCommands.AlignLeft.Execute(null, RichTextBox1);
                TextRange selection = RichTextBox1.Selection;
                //取消选中状态，光标移动到最后
                selection.Select(selection.End, selection.End);
            }


        }
        /// <summary>
        /// 按纯文本格式粘贴
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasteAndResetToDefaultFormat(object sender, ExecutedRoutedEventArgs e)
        {

            String text = Clipboard.GetText(TextDataFormat.UnicodeText);
            if (String.IsNullOrEmpty(text))
            {
                return;
            }
            else
            {
                //插入内容
                RichTextBox1.Selection.Text = text;
                //清除选中状态
                RichTextBox1.Selection.Select(RichTextBox1.Selection.End, RichTextBox1.Selection.End);
            }

        }



 
        private void btnChnageFontSize_Click(object sender, RoutedEventArgs e)
        {

            ShowFontSize();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //关闭查找窗体
            if (winFR != null)
            {
                winFR.CloseTag = true;
                winFR.Close();
                winFR = null;
            }

        }

        private void RichTextBox1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //切换粗体选项
            SynchronizeBold();
            //切换斜体选项
            SynchronzieItalic();
            //切换下划线选项
            SychronzieUnderline();
            ShowFontSize();

            SynchronzieFontFamilies();
            SynchronzieTextAligmentButton();
            SynchronizeSuperscriptAndSubscriptButton();
        }
      
    }
}
