using DataAccessLayer.InfoNodeDA;
using DataAccessLayer.LabelNodeDA;
using InterfaceLibrary;
using System;
using System.Collections.ObjectModel;

namespace LabelNode
{
    public class LabelNodeDataInfo : IDataInfo
    {

        private const string NoteText = "双击信息条目，可加载信息节点控件进行编辑";
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

        private ObservableCollection<DBInfoNodeInfo> _infoNodeInfos = new ObservableCollection<DBInfoNodeInfo>();
        /// <summary>
        /// 本文件夹节点所关联的信息节点信息对象的集合
        /// </summary>
        public ObservableCollection<DBInfoNodeInfo> AttachInfoNodeInfos
        {
            get
            {
                return _infoNodeInfos;
            }
            set
            {
                _infoNodeInfos = value;
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
            get { return "LabelNode"; }
        }
        public string normalIcon = "LabelNode";
        public string IconType
        {
            get
            {
                return normalIcon;

            }

            set { normalIcon = value; }
        }
        public System.Windows.Media.ImageSource NormalIcon
        {
            get { return LabelNodeResources.normalIcon; }
        }

        public System.Windows.Media.ImageSource SelectedIcon
        {
            get { return LabelNodeResources.selectedIcon; }
        }
        public System.Windows.Media.ImageSource FileIcon
        {
            get { return LabelNodeResources.FileIcon; }
        }

        public System.Windows.Controls.Control RootControl
        {
            get { return LabelNodeResources.RootControl; }
        }

        public bool ShouldEmbedInHostWorkingBench
        {
            get { return true; }
        }

        public void RefreshDisplay()
        {
            LabelNodeResources.RootControl.RefreshDisplay();
            //显示默认的提示信息
            if (_mainWindow != null)
            {
                _mainWindow.ShowInfo(NoteText);
            }
        }

        public void BindToRootControl()
        {
            LabelNodeResources.RootControl.DataObject = this;
        }


        public void RefreshMe()
        {
            LabelNodeResources.RootControl.UpdateDataObjectInMemory();
        }

        public void SetRootControlDataAccessObj(IDataAccess accessObj)
        {
            LabelNodeResources.RootControl.accessObj = accessObj as LabelNodeAccess;
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
