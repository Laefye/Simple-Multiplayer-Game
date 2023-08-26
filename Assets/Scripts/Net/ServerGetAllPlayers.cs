using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Net
{
    public class ServerGetAllPlayers : ServerNetworkManager.IServerReceiver
    {
        public static string Channel = "get_all_players";
        
        public void Handle(ServerPlayer player, Packet packet)
        {
            NetworkManager.Execute((manager) =>
            {
                var objects = Object.FindObjectsByType<ServerPlayer>(FindObjectsSortMode.None);
                var packets = objects.Select(pl => ClientShadowPlayer.Build(pl)).ToList();
                new Thread(() =>
                {
                    foreach (var pack in packets)   
                    {
                        player.Send(ClientShadowPlayer.Channel, pack);
                    }
                }).Start();
            });
        }
    }
}