using System;
using System.Net;
using Net;
using UnityEngine;

public class ConnectTool : MonoBehaviour
{
    private NetworkManager _networkManager;
    [SerializeField] private TMPro.TMP_InputField username;
    [SerializeField] private TMPro.TMP_InputField ip;
    
    public void Start()
    {
        _networkManager = FindAnyObjectByType<NetworkManager>();
    }

    public void Listen()
    {
        _networkManager.Server.Listen();
        _networkManager.Server.Register(SyncServerPlayer.Channel, new SyncServerPlayer());
        _networkManager.Server.Register(ServerGetAllPlayers.Channel, new ServerGetAllPlayers());
    }

    public void Connect()
    {
        _networkManager.Client.Register(ClientShadowPlayer.Channel, new ClientShadowPlayer());
        _networkManager.Client.Register(ClientShadowPlayerDisconnect.Channel, new ClientShadowPlayerDisconnect());
        _networkManager.Client.Connect(IPAddress.Parse(ip.text), username.text);
    }
}
