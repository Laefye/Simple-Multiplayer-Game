using UnityEngine;

/// <summary>
/// Represents a remote player visible to the local client
/// </summary>
public class ShadowPlayer : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshPro text;
    
    private Player _player;
    private Rigidbody _rigidbody;
    
    /// <summary>
    /// Initialize the shadow player
    /// </summary>
    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
        
        if (text != null && _player != null)
        {
            text.text = _player.username;
        }
    }

    /// <summary>
    /// Sets the position of the shadow player using physics
    /// </summary>
    public Vector3 Position
    {
        set
        {
            if (_rigidbody != null)
            {
                _rigidbody.MovePosition(value);
            }
            else
            {
                transform.position = value;
            }
        }
    }
    
    /// <summary>
    /// Sets the rotation of the shadow player using physics
    /// </summary>
    public Quaternion Rotation
    {
        set
        {
            if (_rigidbody != null)
            {
                _rigidbody.MoveRotation(value);
            }
            else
            {
                transform.rotation = value;
            }
        }
    }
}
