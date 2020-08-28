using NydusNetwork;
using NydusNetwork.API.Protocol;
using System;

namespace AdequateSource
{
    class Example
    {
        void Ping(GameClient client) {
            if (client.TryWaitPingRequest(out var r))
                Console.WriteLine(r.Ping.GameVersion);
        }

        void ListenForPing(GameClient client)
        {
            var action = new Action<Response>(r => {
                Console.WriteLine($"Heard ping! {r.Ping.GameVersion}");
            });
            client.RegisterHandler(Response.ResponseOneofCase.Ping, action);
        }
    }
}
