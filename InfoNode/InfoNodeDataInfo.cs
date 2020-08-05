using DataAccessLayer.InfoNodeDA;
using InterfaceLibrary;
using System;
using System.Collections.ObjectModel;

namespace InfoNode
{
    public class InfoNodeDataInfo : IDataInfo
    {

        private const string NoteText = "压住Control或Shift键可以选择多个文件。";
        public bool HasBeenLoadFromStorage
        {
            get;
            set;
        }

        public long ID
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

        public string Text { get; set; }

        public string RTFText { get; set; }

        public string NodeText { get; set; }
        private ObservableCollection<DBFileInfo> _files = new ObservableCollection<DBFileInfo>();
        /// <summary>
        /// 本文件夹节点所关联的文件对象集合
        /// </summary>
        public ObservableCollection<DBFileInfo> AttachFiles
        {
            get
            {
                return _files;
            }
            set
            {
                _files = value;
            }

        }
        private ObservableCollection<DBLabelInfo> _labels = new ObservableCollection<DBLabelInfo>();
        /// <summary>
        /// 本文件夹节点所关联的文件对象集合
        /// </summary>
        public ObservableCollection<DBLabelInfo> AttachLabels
        {
            get
            {
                return _labels;
            }
            set
            {
                _labels = value;
            }

        }
        private DateTime _ModifyTime = DateTime.Now;
        public DateTime ModifyTime
        {
            get
            {
                return _ModifyTime;
            }
            set
            {
                _ModifyTime = value;
            }
        }

        public string NodeType
        {
            get { return "InfoNode"; }
        }
        public string normalIcon = "InfoNode";
        public string IconType
        {
            get 
            {
                    return normalIcon;

            }

            set { normalIcon=value; }
        }
        public System.Windows.Media.ImageSource NormalIcon
        {
            get { return InfoNodeResources.NormalIcon; }
        }

        public System.Windows.Media.ImageSource SelectedIcon
        {
            get { return InfoNodeResources.SelectedIcon; }
        }
        public System.Windows.Media.ImageSource FileIcon
        {
            get { return InfoNodeResources.FileIcon; }
        }
        public System.Windows.Media.ImageSource NoInfoIcon
        {
            get { return InfoNodeResources.NoInfoIcon; }
        }
        public System.Windows.Media.ImageSource ImageIcon
        {
            get { return InfoNodeResources.ImageIcon; }
        }
        public System.Windows.Controls.Control RootControl
        {
            get { return InfoNodeResources.RootControl; }
        }

        public bool ShouldEmbedInHostWorkingBench
        {
            get { return true; }
        }

        public void RefreshDisplay()
        {
            InfoNodeResources.RootControl.RefreshDisplay();
            //显示默认的提示信息
            if (_mainWindow != null)
            {
                _mainWindow.ShowInfo(NoteText);
            }
        }

        public void BindToRootControl()
        {
            InfoNodeResources.RootControl.DataObject = this;
        }


        public void RefreshMe()
        {
            InfoNodeResources.RootControl.UpdateDataObjectInMemory();
        }

        public void SetRootControlDataAccessObj(IDataAccess accessObj)
        {
            InfoNodeResources.RootControl.accessObj = accessObj as InfoNodeAccess;
        }

        private IMainWindowFunction _mainWindow = null;
        public IMainWindowFunction MainWindow
        {
            get
            {
                return _mainWindow;
            }
            set
            {
                _mainWindow = value;
            }
        }

    }

}
