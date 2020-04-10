using DataAccessLayer;
using InterfaceLibrary;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoNode
{
   public class FileCountChangedEvent : IDomainEvent
    {
        public bool IsHaveFile{ get; private set; }
        public InfoNodeAccess AccessInfo { get; private set; }
        public InfoNodeDataInfo DataInfo { get; private set; }

        public FileCountChangedEvent(bool ishavefile , InfoNodeAccess accessInfo, InfoNodeDataInfo dataInfo)
        {
            IsHaveFile = ishavefile;
            AccessInfo = accessInfo;
            DataInfo = dataInfo;
        }
    
    }
}
