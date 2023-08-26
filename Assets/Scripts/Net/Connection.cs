using Net;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;

namespace Net
{
    public class Connection
    {
        private readonly Socket _socket;
        
        public Connection(Socket socket)
        {
            _socket = socket;
        }

        public Packet Receive()
        {
            var buffer = new byte[2 * 1024];
            var count = _socket.Receive(buffer);
            if (count > 0)
            {
                return Packet.From(buffer, count);
            }
            return null;
        }

        public void Send(Packet packet)
        {
            _socket.Send(packet.GetBytes());
        }
    }
}