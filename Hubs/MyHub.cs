using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;


namespace SignalRBackEnd.Hubs
{
    public class MyHub : Hub
    {

        private static int current_id = 0;
        private static string current_firebase_token;

        private static Dictionary<int, string> id_key = new Dictionary<int, string>();
        private static Dictionary<string, int> connection_id_key = new Dictionary<string, int>();

        private static List<ClientInfo> clientInfos = new List<ClientInfo>();
        private static Dictionary<string, List<int>> groupInfos = new Dictionary<string, List<int>>();

        public override Task OnConnectedAsync()
        {

            id_key.Add(current_id, Context.ConnectionId);
            connection_id_key.Add(Context.ConnectionId, current_id);
            Clients.Caller.SendAsync("ReceiveId", current_id);
            clientInfos.Add(new ClientInfo(Context.ConnectionId, current_firebase_token));
            Console.WriteLine("client created");
            current_id++;
            Console.WriteLine("new client connectd");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            foreach(var entry in clientInfos)
            {
                if(entry.connection_id == Context.ConnectionId)
                {
                    entry.online = false;
                }
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task sendMessage(int id, string message, bool first_message)
        {
            Console.WriteLine("sending message");

            int callerId = connection_id_key[Context.ConnectionId];

            if (first_message)
            {
                await Clients.Clients(id_key[callerId]).SendAsync("receiveUserName", id, clientInfos[id].user_name);
            }

                if (clientInfos[id].online)
                {
                    await Clients.Clients(id_key[id]).SendAsync("ReceiveNewMessage", callerId, message, clientInfos[callerId].user_name);
                }
                else
                {


                    var notif_message = new Message()
                    {
                        Notification = new Notification()
                        {
                            Title = $"new message from user {connection_id_key[Context.ConnectionId]}",
                            Body = message

                        },

                        Token = clientInfos[id].firebase_token
                    };

                    Console.WriteLine($"fire_base token is: ${clientInfos[id].firebase_token}");

                    // Response is a message ID string.
                    Console.WriteLine("Successfully sent message: " + await FirebaseMessaging.DefaultInstance.SendAsync(notif_message));

                }

            }

        public async Task SendMessageToGroup(string groupName , int id, string message)
        {
            Console.WriteLine("we are here");
            foreach(var entry in groupInfos[groupName])
            {

                if (!clientInfos[entry].online)
                {
                    var notif_message = new Message()
                    {
                        Notification = new Notification()
                        {
                            Title = $"new message for group {groupName}",
                            Body = message

                        },

                        Token = clientInfos[entry].firebase_token
                    };
                    Console.WriteLine("this client is offline");
                    Console.WriteLine("Successfully sent message: " + await FirebaseMessaging.DefaultInstance.SendAsync(notif_message));
                }

            }

            await Clients.Group(groupName).SendAsync("GroupMessage", groupName , id, message, clientInfos[id].user_name);
            
            Console.WriteLine($"message send to group: {message}");
            //await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }


        public async Task AddToGroup(string groupName)
        {

            Console.WriteLine($"join group: {groupName}");
            if (groupInfos.ContainsKey(groupName)) {

                groupInfos[groupName].Add(connection_id_key[Context.ConnectionId]);
            }
            else
            {
                groupInfos.Add(groupName, new List<int>(connection_id_key[Context.ConnectionId]));
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);


            foreach(var entry in groupInfos)
            {
                Console.WriteLine($"group name is: {entry.Key}");
            }

            
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }
        public Task ReceiveFireBaseToken(string fire_base_token)
        {
            clientInfos[current_id - 1].firebase_token = fire_base_token;
            Console.WriteLine("fire base token received");
            return Task.CompletedTask;
        }
        public Task ReceiveUserName(string userName, int id)
        {
            clientInfos[id].user_name = userName;
            Console.WriteLine($"userName received {userName}");
            return Task.CompletedTask;
        }
    }


    public class ClientInfo
    {
        public string connection_id;
        public string firebase_token;
        public bool online;
        public string user_name;
        public ClientInfo(string con_id, string fire_token)
        {
            connection_id = con_id;
            firebase_token = fire_token;
            online = true;
        }

    }
}



