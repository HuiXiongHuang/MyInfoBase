using DataAccessLayer.InfoNodeDA;
using DataAccessLayer.LabelNodeDA;
using InfoNode;
using PublicLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SystemLibrary;
using WPFSuperRichTextBox;
using WinForm = System.Windows.Forms;

namespace LabelNode
{
    /// <summary>
    /// InfoNodeControl.xaml 的交互逻辑
    /// </summary>
    public partial class LabelNodeControl : UserControl
    {
        
       
        public LabelNodeControl()
        {
            InitializeComponent();
            //当点击保存按钮时，刷新数据库
            //richTextBox1.OnSaveDocument += UpdateDb;

        }
        public InfoNodeControl InfoNodeCtrl
        {
            get 
            {
                if (infoNodeCtrlContainer.Content != null)
                {

                    return (infoNodeCtrlContainer.Content as InfoNodeControl);

                }
                else return null;
            
            }
            
        }

        public SuperWPFRichTextBox InnerSuperRichTextBox
        {
            get
            {
                if (InfoNodeCtrl != null)
                {
                    return InfoNodeCtrl.InnerSuperRichTextBox;
                }
                else
                    return null;

            }

        }
        public EditorToolBar EditorTool { get; set; }

        public LabelNodeAccess accessObj
        {
            get;
            set;
        }
        public ContentControl InnerContainer
        {
            get
            {
                return infoNodeCtrlContainer;
            }
            set { infoNodeCtrlContainer = value; }
        }
        /// <summary>
        ///  刷新数据库中的内容
        /// </summary>
        private void UpdateDb()
        {
            UpdateDataObjectInMemory();
            Task task = new Task(() =>
            {

                try
                {
                    accessObj.UpdateDataInfoObject(_dataObject);

                    Dispatcher.Invoke(new Action(() => { MessageBox.Show("数据己保存"); }));

                }
                catch (Exception ex)
                {

                    Dispatcher.Invoke(new Action(() => { MessageBox.Show(ex.ToString()); }));
                }

            });
            task.Start();
        }

        private LabelNodeDataInfo _dataObject = null;

        /// <summary>
        /// 数据对象,读取时自动用当前控件值更新数据对象状态
        /// </summary>
        public LabelNodeDataInfo DataObject
        {
            get
            {
                UpdateDataObjectInMemory();
                return _dataObject;
            }
            set
            {
                _dataObject = value;

            }
        }

        private void ShowDataObjectInUI(LabelNodeDataInfo obj)
        {
            if (obj != null)
            {
                dgFiles.ItemsSource = obj.AttachInfoNodeInfos;
            }
        }

        public void UpdateDataObjectInMemory()
        {
            //_dataObject.RTFText = InnerSuperRichTextBox.Rtf;
             //_dataObject.Text = InnerSuperRichTextBox.Text;
            _dataObject.ModifyTime = DateTime.Now;
            
           
        }
        /// <summary>
        /// 刷新显示
        /// </summary>
        public void RefreshDisplay()
        {
            //因为配置有可能改变，所以重设默认字体
            //InnerSuperRichTextBox.FontSize = SystemConfig.configArgus.RichTextEditorDefaultFontSize;
            ShowDataObjectInUI(_dataObject);

        }



       
        /// <summary>
        /// 失去焦点时，更新数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richTextBox1_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        { /*
            UpdateDataObjectInMemory();

            Thread thread = new Thread(() =>
            {
                try
                {
                    accessObj.UpdateDataInfoObject(_dataObject);
                }
                catch (Exception ex)
                {

                    Dispatcher.Invoke(new Action(() => { MessageBox.Show(ex.ToString()); }));
                }
            });
            thread.Start();
             */
        }
      
        private void dgTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dgFiles_mouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock)
            {
                DBInfoNodeInfo info = dgFiles.SelectedItem as DBInfoNodeInfo;
                LoadInfoNodeControlEventManager loadInfoNodeControlEventManager = new LoadInfoNodeControlEventManager();
                loadInfoNodeControlEventManager.RaiseEvent(info.Path);
            }

        }
    }
}
