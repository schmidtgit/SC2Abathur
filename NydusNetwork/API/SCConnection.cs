using Google.Protobuf;
using NydusNetwork.API.Protocol;
using NydusNetwork.Logging;
using NydusNetwork.Services;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using static NydusNetwork.API.Protocol.Response;

namespace NydusNetwork.API
{
    internal class SCConnection {
        public Status Status { get; private set; }
        private const int READ_BUFFER = 1024;
        private const int MAX_CONNECTION_ATTEMPTS = 25;
        private const int TIMEOUT = 250; //ms

        private ILogger _log;
        private CaseHandler<ResponseOneofCase, Response> _handler;
        private ClientWebSocket _socket;
        public SCConnection(ILogger log) {
            _log = log;
            _handler = new CaseHandler<ResponseOneofCase, Response>();
        }

        public void RegisterHandler(ResponseOneofCase action, Action<Response> handler)
            => _handler.RegisterHandler(action, handler);
        public void DeregisterHandler(Action<Response> handler)
            => _handler.DeregisterHandler(handler);

        public bool Connect(Uri uri, int maxAttempts = MAX_CONNECTION_ATTEMPTS) {
            var failCount = 0;
            do {
                try {
                    _socket = new ClientWebSocket();
                    _socket.ConnectAsync(uri,CancellationToken.None).Wait();
                } catch(AggregateException){Task.Delay(TIMEOUT).Wait(); failCount++;} catch(Exception) { break; }
            } while(_socket.State != WebSocketState.Open && failCount < maxAttempts);
            if(_socket.State == WebSocketState.Open) {
#if DEBUG
                _log?.LogSuccess($"NydusNetwork: Succesfully connected to {uri.AbsoluteUri}");
#endif
                Task.Run(() => Receive());
                return true;
            } 

#if DEBUG
            if(maxAttempts != 1)
                _log?.LogError($"NydusNetwork: Unable to connect to {uri.AbsoluteUri} ({failCount} attempts were made)");
#endif
            return false;
        }

        public void AsyncSend(Request req)
            => _socket.SendAsync(new ArraySegment<byte>(req.ToByteArray()),WebSocketMessageType.Binary,true,CancellationToken.None);


        public bool TryWaitResponse(Request req, out Response response, int wait = TIMEOUT) {
            Response result = null;
            if (_socket.State == WebSocketState.Open) { 
                var marker = new Task(() => { });
                var action = new Action<Response>(r => { result = r; marker.RunSynchronously(); });
                _handler.RegisterHandler((Response.ResponseOneofCase)req.RequestCase,action);
                AsyncSend(req);
                marker.Wait(wait);
                _handler.DeregisterHandler(action);
            }
            response = result;
            return result != null;
        }

        private async Task Receive() {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[READ_BUFFER]);
            while(_socket.State == WebSocketState.Open) {
                WebSocketReceiveResult result = null;
                using(var ms = new MemoryStream()) {
                    do {
                        result = await _socket.ReceiveAsync(buffer,CancellationToken.None);
                        ms.Write(buffer.Array,buffer.Offset,result.Count);
                    } while(!result.EndOfMessage);
                    var msg = Response.Parser.ParseFrom(ms.GetBuffer(),0,(int)ms.Position);
                    Status = msg.Status;
                    _handler.Handle(msg.ResponseCase,msg);
                }
            }
        }

        public void Disconnect() {
            _socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None).Wait();
            _socket.Dispose();
        }
    }
}
