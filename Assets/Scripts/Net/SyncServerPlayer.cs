using System;
using UnityEngine;

namespace Net
{
    /// <summary>
    /// Handles server-side reception of player position/rotation updates from clients
    /// </summary>
    public class SyncServerPlayer : ServerNetworkManager.IServerReceiver
    {
        public static string Channel = NetworkConstants.Channels.SyncServerPlayer;
        
        /// <summary>
        /// Handles incoming player sync packets and broadcasts updates to other players
        /// </summary>
        /// <param name="player">The server player that sent the update</param>
        /// <param name="packet">The packet containing position/rotation data</param>
        public void Handle(ServerPlayer player, Packet packet)
        {
            try
            {
                var position = packet.ReadVector3();
                var rotation = packet.ReadQuaternion();
                
                // Validate the received data
                if (!NetworkValidator.IsValidPosition(position))
                {
                    Debug.LogWarning($"Invalid position received from player {player.GetComponent<Player>()?.username}: {position}");
                    return;
                }
                
                if (!NetworkValidator.IsValidRotation(rotation))
                {
                    Debug.LogWarning($"Invalid rotation received from player {player.GetComponent<Player>()?.username}: {rotation}");
                    return;
                }
                
                NetworkManager.Execute((e) =>
                {
                    // Update player position and rotation
                    player.transform.position = position;
                    player.transform.rotation = rotation;
                    
                    // Build update packet for other clients
                    var updatePacket = ClientShadowPlayer.Build(player);
                    var players = UnityEngine.Object.FindObjectsByType<ServerPlayer>(FindObjectsSortMode.None);
                    
                    // Broadcast to all other connected players
                    foreach (var pl in players)
                    {
                        if (pl != player && pl.Connection.IsConnected)
                        {
                            pl.Send(NetworkConstants.Channels.ShadowPlayerSync, updatePacket);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error handling player sync: {ex.Message}");
            }
        }

        /// <summary>
        /// Builds a player sync packet from a client player
        /// </summary>
        /// <param name="player">The client player to create packet from</param>
        /// <returns>The packet containing position/rotation data</returns>
        public static Packet Build(ClientPlayer player)
        {
            var packet = Packet.Empty();
            packet.Write(player.transform.position);
            packet.Write(player.transform.rotation);
            return packet;
        }
    }
}