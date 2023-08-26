using System;
using UnityEngine;

public class ShadowPlayer : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshPro text;
    private Player _player;
    private Rigidbody _rigidbody;
    
    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
        text.text = _player.username;
    }

    public Vector3 Position
    {
        set => _rigidbody.MovePosition(value);
    }
    
    public Quaternion Rotation
    {
        set => _rigidbody.MoveRotation(value);
    }
}
