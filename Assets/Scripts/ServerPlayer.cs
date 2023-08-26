using Net;
using UnityEditor;
using UnityEngine;

public class ServerPlayer : MonoBehaviour
{
    private Player _player;
    public Connection Connection;

    public void Start()
    {
        _player = GetComponent<Player>();
    }

    public void Send(string channel, Packet body)
    {
        var packet = Packet.Empty();
        packet.Writer.Write(channel);
        packet.Writer.Write(body.GetBytes());
        Connection.Send(packet);
    }
}