using InterfaceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabelNode
{
    public class LoadInfoNodeControlEventManager
    {
        //保存所有的事件响应者引用
        private static List<IHandler<LoadInfoNodeControlEvent>> Handlers = new List<IHandler<LoadInfoNodeControlEvent>>();
        //供外界订阅此事件
        public void Register(IHandler<LoadInfoNodeControlEvent> handler)
        {
            Handlers.Add(handler);
        }
        //触发事件
        public void RaiseEvent(string path)
        {
            //创建事件对象
            var eventObj = new LoadInfoNodeControlEvent(path);
            //依次通知所有订阅了本事件的响应者
            foreach (var handler in Handlers)
            {
                handler.Handle(eventObj);
            }
        }
    }
}
