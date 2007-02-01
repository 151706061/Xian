using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Server.ShredHost
{
    [ServiceContract]
    public interface IShredHost
    {
        [OperationContract]
        WcfDataShred[] GetShreds();

        [OperationContract]
        bool StartShred(WcfDataShred shred);

        [OperationContract]
        bool StopShred(WcfDataShred shred);
    }
}
