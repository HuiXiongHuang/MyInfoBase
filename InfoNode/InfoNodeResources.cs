using PublicLibrary.Utils;
using System;
using System.Windows.Media;


namespace InfoNode
{
   public class InfoNodeResources
    {
        private static ImageSource _normal = null;
        /// <summary>
        /// 正常状态下的图标
        /// </summary>
        public static ImageSource NormalIcon
        {
            get
            {
                if (_normal == null)
                {
                    _normal = ImageUtils.GetBitmapSourceFromImageFileName("pack://application:,,,/InfoNode;component/Images/InfoNode.png", UriKind.Absolute);

                }
                return _normal;
            }
            set
            {
                _normal = value;
            }

        }
        private static ImageSource _noInfoIcon = null;
        /// <summary>
        /// 无信息状态下的图标
        /// </summary>
        public static ImageSource NoInfoIcon
        {
            get
            {
                if (_noInfoIcon == null)
                {
                    _noInfoIcon = ImageUtils.GetBitmapSourceFromImageFileName("pack://application:,,,/InfoNode;component/Images/NoInfoIcon.png", UriKind.Absolute);
                }
                return _noInfoIcon;
            }
            set
            {
                _noInfoIcon = value;
            }

        }
        private static ImageSource _imageIcon = null;
        /// <summary>
        /// 富文本中存在图片时的图标
        /// </summary>
        public static ImageSource ImageIcon
        {
            get
            {
                if (_imageIcon == null)
                {
                    _imageIcon = ImageUtils.GetBitmapSourceFromImageFileName("pack://application:,,,/InfoNode;component/Images/ImageIcon.png", UriKind.Absolute);
                }
                return _imageIcon;
            }
            set
            {
                _imageIcon = value;
            }

        }
        private static ImageSource _fileIcon = null;
        /// <summary>
        /// File图标
        /// </summary>
        public static ImageSource FileIcon
        {
            get
            {
                if (_fileIcon == null)
                {
                    _fileIcon = ImageUtils.GetBitmapSourceFromImageFileName("pack://application:,,,/InfoNode;component/Images/FileIcon.png", UriKind.Absolute);

                }
                return _fileIcon;
            }
            set
            {
                _fileIcon = value;
            }

        }
        private static ImageSource _selected = null;
        /// <summary>
        /// 选中状态的图标
        /// </summary>
        public static ImageSource SelectedIcon
        {
            get
            {
                if (_selected == null)
                {
                    _selected = ImageUtils.GetBitmapSourceFromImageFileName("pack://application:,,,/InfoNode;component/Images/InfoNode.png", UriKind.Absolute);

                }
                return _selected;
            }
            set
            {
                _selected = value;
            }

        }
        /// <summary>
        /// 实现复用的UI组件
        /// </summary>
        public static InfoNodeControl RootControl = new InfoNodeControl();
    }
}
