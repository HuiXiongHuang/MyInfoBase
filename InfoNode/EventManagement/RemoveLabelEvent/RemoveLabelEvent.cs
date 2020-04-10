using DataAccessLayer;
using InterfaceLibrary;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoNode
{
   public class RemoveLabelEvent : IDomainEvent
    {
        public List< LabelNodeDB > LabelNodeDBs{ get; private set; }
       
        public RemoveLabelEvent(List<LabelNodeDB> labelNodeDBs)
        {
            LabelNodeDBs = labelNodeDBs;
        }
    
    }
}
