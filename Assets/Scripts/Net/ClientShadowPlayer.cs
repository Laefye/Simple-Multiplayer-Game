using System;
using System.Collections.Generic;
using UnityEngine;

namespace Net
{
    /// <summary>
    /// Handles client-side reception of shadow player updates from the server
    /// </summary>
    public class ClientShadowPlayer : ClientNetworkManager.IClientReceiver
    {
        public static string Channel = NetworkConstants.Channels.ShadowPlayerSync;
        
        // Cache for shadow players to optimize lookups
        private static readonly Dictionary<Guid, ShadowPlayer> _shadowPlayerCache = new Dictionary<Guid, ShadowPlayer>();
        
        /// <summary>
        /// Handles incoming shadow player update packets
        /// </summary>
        /// <param name="packet">The packet containing player update data</param>
        public void Handle(Packet packet)
        {
            try
            {
                var id = new Guid(packet.Reader.ReadBytes(16));
                var username = packet.Reader.ReadString();
                var position = packet.ReadVector3();
                var rotation = packet.ReadQuaternion();
                
                NetworkManager.Execute((manager) =>
                {
                    var client = UnityEngine.Object.FindAnyObjectByType<ClientPlayer>()?.GetComponent<Player>();
                    if (client?.ID == id)
                    {
                        return; // Don't update our own player
                    }
                    
                    // Try to get from cache first
                    if (_shadowPlayerCache.TryGetValue(id, out var shadowPlayer) && shadowPlayer != null)
                    {
                        // Update existing shadow player
                        shadowPlayer.Position = position;
                        shadowPlayer.Rotation = rotation;
                    }
                    else
                    {
                        // Search for shadow player by ID (fallback for cache misses)
                        var players = UnityEngine.Object.FindObjectsByType<ShadowPlayer>(FindObjectsSortMode.None);
                        ShadowPlayer foundPlayer = null;
                        
                        foreach (var pl in players)
                        {
                            var playerComponent = pl.GetComponent<Player>();
                            if (playerComponent?.ID == id)
                            {
                                foundPlayer = pl;
                                break;
                            }
                        }
                        
                        if (foundPlayer != null)
                        {
                            // Update cache and position
                            _shadowPlayerCache[id] = foundPlayer;
                            foundPlayer.Position = position;
                            foundPlayer.Rotation = rotation;
                        }
                        else
                        {
                            // Create new shadow player
                            var newPlayer = Player.Create(manager.shadowPlayer, position, rotation, id, username);
                            var shadowComponent = newPlayer.GetComponent<ShadowPlayer>();
                            if (shadowComponent != null)
                            {
                                _shadowPlayerCache[id] = shadowComponent;
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error handling shadow player update: {ex.Message}");
            }
        }

        /// <summary>
        /// Builds a shadow player update packet from a server player
        /// </summary>
        /// <param name="player">The server player to create packet from</param>
        /// <returns>The packet containing player data</returns>
        public static Packet Build(ServerPlayer player)
        {
            var packet = Packet.Empty();
            var playerComponent = player.GetComponent<Player>();
            packet.Writer.Write(playerComponent.ID.ToByteArray());
            packet.Writer.Write(playerComponent.username);
            packet.Write(player.transform.position);
            packet.Write(player.transform.rotation);
            return packet;
        }
        
        /// <summary>
        /// Removes a player from the cache (called when player disconnects)
        /// </summary>
        /// <param name="playerId">ID of the player to remove</param>
        public static void RemoveFromCache(Guid playerId)
        {
            _shadowPlayerCache.Remove(playerId);
        }
    }
}