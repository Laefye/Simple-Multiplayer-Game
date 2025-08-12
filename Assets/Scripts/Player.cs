using System;
using UnityEngine;


/// <summary>
/// Represents a player in the multiplayer game
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// Unique identifier for this player
    /// </summary>
    public Guid ID;
    
    /// <summary>
    /// Display name for this player
    /// </summary>
    public string username;
    
    [SerializeField] private string id;

    /// <summary>
    /// Initialize the player component
    /// </summary>
    public void Start()
    {
        id = ID.ToString();
    }

    /// <summary>
    /// Creates a new player instance from a prefab
    /// </summary>
    /// <param name="basePrefab">The prefab to instantiate</param>
    /// <param name="position">World position for the player</param>
    /// <param name="rotation">World rotation for the player</param>
    /// <param name="id">Unique identifier</param>
    /// <param name="username">Display name</param>
    /// <returns>The created Player component</returns>
    public static Player Create(GameObject basePrefab, Vector3 position, Quaternion rotation, Guid id, string username)
    {
        var playerObject = Instantiate(basePrefab, position, rotation);
        var player = playerObject.GetComponent<Player>();
        player.ID = id;
        player.username = username;
        return player;
    }
}
