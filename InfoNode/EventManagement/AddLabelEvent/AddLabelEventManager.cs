using InterfaceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoNode
{
    public class AddLabelEventManager
    {
        //保存所有的事件响应者引用
        private static List<IHandler<AddLabelEvent>> Handlers = new List<IHandler<AddLabelEvent>>();
        //供外界订阅此事件
        public void Register(IHandler<AddLabelEvent> handler)
        {
            Handlers.Add(handler);
        }
        //触发事件
        public void RaiseEvent(InfoNodeAccess accessInfo, InfoNodeDataInfo dataInfo)
        {
            //创建事件对象
            var eventObj = new AddLabelEvent(accessInfo, dataInfo);
            //依次通知所有订阅了本事件的响应者
            foreach (var handler in Handlers)
            {
                handler.Handle(eventObj);
            }
        }
    }
}
