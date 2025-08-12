using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;

namespace Net
{
    /// <summary>
    /// Manages server-side networking operations
    /// </summary>
    public class ServerNetworkManager
    {
        /// <summary>
        /// Interface for handling server-side packet reception
        /// </summary>
        public interface IServerReceiver
        {
            void Handle(ServerPlayer player, Packet packet);
        }

        private readonly Dictionary<string, IServerReceiver> _receivers = new Dictionary<string, IServerReceiver>();

        /// <summary>
        /// Registers a packet handler for a specific channel
        /// </summary>
        /// <param name="channel">Channel identifier</param>
        /// <param name="receiver">Handler for packets on this channel</param>
        public void Register(string channel, IServerReceiver receiver)
        {
            _receivers.Add(channel, receiver);
        }

        private void Resolve(ServerPlayer player, Packet packet)
        {
            try
            {
                var channel = packet.Reader.ReadString();
                if (_receivers.TryGetValue(channel, out var receiver))
                {
                    receiver?.Handle(player, packet);
                }
                else
                {
                    Debug.LogWarning($"No receiver registered for channel: {channel}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error resolving packet: {ex.Message}");
            }
        }

        private Socket _socket;
        private bool _isRunning;

        /// <summary>
        /// Starts listening for client connections on the default port
        /// </summary>
        public void Listen()
        {
            Listen(NetworkConstants.DefaultPort);
        }

        /// <summary>
        /// Starts listening for client connections on the specified port
        /// </summary>
        /// <param name="port">Port to listen on</param>
        public void Listen(int port)
        {
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(new IPEndPoint(IPAddress.Any, port));
                _socket.Listen(NetworkConstants.MaxConnections);
                _isRunning = true;
                new Thread(AcceptThread).Start();
                Debug.Log($"Server listening on port {port}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to start server: {ex.Message}");
            }
        }

        /// <summary>
        /// Stops the server and closes all connections
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
            try
            {
                _socket?.Close();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error stopping server: {ex.Message}");
            }
        }

        private void AcceptThread()
        {
            while (_isRunning)
            {
                try
                {
                    var clientSocket = _socket.Accept();
                    var client = new Connection(clientSocket);
                    Connect(client);
                }
                catch (ObjectDisposedException)
                {
                    // Server socket was closed, exit gracefully
                    break;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error accepting client connection: {ex.Message}");
                }
            }
        }
        
        private void Connect(Connection client)
        {
            try
            {
                var clientInit = client.Receive();
                if (clientInit == null)
                {
                    client.Dispose();
                    return;
                }
                
                var username = clientInit.Reader.ReadString();
                
                // Validate and sanitize username
                if (!NetworkValidator.IsValidUsername(username))
                {
                    Debug.LogWarning($"Invalid username received: '{username}', closing connection");
                    client.Dispose();
                    return;
                }
                
                username = NetworkValidator.SanitizeUsername(username);

                var guid = Guid.NewGuid();
                var serverInit = Packet.Empty();
                serverInit.Writer.Write(guid.ToByteArray());
                
                if (!client.Send(serverInit))
                {
                    client.Dispose();
                    return;
                }
                
                NetworkManager.Execute(manager =>
                {
                    var player = Player.Create(manager.serverPlayer,
                        NetworkConstants.DefaultSpawnPosition,
                        NetworkConstants.DefaultSpawnRotation, guid, username);
                    var serverPlayer = player.GetComponent<ServerPlayer>();
                    serverPlayer.Connection = client;
                    
                    Debug.Log($"Player '{username}' connected with ID: {guid}");
                    
                    new Thread(() => ClientThread(serverPlayer)).Start();
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error during client connection: {ex.Message}");
                client.Dispose();
            }
        }

        private void ClientThread(ServerPlayer player)
        {
            try
            {
                while (player.Connection.IsConnected)
                {
                    var packet = player.Connection.Receive();
                    if (packet == null || packet.Length == 0)
                    {
                        break;
                    }
                    Resolve(player, packet);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in client thread: {ex.Message}");
            }
            finally
            {
                NetworkManager.Execute((e) =>
                {
                    var players = UnityEngine.Object.FindObjectsByType<ServerPlayer>(FindObjectsSortMode.None);
                    var packet = ClientShadowPlayerDisconnect.Build(player);
                    foreach (var pl in players)
                    {
                        if (pl == player)
                        {
                            continue;
                        }
                        pl.Send(NetworkConstants.Channels.ShadowPlayerDisconnect, packet);
                    }
                    UnityEngine.Object.Destroy(player.gameObject);
                });
                
                player.Connection?.Dispose();
            }
        }
    }
}