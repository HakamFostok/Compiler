using Compiler.Core;
using Compiler.Interface.ViewModels;
using Prism.Events;

namespace Compiler.Interface;

public class CloseFilePubSub : PubSubEvent<AubFile> { }

public class WriteEventPubSub : PubSubEvent<WriteEventArgs> { }

public class CompileFailEventPubSub : PubSubEvent<ShowErrorEventArgs> { }

public class CompileSuccessEventPubSub : PubSubEvent { }
