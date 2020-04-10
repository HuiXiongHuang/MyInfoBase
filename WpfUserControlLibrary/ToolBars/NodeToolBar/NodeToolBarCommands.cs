﻿using System.Windows.Input;

namespace WpfUserControlLibrary.ToolBars.NodeToolBar
{
    class NodeToolBarCommands
    {
        /// <summary>
        /// 自定义一些命令
        /// </summary>
      
            /*-------添加子节点------------*/
           
            public static RoutedUICommand AddChildNode= new RoutedUICommand();

            /*-------添加兄弟节点------------*/
         
            public static RoutedUICommand AddSiblingNode = new RoutedUICommand();

            /*-------添加根节点------------*/
          
            public static RoutedUICommand AddRootNode = new RoutedUICommand();

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
            public static RoutedUICommand ToFolder = new RoutedUICommand();

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

        
    }
}
