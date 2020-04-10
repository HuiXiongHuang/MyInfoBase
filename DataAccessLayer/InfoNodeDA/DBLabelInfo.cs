using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DataAccessLayer.InfoNodeDA
{
    public class DBLabelInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
     
            public int ID { get; set; }

        private String _path = "";
        public String Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                OnPropertyChanged("Path");
            }
        }
           private string _label="";
            public string Label
            {
                get
                {
                    return _label;
                }
                set
                {
                _label = value;
                    OnPropertyChanged("Label");
                }
            }
            private DateTime _time;
            public DateTime ModifyTime
            {
                get
                {
                    return _time;
                }
                set
                {
                    _time = value;
                    OnPropertyChanged("ModifyTime");
                }
            }
       
    

    ///// <summary>
    ///// 将DBFileInfo对象转换为EF可以直接保存的LabelNodeDB对象
    ///// </summary>
    ///// <param name="labelInfo"></param>
    ///// <param name="label"></param>
    ///// <returns></returns>
    //public static LabelNodeDB toLabelNodeDB(DBLabelInfo labelInfo, string label)
    //        {
    //            if (labelInfo != null)
    //            {
    //               LabelNodeDB labelNode = new LabelNodeDB()
    //                {
    //                    ID = labelInfo.ID,
    //                    Label = labelInfo.Label,
                        
    //                    ModifyTime = labelInfo.ModifyTime,
                        
    //                };
    //                return labelNode;

    //            }
    //            return null;
    //        }

            

            // Create the OnPropertyChanged method to raise the event
            protected void OnPropertyChanged(string name)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
        }

        // if (infoNodeDataObj.AttachLabels.IndexOf(labelInfo) == -1)如果要对DBLabelInfo类型的实例进行判等，必须实现下面两个函数
        public override bool Equals(object obj)
            {
                if ((obj is DBLabelInfo) == false)
                    return false;

                return _path == (obj as DBLabelInfo).Path;
            }

            public override int GetHashCode()
            {
                return _path.GetHashCode();
            }
        
    }
}
