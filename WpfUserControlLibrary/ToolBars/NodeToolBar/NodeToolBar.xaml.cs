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

namespace WpfUserControlLibrary.ToolBars.NodeToolBar
{
    /// <summary>
    /// NodeToolBar.xaml 的交互逻辑
    /// </summary>
    public partial class NodeToolBar : UserControl
    {
        curDbInfoTab
        public NodeToolBar()
        {
            InitializeComponent();
        }

        private void AddChildNode(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void AddSiblingNode(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void AddRootNode(object sender, ExecutedRoutedEventArgs e)
        {
            if (curDbInfoTab.treeView1.IsInEditMode)
            {
                curDbInfoTab.treeView1.SelectedItem.EndEdit();
            }
            AddNode(AddNodeCategory.AddRoot, "Folder");
        }

        private void DeleteNode(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void MoveLeft(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void MoveRight(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void MoveUp(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void MoveDown(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void RenameNode(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void CutNode(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void PasteNode(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void CopyNodeTextExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ExpandAllNode(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ShowFindNodesWindow(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ShowConfigWin(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ExitApplication(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ChangeDB(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void CopyDB(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void GoBack(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void GoForward(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ToggleNodeTextBold(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}
