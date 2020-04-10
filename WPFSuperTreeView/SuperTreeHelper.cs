using System;

namespace WPFSuperTreeView
{
    public class SuperTreeHelper
    {
        /// <summary>
        /// 依据节点类型，生成节点默认的文本（不包括后面的数字）
        /// </summary>
        /// <param name="NodeType"></param>
        /// <returns></returns>
        public static String getDefaultNodeText(String NodeType)
        {
            String defaultNodeText = "";
            switch (NodeType)
            {

                case "LabelNodeType":
                    defaultNodeText = "新纯文本";
                    break;
                case "InfoNodeType":
                    defaultNodeText = "新信息节点";
                    break;
            }
            return defaultNodeText;
        }
    }
}
