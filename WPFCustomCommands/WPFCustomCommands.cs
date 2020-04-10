using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace WPFCustomCommands
{
    /// <summary>
    /// 自定义一些命令，专用于此应用程序
    /// </summary>
   
    public class WPFCustomCommands
    {
        #region 树视图节点工具命令
        /*-------添加子节点------------*/
            public static RoutedUICommand AddChild = new RoutedUICommand();
          

            /*-------添加兄弟节点------------*/
            public static RoutedUICommand AddSibling = new RoutedUICommand();
         

            /*-------添加根节点------------*/
            public static RoutedUICommand AddRoot = new RoutedUICommand();
           

            /*---------删除------*/
            public static RoutedUICommand DeleteNode = new RoutedUICommand();

            /*---------节点的移动------*/
            public static RoutedUICommand MoveLeft = new RoutedUICommand();
            public static RoutedUICommand MoveRight = new RoutedUICommand();
            public static RoutedUICommand MoveUp = new RoutedUICommand();
            public static RoutedUICommand MoveDown = new RoutedUICommand();

            /*---------节点改名------*/
            public static RoutedUICommand RenameNode = new RoutedUICommand();

            /*---------节点的剪切与粘贴------*/
            public static RoutedUICommand CutNode = new RoutedUICommand();
            public static RoutedUICommand PasteNode = new RoutedUICommand();

            /*--------复制节点文本------*/
            public static RoutedUICommand CopyNodeText = new RoutedUICommand();

            /*--------展开全部子树------*/
            public static RoutedUICommand ExpandAllNode = new RoutedUICommand();

            /*-------显示查找窗体------*/
            public static RoutedUICommand ShowFindNodesWindow = new RoutedUICommand();

            /*------类型转换命令----------*/
            public static RoutedUICommand ToDetailText = new RoutedUICommand();
            public static RoutedUICommand ToOnlyText = new RoutedUICommand();
            public static RoutedUICommand ToInfoNode = new RoutedUICommand();

            /*------显示系统配置命令---------*/
            public static RoutedUICommand ShowConfigWin = new RoutedUICommand();

            /*------退出命令--------*/
            public static RoutedUICommand ExitApplication = new RoutedUICommand();

            /*-----切换数据库------*/
            public static RoutedUICommand ChangeDB = new RoutedUICommand();
            /*-----复制数据库------*/
            public static RoutedUICommand CopyDB = new RoutedUICommand();

            /*-----节点访问历史记录------*/
            public static RoutedUICommand GoBack = new RoutedUICommand();
            public static RoutedUICommand GoForward = new RoutedUICommand();

            /*-----设置节点为粗体------*/
            public static RoutedUICommand ToggleNodeTextBold = new RoutedUICommand();

           /*-----给节点添加标签------*/
          public static RoutedUICommand AddLabelToInfoNode = new RoutedUICommand();
        /*-----添加书签------*/
        public static RoutedUICommand AddBookMark = new RoutedUICommand();
        #endregion
        #region 文件工具命令
        /// <summary>
        /// 打开文档
        /// </summary>
        public static RoutedUICommand OpenDocument = new RoutedUICommand();

        /// <summary>
        /// 插入文档
        /// </summary>
        public static RoutedUICommand InsertDocument = new RoutedUICommand();

        /// <summary>
        /// 导出选择的部分到文件
        /// </summary>
        public static RoutedUICommand ExportSelectionToFile = new RoutedUICommand();
        #endregion
        #region 编辑工具命令
        /// <summary>
        /// 显示流文档的XAML代码
        /// </summary>
        public static RoutedCommand ShowDocumentXAML = new RoutedCommand();

        /// <summary>
        /// 根据流文档的XAML代码重新刷新显示RichTextBox
        /// </summary>
        public static RoutedCommand RefreshRichTextBoxFromXAML = new RoutedCommand();
        /// <summary>
        /// 以默认格式粘贴
        /// </summary>
        public static RoutedCommand PasteAndClearAllProperties = new RoutedCommand();
        #endregion
    }

}
