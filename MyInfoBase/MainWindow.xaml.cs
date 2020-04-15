using DataAccessLayer;
using DataAccessLayer.NodeTreeDA;
using InterfaceLibrary;
using NodeFactoryLibrary;
using WPFDBInfoTab.BackAndForward;
using PublicLibrary.Serializer;
using PublicLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using WPFSuperTreeView;
using WPFUserControlLibrary;
using WPFUserControlLibrary.InfoTab;
using WPFDBInfoTab;
using WinForms = System.Windows.Forms;
using WPFSuperRichTextBox;
using InfoNode;
using Model;
using DataAccessLayer.LabelNodeDA;
using System.Collections.ObjectModel;
using DataAccessLayer.InfoNodeDA;
using LabelNode;
using MessageBox = WPFMessageBoxView.MessageBox;
namespace MyInfoBase
{

    public partial class MainWindow : Window, IMainWindowFunction, IHandler<AddLabelEvent>, IHandler<RemoveLabelEvent>
    {
        /// <summary>
        /// 代表主窗体默认显示的标题
        /// </summary>
        private const String MainWindowDefaultTitle = "我的信息基地";

        public MainWindow()
        {
            InitializeComponent();
            Init();

        }
        /// <summary>
        /// 表示当前激活的数据库选项卡
        /// </summary>
        private DBInfoTab curDbInfoTab = null;
        /// <summary>
        /// 用于查找的窗口
        /// </summary>
        private FindNodes findNodesWindow = null;
        private LabelWindow labelWin = null;

        public EditorToolBar EditorTool
        {
            get { return editorTool; }
        }
        public TextBox GetFontSizeTextBox()
        {
            return editorTool.InnerFontSizeTextBox;
        }
        public RichTextBox GetRichTextBox()
        {
            if (curDbInfoTab == null) return null;
            return curDbInfoTab.CurrentRichTextBox;
        }

