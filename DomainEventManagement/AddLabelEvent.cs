using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventManagement
{
   public class AddLabelEvent:IDomainEvent
    {
        //InfoNodeAccess
        //    InfoNodeDataInfo
        public decimal Delta { get; private set; }
        public AddLabelEvent(decimal delta)
        {
            Delta = delta;
        }
    }
}
