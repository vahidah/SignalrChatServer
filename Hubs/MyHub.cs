using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRBackEnd.Hubs
{
    public class MyHub: Hub
    {

        private static int current_id = 0;

        private static Dictionary<int, string> id_key = new Dictionary<int, string>();
        private static Dictionary<string, int> connection_id_key = new Dictionary<string, int>();

        public override Task OnConnectedAsync()
        {

            id_key.Add(current_id, Context.ConnectionId);
            connection_id_key.Add(Context.ConnectionId, current_id);
            Clients.Caller.SendAsync("ReceiveId", current_id);
            current_id++;
            Console.WriteLine("new client connectd");
            return base.OnConnectedAsync();
        }
        public async Task sendMessage(int id, string message)
        {
            Console.WriteLine("sneding message");

            foreach (var entry in id_key)
            {
                Console.WriteLine($"include: ${entry.Key}");
            }

            Console.WriteLine("after printing keys");

            await Clients.Clients(id_key[id]).SendAsync("ReceiveNewMessage", connection_id_key[Context.ConnectionId], message);

        }   
    }
}
