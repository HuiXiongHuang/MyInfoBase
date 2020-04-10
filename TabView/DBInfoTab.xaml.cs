using InfoNode;
using InterfaceLibrary;
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
using SystemLibrary;
using WPFDBInfoTab.BackAndForward;
using WPFSuperRichTextBox;
using WPFSuperTreeView;
using NodeFactoryLibrary;
using DataAccessLayer.NodeTreeDA;
using DataAccessLayer;
using System.Collections.ObjectModel;
using LabelNode;

namespace WPFDBInfoTab
{
    /// <summary>
    /// DBInfoTab.xaml 的交互逻辑
    /// </summary>
    public partial class DBInfoTab : UserControl, IHandler<LoadInfoNodeControlEvent>, IHandler<FileCountChangedEvent>
    {
        /// <summary>
        /// 用于引用主窗体，节点可以使用此引用调用主窗体所提供的功能（比如显示信息）
        /// </summary>
        public static IMainWindowFunction _mainWindow = null;
        public DBInfoTab()
        {

            InitializeComponent();


        }

        
        public DBInfoTab(DatabaseInfo infoObject)
        {
            InitializeComponent();
          
            dbInfoObject = infoObject;
        
            OutLineViewObj =  new OutLineView(this);
            LabelViewObj =  new LabelView(this);

            CurrentTreeView = InnerTreeViewList[dbInfoObject.LastTabViewIndex];//设置默认树窗口
            //outLinetree.TreeNodeType = "InfoNode";
            //labeltree.TreeNodeType = "LabelNode";
            visitedNodesManager = new VisitedNodesManager(CurrentTreeView);

            LoadInfoNodeControlEventManager loadInfoNodeControlEventManager = new LoadInfoNodeControlEventManager();
            loadInfoNodeControlEventManager.Register(this);
            FileCountChangedEventManager fileCountChangedEventManager = new FileCountChangedEventManager();
            fileCountChangedEventManager.Register(this);

        }
        public TabControl InnerTabControl
        {
            get { return tab; }
        }
        public SuperTreeView CurrentTreeView
        {
            get;
            set;
        }

        
        public OutLineView OutLineViewObj
        {
    

            get { return outlinetreeContainer.Content as OutLineView; }
            set 
            {
  
                    outlinetreeContainer.Content=value; 
            }
        }
        public LabelView LabelViewObj
        {
     
            get { return labeltreeContainer.Content as LabelView; }
            set 
            {
         
                labeltreeContainer.Content = value;
            }
        }
        public RichTextBox CurrentRichTextBox
        {
            get { return (NodeUIContainer.Content as InfoNodeControl).InnerSuperRichTextBox.EmbedRichTextBox; }
        }
        private ObservableCollection<SuperTreeView> _innerTreeViewList;
        public ObservableCollection<SuperTreeView> InnerTreeViewList
        {
            get
            {
                _innerTreeViewList = new ObservableCollection<SuperTreeView>();
                _innerTreeViewList.Add(OutLineViewObj.SuperTree);
                _innerTreeViewList.Add(LabelViewObj.SuperTree);
                return _innerTreeViewList;
            }
            
        }
      
  
        /// <summary>
        /// 用于实现节点历史访问记录功能
        /// </summary>
        public VisitedNodesManager visitedNodesManager = null;
        /// <summary>
        /// 相关联的数据库信息对象，序列化后将保存于配置文件中
        /// </summary>
        public DatabaseInfo dbInfoObject { get; set; }
        /// <summary>
        /// 是否己从数据库中装入
        /// </summary>
        public bool HasBeenLoadedFromDB { get; set; }
        public EditorToolBar EditorTool { get; set; }

        #region "更改图标"


        private void ChangeNodeIcon(TreeViewIconsItem node, ImageSource newIcon)
        {
            if (node == null || newIcon == null)
            {
                return;
            }
            node.Icon = newIcon;
        }
        /// <summary>
        /// 当用户点击树节点时，切换节点图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangedSelectedNodeIconWhenClick(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ////将老节点的图标改为默认的文件夹图标
            //TreeViewIconsItem oldNode = e.OldValue as TreeViewIconsItem;
            //if (oldNode != null)
            //{
            //    ChangeNodeIcon(oldNode, oldNode.NodeData.DataItem.NormalIcon);
            //}
            ////设置新节点图标
            //TreeViewIconsItem selectedNode = e.NewValue as TreeViewIconsItem;
            //if (selectedNode != null)
            //{
            //    ChangeNodeIcon(selectedNode, selectedNode.NodeData.DataItem.SelectedIcon);
            //}

        }
        #endregion


