using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Net
{
    public class ServerNetworkManager
    {
        public interface IServerReceiver
        {
            void Handle(ServerPlayer player, Packet packet);
        }

        private readonly Dictionary<string, IServerReceiver> _receivers = new Dictionary<string, IServerReceiver>();

        public void Register(string channel, IServerReceiver receiver)
        {
            _receivers.Add(channel, receiver);
        }

        private void Resolve(ServerPlayer player, Packet packet)
        {
            var channel = packet.Reader.ReadString();
            var receiver = _receivers[channel];
            receiver?.Handle(player, packet);
        }

        private Socket _socket;
        private bool _isRunning;

        public void Listen()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, 2228));
            _socket.Listen(10);
            _isRunning = true;
            new Thread(Thread).Start();
        }

        public void Stop()
        {
            _isRunning = false;
        }

        private void Thread()
        {
            while (_isRunning)
            {
                var client = new Connection(_socket.Accept());
                Connect(client);
            }
        }
        
        private void Connect(Connection client)
        {
            var clientInit = client.Receive();
            var username = clientInit.Reader.ReadString();

            var guid = Guid.NewGuid();
            var serverInit = Packet.Empty();
            serverInit.Writer.Write(guid.ToByteArray());
            client.Send(serverInit);
            
            NetworkManager.Execute(manager =>
            {
                var player = Player.Create(manager.serverPlayer,
                    new Vector3(0, 3, 0),
                    Quaternion.identity, guid, username);
                var serverPlayer = player.GetComponent<ServerPlayer>();
                serverPlayer.Connection = client;
                
                new Thread(() => ClientThread(serverPlayer)).Start();
            });
        }

        private void ClientThread(ServerPlayer player)
        {
            while (true)
            {
                var packet = player.Connection.Receive();
                if (packet == null || packet.Length == 0)
                {
                    break;
                }
                Resolve(player, packet);
            }
            
            NetworkManager.Execute((e) =>
            {
                var players = Object.FindObjectsByType<ServerPlayer>(FindObjectsSortMode.None);
                var packet = ClientShadowPlayerDisconnect.Build(player);
                foreach (var pl in players)
                {
                    if (pl == player)
                    {
                        continue;
                    }
                    pl.Send(ClientShadowPlayerDisconnect.Channel, packet);
                }
                Object.Destroy(player.gameObject);
            });
        }
    }
}