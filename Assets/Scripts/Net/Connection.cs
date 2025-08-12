using System;
using System.Net.Sockets;
using UnityEngine;

namespace Net
{
    /// <summary>
    /// Manages a TCP network connection with packet-based communication
    /// </summary>
    public class Connection : IDisposable
    {
        private readonly Socket _socket;
        private bool _disposed = false;
        
        /// <summary>
        /// Creates a new connection wrapper around a socket
        /// </summary>
        /// <param name="socket">The underlying TCP socket</param>
        public Connection(Socket socket)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }

        /// <summary>
        /// Receives a packet from the connection
        /// </summary>
        /// <returns>The received packet, or null if connection closed or error occurred</returns>
        public Packet Receive()
        {
            if (_disposed)
                return null;
                
            try
            {
                var buffer = new byte[NetworkConstants.BufferSize];
                var count = _socket.Receive(buffer);
                if (count > 0)
                {
                    return Packet.From(buffer, count);
                }
                return null;
            }
            catch (SocketException ex)
            {
                Debug.LogWarning($"Socket error during receive: {ex.Message}");
                return null;
            }
            catch (ObjectDisposedException)
            {
                return null;
            }
        }

        /// <summary>
        /// Sends a packet through the connection
        /// </summary>
        /// <param name="packet">The packet to send</param>
        /// <returns>True if sent successfully, false otherwise</returns>
        public bool Send(Packet packet)
        {
            if (_disposed || packet == null)
                return false;
                
            // Validate packet size
            if (!NetworkValidator.IsValidPacketSize(packet.Length))
            {
                Debug.LogWarning($"Packet too large ({packet.Length} bytes), rejecting send");
                return false;
            }
                
            try
            {
                _socket.Send(packet.GetBytes());
                return true;
            }
            catch (SocketException ex)
            {
                Debug.LogWarning($"Socket error during send: {ex.Message}");
                return false;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets whether the connection is still connected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (_disposed || _socket == null)
                    return false;
                    
                try
                {
                    return _socket.Connected;
                }
                catch (ObjectDisposedException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Disposes the connection and closes the underlying socket
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    _socket?.Close();
                    _socket?.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error disposing connection: {ex.Message}");
                }
                finally
                {
                    _disposed = true;
                }
            }
        }
    }
}