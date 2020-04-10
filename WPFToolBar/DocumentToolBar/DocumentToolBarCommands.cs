using System.Windows.Input;


namespace WPFToolBar.DocumentToolBar
{
    class DocumentToolBarCommands
    {
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

    }
}
