using InterfaceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoNode
{
   public class AddLabelEvent:IDomainEvent
    {
        public InfoNodeAccess AccessInfo { get; private set; }
        public InfoNodeDataInfo DataInfo { get; private set; }
     
        public AddLabelEvent(InfoNodeAccess accessInfo, InfoNodeDataInfo dataInfo)
        {
            AccessInfo = accessInfo;
            DataInfo = dataInfo;
        }
    }
}
