using Compiler.Core;
using Compiler.Interface.ViewModels;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Interface
{
    public class CloseFilePubSub : PubSubEvent<AubFile> { }

    public class WriteEventPubSub : PubSubEvent<WriteEventArgs> { }

    public class CompileFailEventPubSub : PubSubEvent<ShowErrorEventArgs> { }

    public class CompileSuccessEventPubSub : PubSubEvent { }
}
