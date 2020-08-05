using DataAccessLayer.InfoNodeDA;
using Model;
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
using SystemLibrary;
using WPFSuperRichTextBox;
using WinForm = System.Windows.Forms;
using MessageBox = WPFMessageBoxView.MessageBox;
using System.Windows.Threading;
using InterfaceLibrary;
using PublicLibrary.Security.SymmetricEncryption;
using PublicLibrary.Security.AsymmetricEncryption;

namespace InfoNode
{
    /// <summary>
    /// InfoNodeControl.xaml 的交互逻辑
    /// </summary>
    public partial class InfoNodeControl : UserControl
    {




        public InfoNodeControl()
        {
            InitializeComponent();
            //当点击保存按钮时，刷新数据库
            richTextBox1.OnSaveDocument += UpdateDb;

            RaiseFileCountChangedEvent();
        }
        public SuperWPFRichTextBox InnerSuperRichTextBox
        {
            get { return richTextBox1; }

        }

        public InfoNodeAccess accessObj
        {
            get;
            set;
        }
        public EditorToolBar EditorTool { get; set; }

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
                    Dispatcher.Invoke(new Action(() => { MessageBox.ShowInformation("数据己保存"); }));
                }
                catch (Exception ex)
                {

                    Dispatcher.Invoke(new Action(() => { MessageBox.ShowInformation(ex.ToString()); }));
                }

            });
            task.Start();
        }

        private InfoNodeDataInfo _dataObject = null;

        /// <summary>
        /// 数据对象,读取时自动用当前控件值更新数据对象状态
        /// </summary>
        public InfoNodeDataInfo DataObject
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

        private void ShowDataObjectInUI(InfoNodeDataInfo obj)
        {
            if (obj != null)
            {
                richTextBox1.Rtf = obj.RTFText;
                dgFiles.ItemsSource = obj.AttachFiles;
                dgLabels.ItemsSource = obj.AttachLabels;
            }
        }

        public void UpdateDataObjectInMemory()
        {
            _dataObject.RTFText = richTextBox1.Rtf;
            _dataObject.Text = richTextBox1.Text;
            _dataObject.ModifyTime = DateTime.Now;


        }
        /// <summary>
        /// 刷新显示
        /// </summary>
        public void RefreshDisplay()
        {
            //因为配置有可能改变，所以重设默认字体
            richTextBox1.FontSize = SystemConfig.configArgus.RichTextEditorDefaultFontSize;
            ShowDataObjectInUI(_dataObject);

        }




        /// <summary>
        /// 失去焦点时，更新数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richTextBox1_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            UpdateDataObjectInMemory();

            Thread thread = new Thread(() =>
            {
                try
                {
                    accessObj.UpdateDataInfoObject(_dataObject);
                }
                catch (Exception ex)
                {

                    Dispatcher.Invoke(new Action(() => { MessageBox.ShowInformation(ex.ToString()); }));
                }
            });
            thread.Start();

        }
        #region 操作附件栏
        private void RaiseFileCountChangedEvent()
        {
            bool einfo = false;
            if (dgFiles.Items.Count > 0)
                einfo = true;
            FileCountChangedEventManager eventManager = new FileCountChangedEventManager();
            eventManager.RaiseEvent(einfo, accessObj, _dataObject);
        }


        static AutoResetEvent myResetEvent = new AutoResetEvent(false);

        /// <summary>
        /// 添加文件
        /// </summary>
        private  void AddFiles()
        {
            WinForm.OpenFileDialog openFileDialog = new WinForm.OpenFileDialog();
            openFileDialog.Multiselect = true;
            String info = "";
            if (openFileDialog.ShowDialog() == WinForm.DialogResult.OK)
            {
                int t = 1;
                foreach (var FileName in openFileDialog.FileNames)
                {
                    FileInfo fi = new FileInfo(FileName);
                    if (fi.Length > 500 * 1024 * 1024)
                    {
                        MessageBoxResult result = MessageBox.Show("要加入的文件:“" + System.IO.Path.GetFileName(FileName) + "”大小为：" + FileUtils.FileSizeFormater(fi.Length) + "，将大文件加入数据库会导致程序性能下降，点击“是”添加此文件，点击“否”跳过此文件", "加入大文件", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                        if (result == MessageBoxResult.No)
                        {
                            continue;
                        }
                    }
                   

                    DBFileInfo fileInfo = new DBFileInfo()
                    {
                        AddTime = DateTime.Now,
                        FilePath = FileName,
                        FileSize = fi.Length,
                      //FileHashByte = hashByte
                    };
                    //不加入重复的文件
                    if (_dataObject.AttachFiles.IndexOf(fileInfo) != -1)
                    {
                        continue;
                    }
                    //当加入文件时，有可能因为另一进程也使用此文件而导致加载失败
                    try
                    {
                        byte[] fileContent = System.IO.File.ReadAllBytes(FileName);
                        //byte[] fileContentToEncrypt = System.IO.File.ReadAllBytes(FileName);
                        //var list= TripleDESHelper.EncryptData(fileContentToEncrypt);
                        //byte[] fileContent = list[1];
                        //byte[] keyToDB = RSAHelper.EncryptData(list[0]);

                        //计算个文件的哈希值
                        var hash = System.Security.Cryptography.HashAlgorithm.Create();
                        byte[] hashByte = hash.ComputeHash(fileContent);
                        fileInfo.FileHash = hashByte;
                        info = string.Format("正在加入文件{0}", System.IO.Path.GetFileName(FileName));

                        //Call the render thread update UI from the current thread
                        int i = 1;
                        while (i < 10)
                        {
                            if (_dataObject.MainWindow != null)
                            {
                                _dataObject.MainWindow.ShowInfo(info);
                            }
                            Dispatcher.Invoke(new Action(() =>
                            {
                            }), System.Windows.Threading.DispatcherPriority.Background);
                           // Thread.Sleep(100);
                            i++;
                        }
                        //this is a time-consuming process.
                        accessObj.AddFile(_dataObject.Path, fileInfo, fileContent);
                        //Call the render thread update UI from the current thread
                        int j = 1;
                        while (j < 10)
                        {
                            if (j == 1) {
                                if (_dataObject.MainWindow != null)
                                {
                                    _dataObject.MainWindow.ShowInfo(info+"添加完成");
                                }
                                _dataObject.AttachFiles.Add(fileInfo);
                            }
                            Dispatcher.Invoke(new Action(() =>
                            {
                            }), System.Windows.Threading.DispatcherPriority.Background);
                            //Thread.Sleep(100);
                            j++;
                        }                   
                    }
                    catch (IOException ex)
                    {
                        MessageBox.ShowInformation(ex.Message);
                    }
                }
                if (_dataObject.MainWindow != null)
                {
                    _dataObject.MainWindow.ShowInfo("添加完毕");

                }
                RaiseFileCountChangedEvent();
            }

        }
       
        /// <summary>
        /// 删除用户选择的文件
        /// </summary>
        private void RemoveFiles()
        {
            List<long> fileInfoIDs = new List<long>();

            List<DBFileInfo> fileInfos = new List<DBFileInfo>();
            foreach (var item in dgFiles.SelectedItems)
            {
                DBFileInfo fileInfo = item as DBFileInfo;
                fileInfos.Add(fileInfo);
                fileInfoIDs.Add(fileInfo.ID);
            }
            //var test = BitConverter.ToString(fileInfos[0].FileHash).Replace("-", "");
            //删除文件内容
            List<long> fileIDs = accessObj.GetDBFileToBeRemove(fileInfos);
            if (fileIDs.Count >= 0)
            {
                accessObj.DeleteFiles(fileIDs);
            }

            //删除文件信息
            if (fileInfoIDs.Count > 0)
            {
                accessObj.DeleteFilesOfInfoNodeDB(_dataObject.Path, fileInfoIDs);
            }
            foreach (var fileInfo in fileInfos)
            {
                _dataObject.AttachFiles.Remove(fileInfo);
            }
            RaiseFileCountChangedEvent();
        }


        /// <summary>
        /// 将选中的文件导出到指定文件夹
        /// </summary>
        private void ExportToDisk()
        {
            WinForm.FolderBrowserDialog InfoNodeDialog = null;
            String SavePath = "";
            String SaveFileNameWithPath = "";
            String SaveFileName = "";
            byte[] fileContent = null;
            if (dgFiles.SelectedItems.Count > 0)
            {

                var fileInfo = dgFiles.SelectedItems[0] as DBFileInfo;
                //设置要导出的文件夹
                InfoNodeDialog = new WinForm.FolderBrowserDialog();
                InfoNodeDialog.Description = "选择用于保存文件的文件夹";
                SavePath = System.IO.Path.GetDirectoryName(fileInfo.FilePath);
                //提取用户选择的第一个文件路径，如果其存在，则将其作为保存导出文件的文件夹
                //如果不存在，导出到"我的文档"文件夹
                if (System.IO.Directory.Exists(SavePath))
                {
                    InfoNodeDialog.SelectedPath = SavePath;
                }
                else
                {
                    InfoNodeDialog.RootFolder = Environment.SpecialFolder.MyDocuments;
                }

                if (InfoNodeDialog.ShowDialog() == WinForm.DialogResult.OK)
                {
                    SavePath = InfoNodeDialog.SelectedPath;
                    try
                    {
                        int exportedFiles = 0;
                        //循环导出所有文件。
                        foreach (DBFileInfo dbFile in dgFiles.SelectedItems)
                        {
                            SaveFileName = System.IO.Path.GetFileName(dbFile.FilePath);
                            SaveFileNameWithPath = SavePath + "\\" + SaveFileName;
                            fileContent = accessObj.getFileContent(dbFile.ID);

                            //var Encryptedfiles = accessObj.getFileContentAndItsKey(fileInfo.ID);
                            //byte[] Decryptedfiles = TripleDESHelper.DecryptData(RSAHelper.DecryptData(Encryptedfiles[0]), Encryptedfiles[1]);
                            //fileContent = Decryptedfiles;

                            if (fileContent != null)
                            {
                                File.WriteAllBytes(SaveFileNameWithPath, fileContent);
                            }
                            exportedFiles++;
                            //在主窗体上显示相关信息
                            if (_dataObject.MainWindow != null)
                            {
                                _dataObject.MainWindow.ShowInfo("正在导出" + SaveFileName);
                            }
                        }

                    }
                    catch (IOException)
                    {
                        //忽略不能导出的文件（比如正在尝试覆盖的文件正在使用中，不能被覆盖）
                    }
                    //导出结束
                    String info = string.Format("文件导出结束，共导出{0}个文件到{1}", dgFiles.SelectedItems.Count, SavePath);

                    if (_dataObject.MainWindow != null)
                    {
                        _dataObject.MainWindow.ShowInfo(info);
                    }
                    //自动在Explorer中打开文件夹
                    Process.Start(SavePath);
                }

            }




        }

        #region 事件
        private void dgTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void btnAddFile_Click(object sender, RoutedEventArgs e)
        {
            AddFiles();

        }

        private void btnRemoveFile_Click(object sender, RoutedEventArgs e)
        {
            RemoveFiles();
        }

        private void btnExportToDisk_Click(object sender, RoutedEventArgs e)
        {
            ExportToDisk();
        }

        #endregion
        #endregion


        #region 操作标签栏

        private void btnAddLabel_Click(object sender, RoutedEventArgs e)
        {
            AddLabelEventManager addLabelEventManager = new AddLabelEventManager();

            addLabelEventManager.RaiseEvent(accessObj, _dataObject);

        }
        /// <summary>
        /// 获取需要删除的标签节点
        /// </summary>
        /// <returns></returns>
        public List<LabelNodeDB> GetlabelNodeDBsNeedToRemove()
        {
            List<long> labelIDs = new List<long>();
            foreach (var item in dgLabels.SelectedItems)
            {
                DBLabelInfo labelInfo = item as DBLabelInfo;
                labelIDs.Add(labelInfo.ID);
            }

            return accessObj.getLabelNodeDBs(labelIDs);
        }
        /// <summary>
        /// 仅删除标签与节点的关联并未删除数据库中标签
        /// </summary>
        public void RemoveLabelAssociation()
        {
            List<long> labelIDs = new List<long>();
            List<DBLabelInfo> labelInfos = new List<DBLabelInfo>();
            foreach (var item in dgLabels.SelectedItems)
            {
                DBLabelInfo labelInfo = item as DBLabelInfo;
                labelInfos.Add(labelInfo);
                labelIDs.Add(labelInfo.ID);
            }
            if (labelIDs.Count > 0)
            {
                accessObj.DeleteLabelsOfInfoNodeDB(_dataObject.Path, labelIDs);
            }
            foreach (var labelInfo in labelInfos)
            {
                _dataObject.AttachLabels.Remove(labelInfo);
            }
        }
        private void btnRemoveLabel_Click(object sender, RoutedEventArgs e)
        {
            //RemoveLabelEventManager removeLabelEventManager = new RemoveLabelEventManager();
            //List<LabelNodeDB> eventInfo = GetlabelNodeDBsNeedToRemove();
            //removeLabelEventManager.RaiseEvent(eventInfo);
            RemoveLabelAssociation();
        }



        #endregion

        private void OpenFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!(e.OriginalSource is TextBlock))
            {
                return;
            }
            DBFileInfo fileInfo = dgFiles.SelectedItem as DBFileInfo;

            if (fileInfo != null)
            {
                string tempSaveFolder = "";
                if (fileInfo.FilePath != null)
                {
                    try
                    {
                        // string  filePath = System.IO.Path.GetDirectoryName(fileInfo.FilePath); //获取路径到文件夹位置
                        if (System.IO.Directory.Exists(fileInfo.FilePath))
                        {
                            FileOpenUtils.OpentFileByFullPath(fileInfo.FilePath);
                        }
                        else
                        {
                            string tempSaveFileName = System.IO.Path.GetFileName(fileInfo.FilePath);
                            String currentDir = System.Environment.CurrentDirectory;
                            tempSaveFolder = currentDir + "\\" + "tempfolder";
                            if (!Directory.Exists(tempSaveFolder))
                            {
                                Directory.CreateDirectory(tempSaveFolder);
                            }
                            string tempSaveFileNameWithPath = tempSaveFolder + "\\" + tempSaveFileName;
                            var fileContent = accessObj.getFileContent(fileInfo.ID);
                            //var Encryptedfiles = accessObj.getFileContentAndItsKey(fileInfo.ID);
                            //byte[] Decryptedfiles = TripleDESHelper.DecryptData(RSAHelper.DecryptData(Encryptedfiles[0]), Encryptedfiles[1]);
                            //byte[] fileContent = Decryptedfiles;
                            if (fileContent != null)
                            {
                                Task task = new Task(() =>
                                {
                                    File.WriteAllBytes(tempSaveFileNameWithPath, fileContent);
                                    FileOpenUtils.OpentFileByFullPath(tempSaveFileNameWithPath);
                                    int DelayTime = 100;
                                    Thread.Sleep(DelayTime * 1000);//部分文件打开较慢，等待延时100秒再删除
                                    Directory.Delete(tempSaveFolder, true);
                                });
                                task.Start();
                                //task.Wait();//同步操作，等待task执行完毕再执行后面的语句。

                                // File.Delete(tempSaveFileNameWithPath);
                                // Directory.Delete(tempSaveFolder, true);
                            }
                        }

                    }
                    catch
                    {
                        MessageBox.ShowError("打开失败，操作系统中可能没有安装该文件打开所需可执行程序");


                    }
                }

            }

        }

        private void btnEncryptFile_Click(object sender, RoutedEventArgs e)
        {
           
            foreach (DBFileInfo dbFile in dgFiles.SelectedItems)
            {
                if (dbFile.IsEncrypted==false)
                {
                    var fileContentToEncrypt = accessObj.getFileContent(dbFile.ID);

                    //var list = TripleDESHelper.EncryptData(fileContentToEncrypt);
                    //byte[] fileContent = list[1];
                    //byte[] keyToDB = RSAHelper.EncryptData(list[0]);

                
                    byte[] fileContent = TripleDESHelper.Encrypt3DES(fileContentToEncrypt);


                    DiskFileContent diskFileContent = new DiskFileContent();
                    diskFileContent.ID = dbFile.FileContentID;
                    diskFileContent.FileKey = null;
                    diskFileContent.SavedFile = fileContent;
                    if (accessObj.UpdateFile(diskFileContent) <= 0)
                    {
                        MessageBox.ShowInformation("文件加密失败");
                    }

                    
                    else
                    {
                        //在主窗体上显示相关信息
                        if (_dataObject.MainWindow != null)
                        {
                            _dataObject.MainWindow.ShowInfo("加密成功");

                        }
                        dbFile.IsEncrypted = true;
                        accessObj.UpdateFileInfo(DBFileInfo.toDiskFileInfo(dbFile));
                    }
                }
 
            }
        }

        private void btnDecryptFile_Click(object sender, RoutedEventArgs e)
        {
            foreach (DBFileInfo dbFile in dgFiles.SelectedItems)
            {
                
                    //var Encryptedfiles = accessObj.getFileContentAndItsKey(dbFile.ID);
                    //byte[] Decryptedfiles = TripleDESHelper.DecryptData(RSAHelper.DecryptData(Encryptedfiles[0]), Encryptedfiles[1]);
                    //var   fileContent = Decryptedfiles;
                    var Encryptedfiles = accessObj.getFileContent(dbFile.ID);
                    byte[] Decryptedfiles = TripleDESHelper.Decrypt3DES(Encryptedfiles);
                    var fileContent = Decryptedfiles;


                    DiskFileContent diskFileContent = new DiskFileContent();
                    diskFileContent.ID = dbFile.FileContentID;
                    diskFileContent.FileKey = null;
                    diskFileContent.SavedFile = fileContent;
                    if (accessObj.UpdateFile(diskFileContent) <= 0)
                    {
                        MessageBox.ShowInformation("文件解密失败");
                    }
                    
                        //在主窗体上显示相关信息
                        if (_dataObject.MainWindow != null)
                        {
                            _dataObject.MainWindow.ShowInfo("解密成功");
                        }
                        dbFile.IsEncrypted = false;
                        accessObj.UpdateFileInfo(DBFileInfo.toDiskFileInfo(dbFile));
                   
               

            }
        }
     

        
    }
}
