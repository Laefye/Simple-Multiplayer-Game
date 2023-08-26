using System;
using Net;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Net
{
    public class ClientNetworkManager
    {
        public interface IClientReceiver
        {
            void Handle(Packet packet);
        }

        private readonly Dictionary<string, IClientReceiver> _receivers = new Dictionary<string, IClientReceiver>();

        public void Register(string channel, IClientReceiver receiver)
        {
            _receivers.Add(channel, receiver);
        }

        public void Resolve(Packet packet)
        {
            var channel = packet.Reader.ReadString();
            var receiver = _receivers[channel];
            receiver?.Handle(packet);
        }

        private Socket _socket;
        private Connection _connection;

        public void Connect(IPAddress ip, string username)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(new IPEndPoint(ip, 2228));
            _connection = new Connection(_socket);
            Init(username);
        }

        private void Init(string username)
        {
            var packet = Packet.Empty();
            packet.Writer.Write(username);
            _connection.Send(packet);

            var serverPacket = _connection.Receive();
            var guid = new Guid(serverPacket.Reader.ReadBytes(16));
            
            NetworkManager.Execute(manager =>
            {
                var player = Player.Create(manager.clientPlayer,
                    new Vector3(0, 3, 0),
                    Quaternion.identity, guid, username);
            });
            new Thread(Thread).Start();
        }

        private void Thread()
        {
            while (true)
            {
                var packet = _connection.Receive();
                if (packet == null)
                {
                    break;
                }
                Resolve(packet);
            }
        }

        public void Send(string channel, Packet body)
        {
            var packet = Packet.Empty();
            packet.Writer.Write(channel);
            packet.Writer.Write(body.GetBytes());
            _connection.Send(packet);
        }
    }
}