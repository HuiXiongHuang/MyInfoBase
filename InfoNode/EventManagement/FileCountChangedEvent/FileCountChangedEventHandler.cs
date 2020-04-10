using InterfaceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace InfoNode
{
    public  class FileCountChangedEventHandler : IHandler<FileCountChangedEvent>
    {
     
        public void Handle(FileCountChangedEvent _event)
        {

            //MessageBox.Show("you");
        }


    }
}
