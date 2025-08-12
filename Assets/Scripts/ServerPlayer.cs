using Net;
using UnityEngine;

/// <summary>
/// Represents a player on the server side with network communication capabilities
/// </summary>
public class ServerPlayer : MonoBehaviour
{
    private Player _player;
    
    /// <summary>
    /// Network connection to the client
    /// </summary>
    public Connection Connection { get; set; }

    /// <summary>
    /// Initialize the server player
    /// </summary>
    public void Start()
    {
        _player = GetComponent<Player>();
    }

    /// <summary>
    /// Sends a packet to the connected client
    /// </summary>
    /// <param name="channel">Channel identifier</param>
    /// <param name="body">Packet body data</param>
    /// <returns>True if sent successfully, false otherwise</returns>
    public bool Send(string channel, Packet body)
    {
        if (Connection == null || !Connection.IsConnected)
        {
            return false;
        }
        
        var packet = Packet.Empty();
        packet.Writer.Write(channel);
        packet.Writer.Write(body.GetBytes());
        return Connection.Send(packet);
    }
    
    /// <summary>
    /// Clean up when the server player is destroyed
    /// </summary>
    private void OnDestroy()
    {
        Connection?.Dispose();
    }
}