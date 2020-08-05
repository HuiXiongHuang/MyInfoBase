using DataAccessLayer;
using DataAccessLayer.InfoNodeDA;
using DataAccessLayer.LabelNodeDA;
using InfoNode;
using InterfaceLibrary;
using LabelNode;
using Model;
using NodeFactoryLibrary;
using PublicLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFDBInfoTab;
using WPFSuperTreeView;

namespace MyInfoBase
{
    /// <summary>
    /// LabelWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LabelWindow : Window
    {
        public LabelWindow()
        {
            InitializeComponent();
        }
        public LabelWindow(MainWindow mainWindow, DBInfoTab curDbInfoTab,InfoNodeAccess access,InfoNodeDataInfo datainfo)
        {
            InitializeComponent();
            labeltree.TreeNodeType = "LabelNode";
            MainWin = mainWindow;
            this.curDbInfoTab = curDbInfoTab;
           // SelectedInfoNodeDataObj = curDbInfoTab.CurrentTreeView.SelectedItem.NodeData ;
            InfoNodeAccessObj = access;
            InfoNodeDataInfoObj = datainfo;
            LoadLabelTree();
        }
       //private String EFConnectString { get; set; }
       private DBInfoTab curDbInfoTab;
       private MainWindow MainWin { get; set; }
      // private NodeDataObject SelectedInfoNodeDataObj;
        private String LastPasteNodeText = "";
        public SuperTreeView LabelTreeView 
        { 
            get
            {
                return labeltree;
            } 
        }
        InfoNodeAccess InfoNodeAccessObj { get; set; }
        InfoNodeDataInfo InfoNodeDataInfoObj { get; set; }

        /// <summary>
        /// 从数据库中装入标签树
        /// </summary>
        /// <param name="argu"></param>
        private void LoadLabelTree()
        {

            //创建连接字符串
            String EFConnectString = DALConfig.getEFConnectionString(curDbInfoTab.dbInfoObject.DatabaseFilePath);
           // this.Cursor = Cursors.AppStarting;



            Task tsk = new Task(() =>
            {


                labeltree.EFConnectionString = EFConnectString;

                    String treeXML = labeltree.LoadTreeXMLFromDB();

                Action afterFetchTreeXML = () =>
                {

                    labeltree.LoadFromXmlString(treeXML);
                    //MenuItem mnuChangeTextColor = labeltree.ContextMenu.Items[labeltree.ContextMenu.Items.Count - 1] as MenuItem;
                   //Cursor = null;
                    //设置己从数据库中装入标记
                    //curDbInfoTab.HasBeenLoadedFromDB = true;
                };
                Dispatcher.BeginInvoke(afterFetchTreeXML);


            });

            tsk.Start();


        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void treeView1_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void treeView1_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void btnAddRoot_Click(object sender, RoutedEventArgs e)
        {
        
            AddNode(AddNodeCategory.AddRoot);
        }

        private void btnAddSibling_Click(object sender, RoutedEventArgs e)
        {
         
            AddNode(AddNodeCategory.AddSibling);
        }

        private void btnAddChild_Click(object sender, RoutedEventArgs e)
        {

            AddNode(AddNodeCategory.AddChild);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            //AddLabelToInfoNode();
            //AddInfoNodeToLabel();
            CreateAssociation();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //保存树结构
            curDbInfoTab.SaveTreeToDB(LabelTreeView);
            this.Close();
          
        }
        private void AddNode(AddNodeCategory category)
        {
            string treeNodeType = labeltree.TreeNodeType;
            //为新节点生成默认文本
            String NodeText = MainWin.getDefaultNodeText(treeNodeType) + (labeltree.NodeCount + 1);
            //尝试从剪贴板中提取文本
            String textFromClipboard = StringUtils.getFirstLineOfString(Clipboard.GetText());
            if (String.IsNullOrEmpty(textFromClipboard) == false && textFromClipboard != LastPasteNodeText && textFromClipboard.IndexOf("/") == -1)
            {
                //检测一下从剪贴板中获取的文本是否有效（即不会导致重名的节点出现）
                String newNodeText = textFromClipboard;
                bool nodeExisted = labeltree.IsNodeExisted(MainWin.getNewNodePath(category, newNodeText));
                //如果不存在同名的路径
                if (nodeExisted == false)
                {
                    //NodeText = newNodeText;
                    //LastPasteNodeText = NodeText;
                }
            }
            //如果还有重复路径的，则循环使用随机数，务必保证路径不会相同
            while (labeltree.IsNodeExisted(MainWin.getNewNodePath(category, NodeText)))
            {
                NodeText = MainWin.getDefaultNodeText(treeNodeType) + new Random().Next()as string;
            }

            //创建默认的节点数据对象
            NodeDataObject dataobject = NodeFactory.CreateDataInfoNode(treeNodeType, DALConfig.getEFConnectionString(curDbInfoTab.dbInfoObject.DatabaseFilePath));

            TreeViewIconsItem newNode = null;
            //在树中添加节点
            switch (category)
            {
                case AddNodeCategory.AddRoot:
                    newNode = labeltree.AddRoot(NodeText, dataobject);
                    curDbInfoTab.LabelViewObj.SuperTree.AddRoot(NodeText, dataobject);
                    break;
                case AddNodeCategory.AddChild:
                    newNode = labeltree.AddChild(NodeText, dataobject);
                    curDbInfoTab.LabelViewObj.SuperTree.AddChild(NodeText, dataobject);
                    break;
                case AddNodeCategory.AddSibling:
                    newNode = labeltree.AddSibling(NodeText, dataobject);
                    curDbInfoTab.LabelViewObj.SuperTree.AddSibling(NodeText, dataobject);
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
            curDbInfoTab.SaveTreeToDB(labeltree);
            //自动进入编辑状态
            newNode.BeginEdit();

           
        }

        /// <summary>
        /// 给信息节点添加标签关联，当用户在树中删除信息节点时，信息节点及其关联的“标签关联关系”会删除，但不会删除标签本身。在DataGrid中删除标签时，仅会删除信息节点中的标签关联关系。
        /// </summary>
        private void AddLabelToInfoNode()
        {
            //var infoNodeDataObj = SelectedInfoNodeDataObj.DataItem as InfoNodeDataInfo;
            //var infoNodeAccessObj = SelectedInfoNodeDataObj.AccessObject as InfoNodeAccess;
            var labelNodeAccessObj = labeltree.SelectedItem.NodeData.AccessObject as LabelNodeAccess;
            var labelNodeDataObj = labeltree.SelectedItem.NodeData.DataItem as LabelNodeDataInfo;
             string selectedlabel = labeltree.SelectedItem.HeaderText;
            DBLabelInfo labelInfo = new DBLabelInfo()
            {
                ModifyTime = DateTime.Now,
                Path= labelNodeDataObj.Path,
                Label = selectedlabel
            };

            if (InfoNodeDataInfoObj.AttachLabels.IndexOf(labelInfo) == -1)
            {
       
                LabelNodeDB labelNodeDB = labelNodeAccessObj.GetLabelNodeDBWithoutInfoNodeDBsByPath(labelNodeDataObj.Path);
                //根据选中的信息节点的路径将标签添加到数据库中信息节点关联的标签集合中，仅进行信息节点与标签之间的关联添加，并不创建标签。
                InfoNodeAccessObj.AddLabelAssociation(InfoNodeDataInfoObj.Path, labelInfo, labelNodeDB);
                InfoNodeDataInfoObj.AttachLabels.Add(labelInfo);
            }
        }
        /// <summary>
        /// 给标签节点添加信息节点关联，当用户在树中删除标签节点时，标签节点及其关联的“信息节点关联关系”会删除，但不会删除信息节点本身。
        private void AddInfoNodeToLabel()
        {
            //var dataObj = labeltree.SelectedItem.NodeData.DataItem as LabelNodeDataInfo; 如此获得的数据对象不是数据库信息选项卡中的标签节点的数据对象
            //var accessObj = labeltree.SelectedItem.NodeData.AccessObject as LabelNodeAccess;
            string labelnodePath = labeltree.SelectedItem.NodeData.DataItem.Path;
            //查找节点
         TreeViewIconsItem nodeNeedToAddInfoNode= curDbInfoTab.LabelViewObj.SuperTree.Nodes.FirstOrDefault(n => n.Path == labelnodePath);
            var dataObj = nodeNeedToAddInfoNode.NodeData.DataItem as LabelNodeDataInfo;
             var accessObj = nodeNeedToAddInfoNode.NodeData.AccessObject as LabelNodeAccess;
            DBInfoNodeInfo infoNodeInfo = new DBInfoNodeInfo()
                {
                    ModifyTime = DateTime.Now,             
                  //Path= curDbInfoTab.CurrentTreeView.SelectedItem.NodeData.DataItem.Path
                  Path=InfoNodeDataInfoObj.Path

                };
           
            
            //不加入重复的信息节点
            if (dataObj.AttachInfoNodeInfos.IndexOf(infoNodeInfo) == -1)
            {
                //创建连接字符串
                String EFConnectionString = DALConfig.getEFConnectionString(curDbInfoTab.dbInfoObject.DatabaseFilePath);
                InfoNodeRepository repository = new InfoNodeRepository(EFConnectionString);
                //InfoNodeDataInfo obj = SelectedInfoNodeDataObj.DataItem as InfoNodeDataInfo;
                InfoNodeDB dbobj = repository.GetInfoNodeDBWithoutFileInfosByPath(InfoNodeDataInfoObj.Path);
                //给标签节点添加标签节点与信息节点的关联
                accessObj.AddInfoNodeAssociation(dataObj.Path, infoNodeInfo, dbobj);
                dataObj.AttachInfoNodeInfos.Add(infoNodeInfo);
            }

            }
        private void CreateAssociation()
        {
            //创建连接字符串
            String EFConnectionString = DALConfig.getEFConnectionString(curDbInfoTab.dbInfoObject.DatabaseFilePath);
            string labelnodePath = labeltree.SelectedItem.NodeData.DataItem.Path;
            string infoNodePath = InfoNodeDataInfoObj.Path;
            //创建多对多关联时，对信息节点与标签节点在数据库中均存在的，不能直接修改数据库表或实体类，而应该修改关联关系
            using (MyDBEntitiesSqlite context = new MyDBEntitiesSqlite(EFConnectionString))
            {
                var infoNode= context.InfoNodeDBs.FirstOrDefault(p => p.Path == infoNodePath);
                var labelNode = context.LabelNodeDBs.FirstOrDefault(p => p.Path == labelnodePath);
                if (infoNode!=null&& labelNode!=null)
                {

              
                infoNode.LabelNodeDBs.Add(labelNode);
                labelNode.InfoNodeDBs.Add(infoNode);
                    //context.LabelNodeDBs.Add(labelNode);//这句话从字面理解就是数据库的LabelNodeDBs表集合中添加一个labelNode，由于ID是自动生成，相当会复制一份与原有的labelNode内容一致，ID不一致的数据。
                    // context.InfoNodeDBs.Add(infoNode);//不能再添加了否则数据库中会多出一个ID不同的infoNode
                    int r = context.SaveChanges();
                //给标签添加信息绑定
                DBInfoNodeInfo infoNodeInfo = new DBInfoNodeInfo()
                {
                    ID= infoNode.ID,
                    ModifyTime = DateTime.Now,
                    Path = InfoNodeDataInfoObj.Path

                };
                //查找节点
                TreeViewIconsItem nodeNeedToAddInfoNode = curDbInfoTab.LabelViewObj.SuperTree.Nodes.FirstOrDefault(n => n.Path == labelnodePath);
                var dataObj = nodeNeedToAddInfoNode.NodeData.DataItem as LabelNodeDataInfo;
                //不加入重复的信息节点
                if (dataObj.AttachInfoNodeInfos.IndexOf(infoNodeInfo) == -1)
                {
                    dataObj.AttachInfoNodeInfos.Add(infoNodeInfo);
                }
                //给信息添加标签绑定
                DBLabelInfo labelInfo = new DBLabelInfo()
                {
                    ID=labelNode.ID,
                    ModifyTime = DateTime.Now,
                    Path = labelnodePath,
                   // Label = selectedlabel
                };

                if (InfoNodeDataInfoObj.AttachLabels.IndexOf(labelInfo) == -1)
                {
                        InfoNodeDataInfoObj.AttachLabels.Add(labelInfo);
                }
                }
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //保存树结构
            curDbInfoTab.SaveTreeToDB(LabelTreeView);
        }
    }
}
