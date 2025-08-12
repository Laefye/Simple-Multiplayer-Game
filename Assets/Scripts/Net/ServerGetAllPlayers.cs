using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Net
{
    /// <summary>
    /// Handles server-side requests for all current player data
    /// </summary>
    public class ServerGetAllPlayers : ServerNetworkManager.IServerReceiver
    {
        public static string Channel = NetworkConstants.Channels.GetAllPlayers;
        
        /// <summary>
        /// Handles client requests for all current players and sends their data
        /// </summary>
        /// <param name="player">The requesting player</param>
        /// <param name="packet">The request packet (empty)</param>
        public void Handle(ServerPlayer player, Packet packet)
        {
            try
            {
                NetworkManager.Execute((manager) =>
                {
                    var serverPlayers = UnityEngine.Object.FindObjectsByType<ServerPlayer>(FindObjectsSortMode.None);
                    var packets = new List<Packet>();
                    
                    // Build packets for all players except the requesting one
                    foreach (var serverPlayer in serverPlayers)
                    {
                        if (serverPlayer != player && serverPlayer.Connection.IsConnected)
                        {
                            packets.Add(ClientShadowPlayer.Build(serverPlayer));
                        }
                    }
                    
                    // Send packets in a background thread to avoid blocking
                    new Thread(() =>
                    {
                        try
                        {
                            foreach (var pack in packets)   
                            {
                                if (!player.Connection.IsConnected)
                                    break;
                                    
                                player.Send(NetworkConstants.Channels.ShadowPlayerSync, pack);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error sending player data: {ex.Message}");
                        }
                    }).Start();
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error handling get all players request: {ex.Message}");
            }
        }
    }
}