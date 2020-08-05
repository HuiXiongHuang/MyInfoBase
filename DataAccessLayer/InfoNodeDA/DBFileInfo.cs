using Model;
using PublicLibrary.Utils;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace DataAccessLayer.InfoNodeDA
{
    /// <summary>
    /// 代表保存于数据库中的文件信息
    /// 之所以设计它是为了避免将文件内容全部加载到内存所带来的内存浪费
    /// 此对象支持WPF数据绑定
    /// 此对象重写了equals方法和getHashCode()方法，因此，支持判等操作，判断依据是FilePath属性
    /// 即FilePath属性相等的两个DBFileInfo对象认为是相等的。
    /// </summary>
    public class DBFileInfo : INotifyPropertyChanged
    {
        public long ID { get; set; }
        public long FileContentID { get; set; }
        private String _filePath = "";
        public String FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
                OnPropertyChanged("FilePath");
            }
        }
        private byte[] _hashByte;
        public byte[] FileHash
        {
            get
            {
                return _hashByte;
            }
            set
            {
                _hashByte = value;
                OnPropertyChanged("HashByte");
            }
        }
        private bool _isEncrypted = false;
        public bool IsEncrypted
        {
            get
            {
                return _isEncrypted;
            }
            set
            {
                _isEncrypted = value;
                if (_isEncrypted==false)
                {
                    EncryptedStatus = DecryptedIcon;
                }
                else
                {
                    EncryptedStatus = EncryptedIcon;
                }

            }
        }
        private ImageSource _encryptedStatus = null;
        private static ImageSource DecryptedIcon =  ImageUtils.GetBitmapSourceFromImageFileName("pack://application:,,,/InfoNode;component/Images/jiesuo.png", UriKind.Absolute);
        private static ImageSource EncryptedIcon = ImageUtils.GetBitmapSourceFromImageFileName("pack://application:,,,/InfoNode;component/Images/suo.png", UriKind.Absolute);

        /// <summary>
        /// 正常状态下的图标
        /// </summary>
        public ImageSource EncryptedStatus
        {
            get
            {
                if (_encryptedStatus == null)
                {
                    _encryptedStatus = DecryptedIcon;
                }
                return _encryptedStatus;
            }
            set
            {
                _encryptedStatus = value;

                OnPropertyChanged("EncryptedStatus");
            }

        }

        private long _fileSize = 0;
        public long FileSize
        {
            get
            {
                return _fileSize;
            }
            set
            {
                _fileSize = value;
                _fileSizeToUI = FileUtils.FileSizeFormater(_fileSize);
                OnPropertyChanged("FileSize");
            }
        }
        private string _fileSizeToUI = "";
        public string FileSizeToUI
        {
            get
            {
                return _fileSizeToUI;
            }
            set
            {
                _fileSizeToUI = value;
                OnPropertyChanged("FileSizeToUI");
            }
        }
        private DateTime _time;
        public DateTime AddTime
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                OnPropertyChanged("AddTime");
            }
        }
        /// <summary>
        /// 将DBFileInfo对象转换为EF可以直接保存的DiskFileInfo对象,不包括文件内容
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        public static DiskFileInfo toDiskFileInfo(DBFileInfo fileInfo)
        {
            if (fileInfo != null)
            {
                DiskFileInfo file = new DiskFileInfo()
                {
                    ID = fileInfo.ID,
                    FileSize = fileInfo.FileSize,
                    FilePath = fileInfo.FilePath,
                    AddTime = fileInfo.AddTime,
                    FileHash = fileInfo.FileHash,
                    FileContentID = fileInfo.FileContentID,
                    IsEncrypted=fileInfo.IsEncrypted

                };


                return file;

            }
            return null;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public override bool Equals(object obj)
        {
            if ((obj is DBFileInfo) == false)
                return false;

            return _filePath == (obj as DBFileInfo).FilePath;
        }

        public override int GetHashCode()
        {
            return _filePath.GetHashCode();
        }
    }
}
