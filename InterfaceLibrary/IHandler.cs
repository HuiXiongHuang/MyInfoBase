using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfaceLibrary
{
   public interface IHandler<T> where T : IDomainEvent
    {
        void Handle(T domainEvent);
    }
}