        /// <summary>
        /// 将树结构保存到数据库中
        /// </summary>
        public void SaveTreeToDB()
        {
            String treeXml = CurrentTreeView.saveToXmlString();

            (new NodeTreeRepository(DALConfig.getEFConnectionString(dbInfoObject.DatabaseFilePath))).SaveTree(treeXml,CurrentTreeView.TreeNodeType);

        }
        public void SaveTreeToDB(SuperTreeView TreeViewNeedToSave)
        {
            String treeXml = TreeViewNeedToSave.saveToXmlString();
            //if (treeXml == null)
            //    return;

            (new NodeTreeRepository(DALConfig.getEFConnectionString(dbInfoObject.DatabaseFilePath))).SaveTree(treeXml, TreeViewNeedToSave.TreeNodeType);

        }

        #region "用户点击树中的不同节点"

        /// <summary>
        /// 当切换数据库选项卡时，必须手动刷新界面
        /// </summary>
        public void RefreshDisplay()
        {
            if (CurrentTreeView.SelectedItem != null)
            {
                LoadDataAndShowInUI(CurrentTreeView.SelectedItem);
            }
            else
            {
                //没有选中任何节点，则显示空白的窗体
                NodeUIContainer.Content = null;
            }
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            TreeViewIconsItem newSelectedNode = e.NewValue as TreeViewIconsItem;
            if (CurrentTreeView.IsInEditMode)
            {
                if (newSelectedNode != null)
                {
                    newSelectedNode.IsSelected = false;
                }
                return;
            }

            //更换节点图标
            ChangedSelectedNodeIconWhenClick(sender, e);

            if (newSelectedNode != null)
            {
                LoadDataAndShowInUI(newSelectedNode);
            }
            else
            {
                //没有选中任何节点，则显示空白的窗体
                CurrentTreeView.Content = null;
            }
            if (visitedNodesManager != null && e.OldValue != null)
            {
                visitedNodesManager.AddHistoryRecord((e.OldValue as TreeViewIconsItem).Path);
            }


        }
        /// <summary>
        /// 为新节点提取数据并显示
        /// </summary>
        /// <param name="newSelectedNode"></param>
       public void LoadDataAndShowInUI(TreeViewIconsItem newSelectedNode)
        {

          
            NodeDataObject dataInfoObject = newSelectedNode.NodeData;
            //正确地设置可视化界面所关联的数据存取对象
            dataInfoObject.DataItem.SetRootControlDataAccessObj(dataInfoObject.AccessObject);

            //检查一下数据是否己被装入
            if (!dataInfoObject.DataItem.HasBeenLoadFromStorage)
            {
                //装入数据
                IDataInfo dataObj = dataInfoObject.AccessObject.GetDataInfoObjectByPath(newSelectedNode.Path);

                if (dataObj != null)
                {
                    //给将己装入数据的对象添加对主窗体的引用
                    if (NodeFactory._mainWindow != null)
                    {
                        dataObj.MainWindow = NodeFactory._mainWindow;
                    }
                    //把节点数据对象挂到节点上
                    newSelectedNode.NodeData.DataItem = dataObj;

                }
            }

            if (dataInfoObject.DataItem.ShouldEmbedInHostWorkingBench)
            {
                if (dataInfoObject.DataItem.RootControl.Parent != null)
                {
                    (dataInfoObject.DataItem.RootControl.Parent as ContentControl).Content = null;
                }
                //显示静态公有控件
                NodeUIContainer.Content = dataInfoObject.DataItem.RootControl;

           
                if (dataInfoObject.DataItem.RootControl is InfoNodeControl)
                {
                    (dataInfoObject.DataItem.RootControl as InfoNodeControl).InnerSuperRichTextBox.EditorTool = EditorTool;
               
                }


                //根节点不显示内容
                if (newSelectedNode.Parent is TreeView)
                {
                    NodeUIContainer.Content = null;

                }


            }
            //显示最新的数据
            dataInfoObject.DataItem.BindToRootControl();
            dataInfoObject.DataItem.RefreshDisplay();

        }

