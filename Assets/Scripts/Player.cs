using Net;
using System;
using System.Collections;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Serialization;


public class Player : MonoBehaviour
{
    public Guid ID;
    public string username;
    [SerializeField] private string id;

    public void Start()
    {
        id = ID.ToString();
    }

    public static Player Create(GameObject basePrefab, Vector3 position, Quaternion rotation, Guid id, string username)
    {
        var playerObject = Instantiate(basePrefab, position, rotation);
        var player = playerObject.GetComponent<Player>();
        player.ID = id;
        player.username = username;
        return player;
    }
}
