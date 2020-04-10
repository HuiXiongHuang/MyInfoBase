using InterfaceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabelNode
{
   public class LoadInfoNodeControlEvent : IDomainEvent
    {
        //public LabelNodeAccess AccessInfo { get; private set; }
        //public LabelNodeDataInfo DataInfo { get; private set; }
     public string Path { get; private set; }
        public LoadInfoNodeControlEvent(string path)
        {
            Path = path;
        }
    }
}