        /// <summary>
        /// 如果当前有选中的节点，则显示快捷菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (CurrentTreeView.SelectedItem == null)
            {
                e.Handled = true;
            }
          
        }
        /// <summary>
        /// 当用户在TreeView上右击鼠标时，设置当前选中的节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            IInputElement currentElement = CurrentTreeView.InputHitTest(e.GetPosition(CurrentTreeView));
            TreeViewIconsItem node = GetNodeUnderMouseCursor(currentElement as DependencyObject);
            if (node != null)
            {

                node.IsSelected = true;
            }
            else
            {
                e.Handled = true;
            }
        }
        /// <summary>
        /// 获取当前鼠标指针下的TreeView节点
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
       public TreeViewIconsItem GetNodeUnderMouseCursor(DependencyObject element)
        {
            if (element == null)
            {
                return null;
            }
            DependencyObject parent = LogicalTreeHelper.GetParent(element);
            while (parent != null && (parent as TreeViewIconsItem) == null)
            {
                parent = LogicalTreeHelper.GetParent(parent);

            }
            if (parent == null)
            {
                return null;
            }
            return parent as TreeViewIconsItem;
        }

        private void treeView1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) && (e.Key == Key.C))
            {
                CopyNodeText();
            }
        }
        /// <summary>
        /// 复制节点文本
        /// </summary>
        public void CopyNodeText()
        {
            if (CurrentTreeView.IsInEditMode)
            {
                CurrentTreeView.SelectedItem.EndEdit();
            }
            if (CurrentTreeView.SelectedItem != null)
            {
                String str = CurrentTreeView.SelectedItem.HeaderText;

                try
                {
                    System.Windows.Forms.Clipboard.SetText(str);
                }
                catch (System.Runtime.InteropServices.ExternalException)
                {
                    MessageBox.Show("打开剪贴板失败,可能有其他程序正在使用。请稍候重试操作。");
                }


            }
        }

        #endregion

        #region "节点移动"
        /// <summary>
        /// 用于代表当前节点移动的类型
        /// </summary>
        // private NodeMoveType curNodeMove=NodeMoveType.NodeNotMove ;

        public void OnNodeMove(NodeMoveType moveType)
        {
            // curNodeMove = moveType;

            if (CurrentTreeView.IsInEditMode)
            {
                CurrentTreeView.SelectedItem.EndEdit();
            }
            TreeViewIconsItem selectedNode = CurrentTreeView.SelectedItem;
            //还原默认节点图标
            ChangeNodeIcon(CurrentTreeView.SelectedItem, CurrentTreeView.SelectedItem.NodeData.DataItem.NormalIcon);
            switch (moveType)
            {
                case NodeMoveType.NodeMoveUp:
                    CurrentTreeView.MoveUp(selectedNode);

                    break;
                case NodeMoveType.NodeMoveDown:
                    CurrentTreeView.MoveDown(selectedNode);
                    break;
                case NodeMoveType.NodeMoveLeft:
                    CurrentTreeView.MoveLeft(selectedNode);
                    break;
                case NodeMoveType.NodeMoveRight:
                    CurrentTreeView.MoveRight(selectedNode);
                    break;
                case NodeMoveType.NodePaste:
                    //todo:paste!
                    break;
                default:
                    break;
            }
            SaveTreeToDB();
            //更新节点图标
            ChangeNodeIcon(CurrentTreeView.SelectedItem, CurrentTreeView.SelectedItem.NodeData.DataItem.SelectedIcon);

            //SaveTreeToFile();
        }

        public void MoveLeft(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentTreeView.IsInEditMode)
            {
                CurrentTreeView.SelectedItem.EndEdit();
            }
            OnNodeMove(NodeMoveType.NodeMoveLeft);
        }

        public void MoveRight(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentTreeView.IsInEditMode)
            {
                CurrentTreeView.SelectedItem.EndEdit();
            }
            OnNodeMove(NodeMoveType.NodeMoveRight);
        }

        public void MoveUp(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentTreeView.IsInEditMode)
            {
                CurrentTreeView.SelectedItem.EndEdit();
            }
            OnNodeMove(NodeMoveType.NodeMoveUp);
        }

        public void MoveDown(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentTreeView.IsInEditMode)
            {
                CurrentTreeView.SelectedItem.EndEdit();
            }
            OnNodeMove(NodeMoveType.NodeMoveDown);
        }


        #endregion


        private void tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            e.Handled = true;//防止路由到根元素被误接收。

            //if (e.RemovedItems.Count > 0)//SelectionChanged事件在WPF页面加载完成之前会自动执行一次，我们并不希望这样。该判断可避免SelectionChanged事件的第一次执行：
            //{

            //事件响应
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var data = tab.SelectedItem as TabItem;
                var headerText = data.Header.ToString();
                if (headerText == "标签")
                {

                    CurrentTreeView = LabelViewObj.SuperTree;

               
                }
               if ((headerText == "大纲"))
                {
                    CurrentTreeView = OutLineViewObj.SuperTree;
                  

                }
                if (CurrentTreeView.SelectedItem != null)
                {
                   
                    CurrentTreeView.SelectedItem.IsSelected = true;
                    LoadDataAndShowInUI(CurrentTreeView.SelectedItem);
                }
                
               
            }));
            //}



        }

        public void Handle(LoadInfoNodeControlEvent e)
        {
            //查找节点，显示其内容
         if(OutLineViewObj.SuperTree.IsNodeExisted(e.Path))
          {
                TreeViewIconsItem nodeNeedToLoad = OutLineViewObj.SuperTree.Nodes.FirstOrDefault(n => n.Path == e.Path);
                LoadDataAndShowInLabelNodeUI(nodeNeedToLoad);
            }
              
        }

        /// <summary>
        /// 再标签节点中显示信息节点内容
        /// </summary>
        /// <param name="newSelectedNode"></param>
        private void LoadDataAndShowInLabelNodeUI(TreeViewIconsItem newSelectedNode)
        {


            NodeDataObject dataInfoObject = newSelectedNode.NodeData;
            //正确地设置可视化界面所关联的数据存取对象
            dataInfoObject.DataItem.SetRootControlDataAccessObj(dataInfoObject.AccessObject);

            //检查一下数据是否己被装入
            if (!dataInfoObject.DataItem.HasBeenLoadFromStorage)
            {
                //装入数据
                IDataInfo dataObj = dataInfoObject.AccessObject.GetDataInfoObjectByPath(newSelectedNode.Path);

                if (dataObj != null)
                {
                    //给将己装入数据的对象添加对主窗体的引用
                    if (NodeFactory._mainWindow != null)
                    {
                        dataObj.MainWindow = NodeFactory._mainWindow;
                    }
                    //把节点数据对象挂到节点上
                    newSelectedNode.NodeData.DataItem = dataObj;

                }
            }

            if (dataInfoObject.DataItem.ShouldEmbedInHostWorkingBench)
            {
                if (dataInfoObject.DataItem.RootControl.Parent != null)
                {
                    (dataInfoObject.DataItem.RootControl.Parent as ContentControl).Content = null;
                }

            ( NodeUIContainer.Content as LabelNodeControl ).InnerContainer.Content= dataInfoObject.DataItem.RootControl;


                if (dataInfoObject.DataItem.RootControl is InfoNodeControl)
                {
                    (dataInfoObject.DataItem.RootControl as InfoNodeControl).InnerSuperRichTextBox.EditorTool = EditorTool;

                }


                //根节点不显示内容
                if (newSelectedNode.Parent is TreeView)
                {
                    NodeUIContainer.Content = null;

                }


            }
            //显示最新的数据
            dataInfoObject.DataItem.BindToRootControl();
            dataInfoObject.DataItem.RefreshDisplay();

        }

        public void Handle(FileCountChangedEvent e)
        {
            TreeViewIconsItem node = OutLineViewObj.SuperTree.Nodes.FirstOrDefault(n => n.Path == e.DataInfo.Path);
            if (node != null)
            {

                if (e.IsHaveFile)
                { 
                    ChangeNodeIcon(node, node.NodeData.DataItem.NewIcon);
                    node.NodeData.DataItem.IconType = "FileIcon";
                    SaveTreeToDB(OutLineViewObj.SuperTree);
                }
                else
                {
                    ChangeNodeIcon(node, node.NodeData.DataItem.NormalIcon);
                    node.NodeData.DataItem.IconType = "InfoNode";
                    SaveTreeToDB(OutLineViewObj.SuperTree);
                }
            }
        }
    }
}

