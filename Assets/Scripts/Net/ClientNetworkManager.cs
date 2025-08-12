using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Net
{
    /// <summary>
    /// Manages client-side networking operations
    /// </summary>
    public class ClientNetworkManager
    {
        /// <summary>
        /// Interface for handling client-side packet reception
        /// </summary>
        public interface IClientReceiver
        {
            void Handle(Packet packet);
        }

        private readonly Dictionary<string, IClientReceiver> _receivers = new Dictionary<string, IClientReceiver>();

        /// <summary>
        /// Registers a packet handler for a specific channel
        /// </summary>
        /// <param name="channel">Channel identifier</param>
        /// <param name="receiver">Handler for packets on this channel</param>
        public void Register(string channel, IClientReceiver receiver)
        {
            _receivers.Add(channel, receiver);
        }

        /// <summary>
        /// Resolves and handles an incoming packet
        /// </summary>
        /// <param name="packet">The packet to resolve</param>
        public void Resolve(Packet packet)
        {
            try
            {
                var channel = packet.Reader.ReadString();
                if (_receivers.TryGetValue(channel, out var receiver))
                {
                    receiver?.Handle(packet);
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
        private Connection _connection;
        private Thread _networkThread;

        /// <summary>
        /// Connects to a server using default port
        /// </summary>
        /// <param name="ip">Server IP address</param>
        /// <param name="username">Player username</param>
        public void Connect(IPAddress ip, string username)
        {
            Connect(ip, NetworkConstants.DefaultPort, username);
        }

        /// <summary>
        /// Connects to a server on specified port
        /// </summary>
        /// <param name="ip">Server IP address</param>
        /// <param name="port">Server port</param>
        /// <param name="username">Player username</param>
        public void Connect(IPAddress ip, int port, string username)
        {
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(new IPEndPoint(ip, port));
                _connection = new Connection(_socket);
                Init(username);
                Debug.Log($"Connected to server at {ip}:{port}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to connect to server: {ex.Message}");
                Disconnect();
            }
        }

        /// <summary>
        /// Disconnects from the server
        /// </summary>
        public void Disconnect()
        {
            try
            {
                _connection?.Dispose();
                _socket?.Close();
                _networkThread?.Join(1000); // Wait up to 1 second for thread to finish
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error during disconnect: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets whether the client is connected to a server
        /// </summary>
        public bool IsConnected => _connection?.IsConnected ?? false;

        private void Init(string username)
        {
            try
            {
                var packet = Packet.Empty();
                packet.Writer.Write(username);
                
                if (!_connection.Send(packet))
                {
                    throw new InvalidOperationException("Failed to send initial packet");
                }

                var serverPacket = _connection.Receive();
                if (serverPacket == null)
                {
                    throw new InvalidOperationException("Failed to receive server response");
                }
                
                var guid = new Guid(serverPacket.Reader.ReadBytes(16));
                
                NetworkManager.Execute(manager =>
                {
                    Player.Create(manager.clientPlayer,
                        NetworkConstants.DefaultSpawnPosition,
                        NetworkConstants.DefaultSpawnRotation, guid, username);
                });
                
                _networkThread = new Thread(NetworkThread);
                _networkThread.Start();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize connection: {ex.Message}");
                Disconnect();
            }
        }

        private void NetworkThread()
        {
            try
            {
                while (_connection.IsConnected)
                {
                    var packet = _connection.Receive();
                    if (packet == null)
                    {
                        break;
                    }
                    Resolve(packet);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in network thread: {ex.Message}");
            }
            finally
            {
                Debug.Log("Disconnected from server");
            }
        }

        /// <summary>
        /// Sends a packet to the server
        /// </summary>
        /// <param name="channel">Channel identifier</param>
        /// <param name="body">Packet body data</param>
        /// <returns>True if sent successfully, false otherwise</returns>
        public bool Send(string channel, Packet body)
        {
            if (!IsConnected)
                return false;
                
            try
            {
                var packet = Packet.Empty();
                packet.Writer.Write(channel);
                packet.Writer.Write(body.GetBytes());
                return _connection.Send(packet);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error sending packet: {ex.Message}");
                return false;
            }
        }
    }
}