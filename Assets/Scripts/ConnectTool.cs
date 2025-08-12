using System;
using System.Net;
using Net;
using UnityEngine;

/// <summary>
/// Provides UI controls for connecting to or hosting a multiplayer game
/// </summary>
public class ConnectTool : MonoBehaviour
{
    private NetworkManager _networkManager;
    
    [SerializeField] private TMPro.TMP_InputField username;
    [SerializeField] private TMPro.TMP_InputField ip;
    
    /// <summary>
    /// Initialize the connect tool
    /// </summary>
    public void Start()
    {
        _networkManager = FindAnyObjectByType<NetworkManager>();
        
        // Set default values if fields are empty
        if (string.IsNullOrEmpty(ip.text))
        {
            ip.text = "127.0.0.1";
        }
        if (string.IsNullOrEmpty(username.text))
        {
            username.text = "Player";
        }
    }

    /// <summary>
    /// Starts the server and begins listening for connections
    /// </summary>
    public void Listen()
    {
        try
        {
            _networkManager.Server.Listen();
            _networkManager.Server.Register(SyncServerPlayer.Channel, new SyncServerPlayer());
            _networkManager.Server.Register(ServerGetAllPlayers.Channel, new ServerGetAllPlayers());
            Debug.Log("Server started successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to start server: {ex.Message}");
        }
    }

    /// <summary>
    /// Connects to a server as a client
    /// </summary>
    public void Connect()
    {
        try
        {
            // Validate and sanitize username
            string sanitizedUsername = NetworkValidator.SanitizeUsername(username.text);
            if (!NetworkValidator.IsValidUsername(sanitizedUsername))
            {
                Debug.LogError("Invalid username provided");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(ip.text))
            {
                Debug.LogError("IP address cannot be empty");
                return;
            }
            
            _networkManager.Client.Register(ClientShadowPlayer.Channel, new ClientShadowPlayer());
            _networkManager.Client.Register(ClientShadowPlayerDisconnect.Channel, new ClientShadowPlayerDisconnect());
            _networkManager.Client.Connect(IPAddress.Parse(ip.text), sanitizedUsername);
            
            Debug.Log($"Attempting to connect to {ip.text} as '{sanitizedUsername}'");
        }
        catch (FormatException)
        {
            Debug.LogError("Invalid IP address format");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to connect: {ex.Message}");
        }
    }
}
