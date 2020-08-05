using InfoNode;
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
using WPFSuperTreeView;

namespace WPFDBInfoTab
{
    /// <summary>
    /// OutLineTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class OutLineView : UserControl
    {
        public OutLineView()
        {
            InitializeComponent();
            SuperTree.TreeNodeType = "InfoNode";
        }
        public OutLineView(DBInfoTab dBInfoTab)
        {
            InitializeComponent();
            DBInfoTabObj = dBInfoTab;
            SuperTree.TreeNodeType = "InfoNode";
        }
        public DBInfoTab DBInfoTabObj { get; set; }
        public SuperTreeView SuperTree { get { return tree; } }
        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            if (this.SuperTree.TreeNodeType == "InfoNode"&& e.OldValue!=null)
            {
               //  TreeViewIconsItem oldSelectedNode = e.OldValue as TreeViewIconsItem;
                TreeViewIconsItem oldSelectedNode = SuperTree.Nodes.FirstOrDefault(n => n.Path == (e.OldValue as TreeViewIconsItem).Path);
                if (oldSelectedNode != null)
                {
                    var oldIconType = oldSelectedNode.IconType;

                    var nodeFileCount = (oldSelectedNode.NodeData.DataItem as InfoNodeDataInfo).AttachFiles.Count;
                    var nodeDataInfo = oldSelectedNode.NodeData.DataItem as InfoNodeDataInfo;

                    if (nodeFileCount <= 0)
                    {

                       // string nullRtf = "{\\rtf1\\ansi\\ansicpg1252\\uc1\\htmautsp\\deff2{\\fonttbl{\\f0\\fcharset0 Times New Roman;}{\\f2\\fcharset0 Microsoft YaHei UI;}}{\\colortbl\\red0\\green0\\blue0;\\red255\\green255\\blue255;}\\loch\\hich\\dbch\\pard\\plain\\ltrpar\\itap0{\\lang1033\\fs30\\f2\\cf0 \\cf0\\ql\\sl30\\slmult0{\\f2 {\\lang2052\\ltrch }\\li0\\ri0\\sa0\\sb0\\fi0\\ql\\sl30\\slmult0\\par}\r\n}\r\n}";
                
                        if (nodeDataInfo.RTFText==null||nodeDataInfo.RTFText==""|| nodeDataInfo.RTFText.Length<=326) 
                        {
                            oldSelectedNode.Icon = nodeDataInfo.NoInfoIcon;
                                oldSelectedNode.IconType = "NoInfoIcon";
                        }
                       else if (nodeDataInfo.RTFText.IndexOf(@"{\pict\") > -1)
                        {
                            oldSelectedNode.Icon = nodeDataInfo.ImageIcon;
                            oldSelectedNode.IconType = "ImageIcon";
                        }
                        else
                        {
                            oldSelectedNode.Icon = nodeDataInfo.NormalIcon;
                            oldSelectedNode.IconType = "InfoNode";
                        }
                     
                    }

                    if (nodeFileCount > 0)
                    {
                        oldSelectedNode.Icon = nodeDataInfo.FileIcon;                      
                        oldSelectedNode.IconType = "FileIcon";

                    }
                    var newIconType = oldSelectedNode.IconType;

                    if (oldIconType == null || oldIconType!= newIconType)//加此判断语句，不必每次切换节点都链接数据库消耗计算机资源。
                    {
                        //不能将此步骤移动至数据库选项卡关闭或移动主窗口关闭时，因为会带来新的bug，当未加载完成时关闭APP会导致，保存为空树。
                        DBInfoTabObj.SaveTreeToDB(SuperTree);
                    }

                       
                }
            }

            TreeViewIconsItem newSelectedNode = e.NewValue as TreeViewIconsItem;
            if (SuperTree.IsInEditMode)
            {
                if (newSelectedNode != null)
                {
                    newSelectedNode.IsSelected = false;
                }
                return;
            }

            //更换节点图标
            DBInfoTabObj.ChangedSelectedNodeIconWhenClick(sender, e);

            if (newSelectedNode != null)
            {
                DBInfoTabObj.LoadDataAndShowInUI(newSelectedNode);
            }
            else
            {
                //没有选中任何节点，则显示空白的窗体
                SuperTree.Content = null;
            }
            if (DBInfoTabObj.visitedNodesManager != null && e.OldValue != null)
            {
                DBInfoTabObj.visitedNodesManager.AddHistoryRecord((e.OldValue as TreeViewIconsItem).Path);
            }
        }

        private void treeView1_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (SuperTree.SelectedItem == null)
            {
                e.Handled = true;
            }
        }

        private void treeView1_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            IInputElement currentElement = SuperTree.InputHitTest(e.GetPosition(SuperTree));
            TreeViewIconsItem node = DBInfoTabObj.GetNodeUnderMouseCursor(currentElement as DependencyObject);
            if (node != null)
            {

                node.IsSelected = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void treeView1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) && (e.Key == Key.C))
            {
                DBInfoTabObj.CopyNodeText();
            }
        }

    }
}
