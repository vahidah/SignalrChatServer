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

        Dictionary<int, string> idBook = new Dictionary<int, string>();

        public override Task OnConnectedAsync()
        {

            idBook.Add(current_id, Context.ConnectionId);
            Clients.Caller.SendAsync("ReciveId", current_id);
            current_id++;
            Console.WriteLine("new client connectd");
            return base.OnConnectedAsync();
        }
        public async Task sendMess(int id, string message)
        {
            Console.WriteLine("sneding message");

            await Clients.Clients(idBook[id]).SendAsync("ReceiveNewMessage", message);

        }   
    }
}
