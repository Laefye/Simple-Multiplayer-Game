using System;
using System.Threading;
using Net;
using UnityEditor;
using UnityEngine;

public class ClientPlayer : MonoBehaviour
{
    private Player _player;
    private NetworkManager _manager;

    public void Start()
    {
        _player = GetComponent<Player>();
        _manager = FindAnyObjectByType<NetworkManager>();
        // _manager.Client.Send(ServerGetAllPlayers.Channel, Packet.Empty());
    }

    private float _eyeX = 0;
    private float _eyeY = 0;

    public void Update()
    {
        _eyeX += Input.GetAxis("Mouse X");
        _eyeY -= Input.GetAxis("Mouse Y");
        transform.rotation = Quaternion.Euler(_eyeY, _eyeX, 0);
        transform.position += transform.forward * Time.deltaTime * Input.GetAxis("Vertical");
    }

    public void FixedUpdate()
    {
        _manager.Client.Send(SyncServerPlayer.Channel, SyncServerPlayer.Build(this));
    }
}
