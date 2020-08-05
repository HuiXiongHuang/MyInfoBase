using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DataAccessLayer.LabelNodeDA
{
    
   public class DBInfoNodeInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long ID { get; set; }
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
        private string _infoNodeHearder = "";
        public string InfoNodeHearder
        {
            get
            {
                return _infoNodeHearder;
            }
            set
            {
                _infoNodeHearder = value;
                OnPropertyChanged("InfoNodeHearder");
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



        /// <summary>
        /// 将DBInfoNodeInfo对象转换为EF可以直接保存的InfoNode对象
        /// </summary>
        /// <param name="dbInfoNodeInfo"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static InfoNodeDB toInfoNodeDB(DBInfoNodeInfo dbInfoNodeInfo, InfoNodeDB infoNodeObj)
        {
            if (dbInfoNodeInfo != null)
            {
                InfoNodeDB infoNode = new InfoNodeDB()
                {
                    ID = dbInfoNodeInfo.ID,
                    Text = dbInfoNodeInfo.InfoNodeHearder,
                    Path = dbInfoNodeInfo.Path,
                    ModifyTime = dbInfoNodeInfo.ModifyTime,


                };
                return infoNode;

            }
            return null;
        }



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
            if ((obj is DBInfoNodeInfo) == false)
                return false;

            return _path == (obj as DBInfoNodeInfo).Path;
        }

        public override int GetHashCode()
        {
            return _path.GetHashCode();
        }

    }
}
