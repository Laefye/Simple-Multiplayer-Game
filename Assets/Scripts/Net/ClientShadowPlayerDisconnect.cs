using System;
using UnityEngine;

namespace Net
{
    /// <summary>
    /// Handles client-side reception of shadow player disconnect notifications
    /// </summary>
    public class ClientShadowPlayerDisconnect : ClientNetworkManager.IClientReceiver
    {
        public static string Channel = NetworkConstants.Channels.ShadowPlayerDisconnect;
        
        /// <summary>
        /// Handles incoming shadow player disconnect packets
        /// </summary>
        /// <param name="packet">The packet containing disconnect data</param>
        public void Handle(Packet packet)
        {
            try
            {
                var id = new Guid(packet.Reader.ReadBytes(16));
                
                NetworkManager.Execute((manager) =>
                {
                    // Remove from cache first
                    ClientShadowPlayer.RemoveFromCache(id);
                    
                    // Find and destroy the shadow player object
                    var players = UnityEngine.Object.FindObjectsByType<ShadowPlayer>(FindObjectsSortMode.None);
                    foreach (var player in players)
                    {
                        var playerComponent = player.GetComponent<Player>();
                        if (playerComponent?.ID == id)
                        {
                            UnityEngine.Object.Destroy(player.gameObject);
                            break;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error handling shadow player disconnect: {ex.Message}");
            }
        }

        /// <summary>
        /// Builds a shadow player disconnect packet from a server player
        /// </summary>
        /// <param name="player">The server player that disconnected</param>
        /// <returns>The packet containing disconnect data</returns>
        public static Packet Build(ServerPlayer player)
        {
            var packet = Packet.Empty();
            var playerComponent = player.GetComponent<Player>();
            packet.Writer.Write(playerComponent.ID.ToByteArray());
            return packet;
        }   
    }
}