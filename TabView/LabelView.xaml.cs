using InfoNode;
using PublicLibrary.Utils;
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
    /// LabelTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class LabelView : UserControl
    {
        public LabelView()
        {
            InitializeComponent();
            SuperTree.TreeNodeType = "LabelNode";
        }
        public LabelView(DBInfoTab dBInfoTab)
        {
            InitializeComponent();
            DBInfoTabObj = dBInfoTab;
            SuperTree.TreeNodeType = "LabelNode";
        }
        public DBInfoTab DBInfoTabObj { get; set; }
        public SuperTreeView SuperTree { get { return tree; } }
        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
          
            
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