        /// <summary>
        /// 完成系统初始化功能
        /// </summary>
        private void Init()
        {
            //设置NodeFactory的主窗体引用，以便让它创建节点对象时，一并将主窗体引用传给节点
            //从而允许节点调用主窗体的功能，此句代码必须在装入全树节点前完成，否则，节点的DataItem对象
            //将无法引用到主窗体
            NodeFactory._mainWindow = this;
            //DBInfoTab._mainWindow = this;
            SuperWPFRichTextBox._mainWindow = this;
            EditorToolBar._mainWindow = this;
            //订阅事件
            AddLabelEventManager addLabelEventManager = new AddLabelEventManager();
            addLabelEventManager.Register(this);
            RemoveLabelEventManager removeLabelEventManager = new RemoveLabelEventManager();
            removeLabelEventManager.Register(this);


            ConfigArgus argu = new ConfigArgus();
            tbVersionInfo.Text = String.Format("版本: {0} ", ConfigArgus.version);
            //如果找到了配置文件
            if (File.Exists(SystemConfig.ConfigFileName))
            {
                tbInfo.Text = "正在加载配置文件……";
                //定位到上次访问的节点
                try
                {
                    argu = DeepSerializer.BinaryDeserialize(SystemConfig.ConfigFileName) as ConfigArgus;
                }
                catch (Exception)
                {
                    argu = null;
                }

                if (argu != null)
                {
                    SystemConfig.configArgus = argu;
                    //设置树节点的默认字体大小
                    TreeViewIconsItem.TreeNodeDefaultFontSize = argu.TreeNodeDefaultFontSize;
                }
                findNodesWindow = new FindNodes();

                //labelWindow = new LabelWindow();

                //用于保存有效的数据库文件对象
                List<DatabaseInfo> validDBInfos = new List<DatabaseInfo>();
                //创建数据库选项卡
                foreach (var dbinfo in argu.DbInfos)
                {
                    if (File.Exists(dbinfo.DatabaseFilePath))
                    {
                        DBInfoTab tab = new DBInfoTab(dbinfo);
                        DBtabContainer.Add(System.IO.Path.GetFileName(dbinfo.DatabaseFilePath), tab);
                        validDBInfos.Add(dbinfo);
                    }
                }
                bool allDBAreOk = (argu.DbInfos.Count == validDBInfos.Count);
                if (!allDBAreOk)
                {
                    argu.DbInfos = validDBInfos;
                }
                if (DBtabContainer.Items.Count != 0)
                {

                    //设置当前激活的卡片
                    if (argu.ActiveDBIndex < DBtabContainer.Items.Count && allDBAreOk)
                    {
                        DBtabContainer.SelectedIndex = argu.ActiveDBIndex;

                    }
                    else
                    {
                        argu.ActiveDBIndex = 0;
                        DBtabContainer.SelectedIndex = 0;
                    }

                    curDbInfoTab = (DBtabContainer.Items[argu.ActiveDBIndex] as TabItem).Content as DBInfoTab;
                    curDbInfoTab.EditorTool = EditorTool;
                    //显示程序退出时最后一次展现的数据库信息选项卡内部的选项卡视图
                    curDbInfoTab.InnerTabControl.SelectedIndex = curDbInfoTab.dbInfoObject.LastTabViewIndex;

                    LoadCurrentTabDataFromDB();


                }
                else
                {
                    tbInfo.Text = "未能找到上次打开的数据库，请从系统功能菜单中选择打开命令打开一个资料库";
                }
            }
            else
            {
                tbInfo.Text = "请从系统功能菜单中选择打开命令打开一个资料库";
            }
            //响应用户点击不同选项卡的操作,此处添加事件处理程序而不是在xaml文件中添加可以避免在初始化时执行一次。
            DBtabContainer.SelectionChanged += DBtabContainer_SelectionChanged;
            //关闭选项卡时，激发此事件
            DBtabContainer.TabPageClosed += DBtabContainer_TabPageClosed;
        }
        private void DBtabContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //打开数据库，首次选择节点时，附加栏选项卡的SelectionChanged事件被触发，此处用于屏蔽此路由事件，也可在事件源屏蔽。
            if (e.Source.GetType().Name == "DBInfoTab")
            {
                return;
            }
            //初始化时，此方法不做任何事，仅设置默认标题,下面语句无法执行
            if (DBtabContainer.SelectedIndex == -1)
            {
                this.Title = MainWindowDefaultTitle;
                return;
            }
            if (e.AddedItems.Count == 1 && e.AddedItems[0].GetType().Name != "TabItem")
            {
                return;
            }
            //初始化时，此方法不做任何事，仅设置默认标题
            if (e.OriginalSource.GetType().Name == "InfoTabControl")
            {
                //if (e.RemovedItems.Count <= 0)
                //{

                curDbInfoTab = (DBtabContainer.Items[DBtabContainer.SelectedIndex] as TabItem).Content as DBInfoTab;
                curDbInfoTab.EditorTool = EditorTool;
                curDbInfoTab.InnerTabControl.SelectedIndex = curDbInfoTab.dbInfoObject.LastTabViewIndex;

                this.Title = MainWindowDefaultTitle;
                if (curDbInfoTab.HasBeenLoadedFromDB == false)
                {
                    //从数据库中装入数据
                    LoadCurrentTabDataFromDB();
                    curDbInfoTab.HasBeenLoadedFromDB = true;
                }
                //return;
                //}
            }
            //TabControl的SelectionChanged事件，虽然可以通过在下层控件的事件响应过程中添加
            //e.Handled = true阻止这一过程，但还是在此屏蔽掉此事件的响应代码，以免有漏网之鱼
            if (e.AddedItems.Count == 1 && e.AddedItems[0].GetType().Name != "TabItem")
            {
                return;
            }
            // this.Title = "我的信息基地-" + curDbInfoTab.dbInfoObject.DatabaseFilePath;
            this.Title = "我的信息基地";
            if (curDbInfoTab.HasBeenLoadedFromDB == false)
            {
                //从数据库中装入数据
                LoadCurrentTabDataFromDB();
                curDbInfoTab.HasBeenLoadedFromDB = true;
            }
            else
            {
                String EFConnectString = DALConfig.getEFConnectionString(curDbInfoTab.dbInfoObject.DatabaseFilePath);
                curDbInfoTab.CurrentTreeView.EFConnectionString = EFConnectString;
                findNodesWindow.SetTree(curDbInfoTab.OutLineViewObj.SuperTree);
                curDbInfoTab.RefreshDisplay();
            }
            //切换选项卡时，恢复状态栏默认显示文本
            if (cutNode == null)
            {
                ShowInfo("就绪");
            }
        }
        /// <summary>
        /// 线程安全的在主窗体上显示信息的方法
        /// </summary>
        /// <param name="info"></param>
        public void ShowInfo(string info)
        {
            if (Dispatcher.CheckAccess())
            {
                tbInfo.Text = info;
            }
            else
            {
                Action<string> showInfoDel = (str) =>
                {
                    tbInfo.Text = info;
                };
                Dispatcher.BeginInvoke(showInfoDel, info);
            }
        }
        void DBtabContainer_TabPageClosed(object sender, TabPageClosedEventArgs e)
        {
            //保存被关闭的选项卡的数据
            DBInfoTab closedTab = e.ClosedTabItem.Content as DBInfoTab;
            //如果被关闭的是包容有被剪切节点的选项卡，则设置相关的变量为null
            if (cutNodeSourceTab != null && cutNodeSourceTab == closedTab)
            {
                cutNodeSourceTab = null;
                cutNode = null;
            }
            //从参数中移除本选项卡所对应的DbInfo对象
            int index = SystemConfig.configArgus.DbInfos.IndexOf(closedTab.dbInfoObject);
            if (index != -1)
            {
                SystemConfig.configArgus.DbInfos.RemoveAt(index);
            }
            SaveDbTabDataToDB(closedTab);
        }
        /// <summary>
        /// 将指定选项卡的数据保存到数据库中
        /// </summary>
        /// <param name="infoTab"></param>
        private void SaveDbTabDataToDB(DBInfoTab infoTab)
        {


            TreeViewIconsItem selectedItem = infoTab.CurrentTreeView.SelectedItem;
            DatabaseInfo info = infoTab.dbInfoObject;
            if (selectedItem != null)
            {
                if (infoTab.CurrentTreeView == infoTab.OutLineViewObj.SuperTree || infoTab.CurrentTreeView == infoTab.LabelViewObj.SuperTree)
                {
                    info.LastVisitNodePath = selectedItem.Path;
                    if (infoTab.CurrentTreeView == infoTab.OutLineViewObj.SuperTree)
                    {
                        infoTab.dbInfoObject.LastTabViewIndex = 0;
                    }
                    if (infoTab.CurrentTreeView == infoTab.LabelViewObj.SuperTree)
                    {
                        infoTab.dbInfoObject.LastTabViewIndex = 1;
                    }

                }

                IDataAccess accessobj = selectedItem.NodeData.AccessObject;
                IDataInfo infoObj = selectedItem.NodeData.DataItem;
                if (accessobj != null)
                {
                    try
                    {
                        infoObj.RefreshMe();
                        accessobj.UpdateDataInfoObject(infoObj);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new Action(() => { MessageBox.ShowInformation(ex.ToString()); }));
                    }
                }
            }
        }
        /// <summary>
        /// 从数据库中为当前激活的选项卡装入信息
        /// </summary>
        /// <param name="argu"></param>
        public void LoadCurrentTabDataFromDB()
        {

            //绑定显示数据源
            if (findNodesWindow == null)
            {
                findNodesWindow = new FindNodes();
            }

            //创建连接字符串
            String EFConnectString = DALConfig.getEFConnectionString(curDbInfoTab.dbInfoObject.DatabaseFilePath);

            //this.Title = "我的信息基地-" + curDbInfoTab.dbInfoObject.DatabaseFilePath;
            this.Title = "我的信息基地";
            this.Cursor = Cursors.AppStarting;
            //profiler发现，GetTreeFromDB()需要花费大量的时间，因此，将其移到独立的线程中去完成
            tbInfo.Text = "从数据库中装载数据……";
            foreach (SuperTreeView treeView in curDbInfoTab.InnerTreeViewList)
            {
                Task tsk = new Task(() =>
                {

                    treeView.EFConnectionString = EFConnectString;

                    String treeXML = treeView.LoadTreeXMLFromDB();

                    Action afterFetchTreeXML = () =>
                    {

                        treeView.LoadFromXmlString(treeXML);
                        treeView.ShowNode(curDbInfoTab.dbInfoObject.LastVisitNodePath);


                        //绑定树节点集合到查找窗体
                        findNodesWindow.SetTree(curDbInfoTab.OutLineViewObj.SuperTree);
                        curDbInfoTab.visitedNodesManager = new VisitedNodesManager(treeView);



                        MenuItem mnuChangeTextColor = treeView.ContextMenu.Items[treeView.ContextMenu.Items.Count - 1] as MenuItem;

                        ColorBrushList brushList = new ColorBrushList(mnuChangeTextColor);
                        brushList.BrushChanged += brushList_BrushChanged;
                        tbInfo.Text = "就绪。";
                        Cursor = null;
                        ////设置己从数据库中装入标记
                        curDbInfoTab.HasBeenLoadedFromDB = true;


                    };
                    Dispatcher.BeginInvoke(afterFetchTreeXML);


                });

                tsk.Start();


            }
        }


        void brushList_BrushChanged(object sender, BrushChangeEventArgs e)
        {
            if (curDbInfoTab.CurrentTreeView.SelectedItem != null)
            {
                curDbInfoTab.CurrentTreeView.SelectedItem.MyForeground = e.selectedBrush;
                curDbInfoTab.SaveTreeToDB();
            }
        }



        /// <summary>
        /// 剪切的节点
        /// </summary>
        private TreeViewIconsItem cutNode = null;
        /// <summary>
        /// 保存上次粘贴的文本
        /// </summary>
        private String LastPasteNodeText = "";



        #region "添加节点"

        /// <summary>
        /// 依据要添加节点的种类和节点文本，生成其路径，可用于检测是否己存在同名节点
        /// </summary>
        /// <param name="category"></param>
        /// <param name="newNodeText"></param>
        /// <returns></returns>
        public String getNewNodePath(AddNodeCategory category, String newNodeText)
        {
            String selectNodePath = (curDbInfoTab.CurrentTreeView.SelectedItem as TreeViewIconsItem) == null ? "" : (curDbInfoTab.CurrentTreeView.SelectedItem as TreeViewIconsItem).Path;
            String newNodePath = "";
            switch (category)
            {
                case AddNodeCategory.AddRoot:
                    newNodePath = "/" + newNodeText + "/";
                    break;
                case AddNodeCategory.AddChild:
                    if (selectNodePath != "")
                    {
                        newNodePath = selectNodePath + newNodeText + "/";
                    }

                    break;
                case AddNodeCategory.AddSibling:
                    if (selectNodePath != "")
                    {
                        String selectNodeText = (curDbInfoTab.CurrentTreeView.SelectedItem as TreeViewIconsItem).HeaderText;
                        newNodePath = selectNodePath.Replace("/" + selectNodeText + "/", "/" + newNodeText + "/");
                    }

                    break;
            }
            return newNodePath;
        }
        /// <summary>
        /// 依据节点类型，生成节点默认的文本（不包括后面的数字）
        /// </summary>
        /// <param name="treeType"></param>
        /// <returns></returns>
        public String getDefaultNodeText(String treeType)
        {
            String defaultNodeText = "";
            switch (treeType)
            {
                case "LabelNode":
                    defaultNodeText = "新标签节点";
                    break;
                case "InfoNode":
                    defaultNodeText = "新信息节点";
                    break;
            }
            return defaultNodeText;
        }
        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="category">指要新建儿子、兄弟还是根节点</param>
        /// <param name="treeType">指节点的类型是：DetailText\OnlyText等 </param>
        private void AddNode(AddNodeCategory category)
        {
            string treeNodeType = curDbInfoTab.CurrentTreeView.TreeNodeType;
            //为新节点生成默认文本
            String NodeText = getDefaultNodeText(treeNodeType) + (curDbInfoTab.CurrentTreeView.NodeCount + 1);
            //尝试从剪贴板中提取文本
            String textFromClipboard = StringUtils.getFirstLineOfString(Clipboard.GetText());
            if (String.IsNullOrEmpty(textFromClipboard) == false && textFromClipboard != LastPasteNodeText && textFromClipboard.IndexOf("/") == -1)
            {
                //检测一下从剪贴板中获取的文本是否有效（即不会导致重名的节点出现）
                String newNodeText = textFromClipboard;
                bool nodeExisted = curDbInfoTab.CurrentTreeView.IsNodeExisted(getNewNodePath(category, newNodeText));
                //如果不存在同名的路径
                if (nodeExisted == false)
                {
                    //NodeText = newNodeText;
                    //LastPasteNodeText = NodeText;
                }
            }
            //如果还有重复路径的，则循环使用随机数，务必保证路径不会相同
            while (curDbInfoTab.CurrentTreeView.IsNodeExisted(getNewNodePath(category, NodeText)))
            {
                NodeText = getDefaultNodeText(treeNodeType) + new Random().Next();
            }

            //创建默认的节点数据对象
            NodeDataObject dataobject = NodeFactory.CreateDataInfoNode(treeNodeType, DALConfig.getEFConnectionString(curDbInfoTab.dbInfoObject.DatabaseFilePath));

            TreeViewIconsItem newNode = null;
            //在树中添加节点
            switch (category)
            {
                case AddNodeCategory.AddRoot:
                    newNode = curDbInfoTab.CurrentTreeView.AddRoot(NodeText, dataobject);
                    break;
                case AddNodeCategory.AddChild:
                    newNode = curDbInfoTab.CurrentTreeView.AddChild(NodeText, dataobject);
                    break;
                case AddNodeCategory.AddSibling:
                    newNode = curDbInfoTab.CurrentTreeView.AddSibling(NodeText, dataobject);
                    break;
                default:
                    break;
            }
            if (newNode == null)
            {
                return;
            }
            //新节点，默认不是粗体的
            newNode.FontWeight = FontWeights.Normal;
            //在数据库中创建记录
            if (dataobject.AccessObject != null)
            {
                dataobject.AccessObject.Create(dataobject.DataItem);
            }
            //保存树结构
            curDbInfoTab.SaveTreeToDB();
            //自动进入编辑状态
            newNode.BeginEdit();
        }

        private void AddChild(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab == null) return;
            if (curDbInfoTab.CurrentTreeView.IsInEditMode)
            {
                curDbInfoTab.CurrentTreeView.SelectedItem.EndEdit();
            }
            AddNode(AddNodeCategory.AddChild);
        }

        private void AddSibling(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab == null) return;
            if (curDbInfoTab.CurrentTreeView.IsInEditMode)
            {
                curDbInfoTab.CurrentTreeView.SelectedItem.EndEdit();
            }
            AddNode(AddNodeCategory.AddSibling);
        }

        private void AddRoot(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab == null) return;


            if (curDbInfoTab.CurrentTreeView.IsInEditMode)
            {
                curDbInfoTab.CurrentTreeView.SelectedItem.EndEdit();
            }
            AddNode(AddNodeCategory.AddRoot);
        }

        #endregion

        private void DeleteNode(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab == null) return;
            if (curDbInfoTab.CurrentTreeView.IsInEditMode)
            {
                curDbInfoTab.CurrentTreeView.SelectedItem.EndEdit();
            }
         



            //删除多级节点时询问用户
            TreeViewIconsItem selectedNode = curDbInfoTab.CurrentTreeView.SelectedItem as TreeViewIconsItem;
            if (selectedNode != null && curDbInfoTab.CurrentTreeView.GetChildren(selectedNode).Count > 0)
            {

                MessageBoxResult result = MessageBox.Show("您将要删除一个包含有下级信息的节点，删除操作不可恢复，真的进行吗？", "删除节点", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    return;
                }

            }


            curDbInfoTab.CurrentTreeView.DeleteNode(selectedNode);
            //保存树结构
            curDbInfoTab.SaveTreeToDB();

            //此时可能出现一个WPF中的bug，当删除树窗口中最后一个节点时，会导致树窗口内部卡死，添加的节点实际存在但无法显示在树窗口中，于是在此处重新添加视图，以避开此bug
            if (curDbInfoTab.CurrentTreeView.Nodes.Count == 0)
            {
                //findNodesWindow.ShouldExit = true;
                //Close();
                //Process.Start(Assembly.GetEntryAssembly().Location);
                if (curDbInfoTab.CurrentTreeView.TreeNodeType == "InfoNode")
                {

                    curDbInfoTab.OutLineViewObj = new OutLineView(curDbInfoTab);
                    curDbInfoTab.OutLineViewObj.SuperTree.EFConnectionString = DALConfig.getEFConnectionString(curDbInfoTab.dbInfoObject.DatabaseFilePath);
                    curDbInfoTab.InnerTabControl.SelectedIndex = 1;
                    curDbInfoTab.InnerTabControl.SelectedIndex = 0;

                    MenuItem mnuChangeTextColor = curDbInfoTab.OutLineViewObj.SuperTree.ContextMenu.Items[curDbInfoTab.OutLineViewObj.SuperTree.ContextMenu.Items.Count - 1] as MenuItem;
                    ColorBrushList brushList = new ColorBrushList(mnuChangeTextColor);
                    brushList.BrushChanged += brushList_BrushChanged;
                    //绑定树节点集合到查找窗体
                    findNodesWindow.SetTree(curDbInfoTab.OutLineViewObj.SuperTree);

                }
                if (curDbInfoTab.CurrentTreeView.TreeNodeType == "LabelNode")
                {

                    curDbInfoTab.LabelViewObj = new LabelView(curDbInfoTab);
                    curDbInfoTab.LabelViewObj.SuperTree.EFConnectionString = DALConfig.getEFConnectionString(curDbInfoTab.dbInfoObject.DatabaseFilePath); 
                    curDbInfoTab.InnerTabControl.SelectedIndex = 0;
                    curDbInfoTab.InnerTabControl.SelectedIndex = 1;

                    MenuItem mnuChangeTextColor = curDbInfoTab.LabelViewObj.SuperTree.ContextMenu.Items[curDbInfoTab.LabelViewObj.SuperTree.ContextMenu.Items.Count - 1] as MenuItem;
                    ColorBrushList brushList = new ColorBrushList(mnuChangeTextColor);
                    brushList.BrushChanged += brushList_BrushChanged;
                    //绑定树节点集合到查找窗体
                    findNodesWindow.SetTree(curDbInfoTab.OutLineViewObj.SuperTree);

                }
            }

        }



        private void RenameNode(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab == null) return;
            if (curDbInfoTab.CurrentTreeView.IsInEditMode)
            {
                curDbInfoTab.CurrentTreeView.SelectedItem.EndEdit();
            }
            TreeViewIconsItem selectedNode = curDbInfoTab.CurrentTreeView.SelectedItem as TreeViewIconsItem;
            if (selectedNode != null)
            {
                selectedNode.BeginEdit();
            }

        }

       
        /// <summary>
        /// 当剪切节点时，记录下被剪切节点所在的数据库选项卡
        /// </summary>
        private DBInfoTab cutNodeSourceTab = null;
        private void CutNode(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab == null) return;
            if (curDbInfoTab.CurrentTreeView.IsInEditMode)
            {
                curDbInfoTab.CurrentTreeView.SelectedItem.EndEdit();
            }
            //如果己经有被剪切的节点，则取消它的下划线状态
            if (cutNode != null && cutNode != curDbInfoTab.CurrentTreeView.SelectedItem)
            {
                cutNode.Strikethrough = false;
            }
            cutNode = curDbInfoTab.CurrentTreeView.SelectedItem as TreeViewIconsItem;
            cutNode.Strikethrough = true;
            cutNodeSourceTab = curDbInfoTab;
            String info = "";
            if (cutNode.HeaderText.Length > 30)
            {
                info = "节点：\"" + cutNode.HeaderText.Substring(0, 30) + "\"己被剪切";
            }
            else
            {
                info = "节点：\"" + cutNode.HeaderText + "\"己被剪切";
            }
            ShowInfo(info);
        }

        private void PasteNode(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab == null) return;
            if (curDbInfoTab.CurrentTreeView.IsInEditMode)
            {
                curDbInfoTab.CurrentTreeView.SelectedItem.EndEdit();
            }
            if (cutNode == null || cutNodeSourceTab == null)
            {
                return;
            }
            TreeViewIconsItem selectedNode = curDbInfoTab.CurrentTreeView.SelectedItem as TreeViewIconsItem;
            if (selectedNode == cutNode)
            {
                return;
            }
            String newPath = "";

            if (selectedNode == null)//如果目标选项卡中没有选中任何节点，则默认添加到根节点
            {
                newPath = "/" + cutNode.HeaderText + "/";
            }
            else
            {
                newPath = selectedNode.Path + cutNode.HeaderText + "/";
            }

            if (curDbInfoTab.CurrentTreeView.IsNodeExisted(newPath))
            {
                MessageBox.ShowInformation("在此处粘贴将导致两个节点拥有相同的路径，因此，请在其他地方粘贴");
                return;
            }
            //在同一数据库中粘贴
            if (curDbInfoTab == cutNodeSourceTab)
            {
                //先移除被剪切的子树
                curDbInfoTab.CurrentTreeView.CutNode(cutNode);
                curDbInfoTab.CurrentTreeView.PasteNode(cutNode, selectedNode);

                curDbInfoTab.OnNodeMove(NodeMoveType.NodePaste);
            }
            else
            {
                //在不同的数据库中粘贴
                String sourcePath = cutNode.Path;
                String targetPath = "";
                if (selectedNode != null)
                {
                    targetPath = selectedNode.Path;
                }
                else
                {
                    targetPath = "/";
                }

                //先移除源树中被剪切的子树
                cutNodeSourceTab.CurrentTreeView.CutNode(cutNode);


                //将剪切的节点子树追加到当前节点
                curDbInfoTab.CurrentTreeView.PasteNodeCrossDB(cutNode, selectedNode);

                //保存目标树结构
                curDbInfoTab.CurrentTreeView.SaveToDB();
                //将源树保存到数据库中
                cutNodeSourceTab.CurrentTreeView.SaveToDB();

                //更新所有粘贴节点的数据存取对象
                String EFConnectionString = DALConfig.getEFConnectionString(curDbInfoTab.dbInfoObject.DatabaseFilePath);
                if (selectedNode != null)
                {
                    UpdateDataAcessObject(selectedNode, EFConnectionString);
                }
                else
                {
                    UpdateDataAcessObject(cutNode, EFConnectionString);
                }

                //更新数据库中内容
                NodeMoveBetweenDBManager nodeMoveManager = new NodeMoveBetweenDBManager(cutNodeSourceTab.dbInfoObject.DatabaseFilePath, curDbInfoTab.dbInfoObject.DatabaseFilePath);
                nodeMoveManager.MoveNodeBetweenDB(sourcePath, targetPath);

            }
            //取消删除线显示
            cutNode.Strikethrough = false;
            cutNode = null;
            cutNodeSourceTab = null;
            if (curDbInfoTab.CurrentTreeView.SelectedItem != null)
            {
                curDbInfoTab.CurrentTreeView.SelectedItem.IsExpanded = true;
            }



        }
        private void UpdateDataAcessObject(TreeViewIconsItem root, String EFConnectionString)
        {
            if (root == null)
            {
                return;
            }
            root.NodeData.AccessObject = NodeFactory.CreateNodeAccessObject(root.NodeData.DataItem.NodeType, EFConnectionString);
            foreach (var node in root.Items)
            {
                UpdateDataAcessObject(node as TreeViewIconsItem, EFConnectionString);
            }

        }
        private void CopyNodeTextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            curDbInfoTab.CopyNodeText();
        }

        private void ExpandAllNode(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab.CurrentTreeView.IsInEditMode)
            {
                curDbInfoTab.CurrentTreeView.SelectedItem.EndEdit();
            }
            if (curDbInfoTab.CurrentTreeView.SelectedItem != null)
            {
                curDbInfoTab.CurrentTreeView.SelectedItem.ExpandSubtree();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (findNodesWindow != null)
            {
                findNodesWindow.ShouldExit = true;
                findNodesWindow.Close();
            }
            if (labelWin != null)
            {
                labelWin.Close();
            }

            //用户没有打开任何数据库文件
            if (curDbInfoTab == null)
            {
                return;
            }
            if (curDbInfoTab.CurrentTreeView.IsInEditMode)
            {
                curDbInfoTab.CurrentTreeView.SelectedItem.EndEdit();
            }
            SaveDbTabDataToDB(curDbInfoTab);


            //更新配置文件内容
            DatabaseInfo info = null;
            TreeViewIconsItem selectedItem = null;
            SystemConfig.configArgus.DbInfos.Clear();
            foreach (var item in DBtabContainer.Items)
            {
                selectedItem = ((item as TabItem).Content as DBInfoTab).CurrentTreeView.SelectedItem;
                info = ((item as TabItem).Content as DBInfoTab).dbInfoObject;

                SystemConfig.configArgus.DbInfos.Add(info);
            }
            SystemConfig.configArgus.ActiveDBIndex = DBtabContainer.SelectedIndex;
            DeepSerializer.BinarySerialize(SystemConfig.configArgus, SystemConfig.ConfigFileName);
        }

        private void ShowFindNodesWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab == null) return;
            findNodesWindow.Show();
        }


        private void ShowConfigWin(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab == null) return;
            ConfigWin win = new ConfigWin();
            bool? result = win.ShowDialog();
            if (curDbInfoTab.CurrentTreeView.IsInEditMode)
            {
                curDbInfoTab.CurrentTreeView.SelectedItem.EndEdit();
            }
            if (result.Value && SystemConfig.configArgus.IsArgumentsValueChanged)
            {
                DeepSerializer.BinarySerialize(SystemConfig.configArgus, SystemConfig.ConfigFileName);

                MessageBox.Show("参数修改，请重新启动程序", "重启");

                findNodesWindow.ShouldExit = true;
                Close();
                Process.Start(Assembly.GetEntryAssembly().Location);
            }
        }


        private void ExitApplication(object sender, ExecutedRoutedEventArgs e)
        {
            findNodesWindow.ShouldExit = true;
            Close();
        }


        private void GoBack(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab == null) return;
            curDbInfoTab.visitedNodesManager.GoBack();

        }



        private void GoForward(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab == null) return;
            curDbInfoTab.visitedNodesManager.GoForward();

        }



        private void ToggleNodeTextBold(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab.CurrentTreeView.SelectedItem != null)
            {
                if (curDbInfoTab.CurrentTreeView.SelectedItem.FontWeight != FontWeights.ExtraBold)
                    curDbInfoTab.CurrentTreeView.SelectedItem.FontWeight = FontWeights.ExtraBold;
                else
                    curDbInfoTab.CurrentTreeView.SelectedItem.FontWeight = FontWeights.Normal;
                curDbInfoTab.SaveTreeToDB();
            }
        }


        #region "数据库相关的操作"

        /// <summary>
        /// 更换数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeDB(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            openFileDialog.RestoreDirectory = true;

            openFileDialog.Filter = "sqlite数据库（*.db）|*.db";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String DBFileName = openFileDialog.FileName;
                bool IsDBAlreadyOpen = false;
                foreach (var item in DBtabContainer.Items)
                {
                    if (((item as TabItem).Content as DBInfoTab).dbInfoObject.DatabaseFilePath == DBFileName)
                    {
                        IsDBAlreadyOpen = true;
                        break;
                    }
                }


                if (IsDBAlreadyOpen)
                {
                    MessageBox.ShowInformation("此资料库己被打开");
                    return;
                }
                AddNewDbInfoTabAndLoadData(DBFileName);
            }
        }
        /// <summary>
        /// 打开指定的数据库文件，创建新选项卡，装入数据，新创建的选项卡成为当前选项卡
        /// </summary>
        /// <param name="DBFileName"></param>
        private void AddNewDbInfoTabAndLoadData(String DBFileName)
        {
            DatabaseInfo dbInfo = new DatabaseInfo()
            {
                DatabaseFilePath = DBFileName,
                LastVisitNodePath = ""
            };

            SystemConfig.configArgus.DbInfos.Add(dbInfo);

            //添加选项卡
            DBInfoTab tab = new DBInfoTab(dbInfo);

            //DBtabContainer.SelectedIndex = DBtabContainer.Items.Count - 1;
            DBtabContainer.Add(System.IO.Path.GetFileName(dbInfo.DatabaseFilePath), tab);//Note:新添加选项卡，会激发会激发DBtabContainer的SelectedIndexChanged事件，所以此函数内本语句下面的代码是不会执行的。
            DBtabContainer.SelectedIndex = DBtabContainer.Items.Count - 1;
            SystemConfig.configArgus.ActiveDBIndex = DBtabContainer.Items.Count - 1;
            curDbInfoTab = tab;
            curDbInfoTab.EditorTool = EditorTool;
            //Note: 新加选项卡，会激发DBtabContainer的SelectedIndexChanged事件，在事件响应代码DBtabContainer_SelectionChanged（）
            //中完成了从数据库中装载数据的工作，无需显示调用LoadCurrentTabDataFromDB();方法


        }
        private void CopyDB(object sender, ExecutedRoutedEventArgs e)
        {
          //  String currentDir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            String currentDir = System.Environment.CurrentDirectory;
            String DBTemplateFileName = "";
            if (File.Exists(currentDir + "\\MyDB.db"))
            {
                DBTemplateFileName = currentDir + "\\MyDB.db";
            }
            else
            {
                if (File.Exists(currentDir + "\\template\\MyDB.db"))
                {
                    DBTemplateFileName = currentDir + "\\template\\MyDB.db";
                }
                else
                {
                    MessageBox.ShowInformation("数据库模板文件：templateDB.db未找到");
                    return;
                }
            }


            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "创建新资料库";
            DateTime now = DateTime.Now;
            String fileDateString = now.Year + "_" + now.Month + "_" + now.Day + "_" + now.Hour + "_" + now.Minute;
            saveFileDialog.FileName = "MyDB_" + fileDateString + ".db";
            saveFileDialog.Filter = "sqlite数据库（*.sdf）|*.db";
            saveFileDialog.DefaultExt = "db";

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bool IsDBAlreadyOpen = false;
                foreach (var item in DBtabContainer.Items)
                {
                    if (((item as TabItem).Content as DBInfoTab).dbInfoObject.DatabaseFilePath == saveFileDialog.FileName)
                    {
                        IsDBAlreadyOpen = true;
                        break;
                    }
                }
                if (IsDBAlreadyOpen)
                {
                    MessageBox.ShowInformation("不能选择正在使用的资料库文件名");
                    return;
                }
                //复制数据库
                File.Copy(DBTemplateFileName, saveFileDialog.FileName, true);
                AddNewDbInfoTabAndLoadData(saveFileDialog.FileName);
            }
        }
        #endregion

        private void MoveLeft(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab == null) return;
            curDbInfoTab.MoveLeft(sender, e);
        }

        private void MoveRight(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab == null) return;
            curDbInfoTab.MoveRight(sender, e);
        }

        private void MoveUp(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab == null) return;
            curDbInfoTab.MoveUp(sender, e);
        }

        private void MoveDown(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab == null) return;
            curDbInfoTab.MoveDown(sender, e);
        }

        private void AddLabelToInfoNode(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab == null) return;
            if (curDbInfoTab.CurrentTreeView.TreeNodeType == "LabelNode" || curDbInfoTab.CurrentTreeView.SelectedItem == null)
            {
                return;

            }
            AddLabel(curDbInfoTab.CurrentTreeView.SelectedItem.NodeData.AccessObject as InfoNodeAccess, curDbInfoTab.CurrentTreeView.SelectedItem.NodeData.DataItem as InfoNodeDataInfo);

        }

        private void AddBookMark(object sender, ExecutedRoutedEventArgs e)
        {

        }

        public void AddLabel(InfoNodeAccess access, InfoNodeDataInfo dataInfo)
        {
            if (curDbInfoTab == null) return;

            var labelWindow = new LabelWindow(this, curDbInfoTab, access, dataInfo);
            labelWin = labelWindow;
            labelWindow.Show();

        }
        public void Handle(AddLabelEvent e)
        {

            AddLabel(e.AccessInfo, e.DataInfo);
        }
        public void Handle(RemoveLabelEvent e)
        {


        }
      
       

    }

}
