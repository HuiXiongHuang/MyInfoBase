using System.Windows.Input;
namespace WPFSuperRichTextBox
{
    public class EditorToolBarCommands
    {
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
    }
}
