using System;
using System.Collections.Generic;
using System.Text;

namespace ProudNet.Abstraction
{
    internal interface INetworkService
    {
        void RaiseError(ErrorEventArgs e);
    }
}
