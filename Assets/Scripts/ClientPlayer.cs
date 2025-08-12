using System;
using Net;
using UnityEngine;

/// <summary>
/// Handles client-side player behavior including movement and networking
/// </summary>
public class ClientPlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float moveSpeed = 5f;
    
    private Player _player;
    private NetworkManager _manager;
    private float _eyeX = 0;
    private float _eyeY = 0;

    /// <summary>
    /// Initialize the client player
    /// </summary>
    public void Start()
    {
        _player = GetComponent<Player>();
        _manager = FindAnyObjectByType<NetworkManager>();
        
        // Lock cursor for FPS-style camera control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Handle input and movement each frame
    /// </summary>
    public void Update()
    {
        HandleMouseLook();
        HandleMovement();
        
        // Allow escape to unlock cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// Send position updates to server at fixed intervals
    /// </summary>
    public void FixedUpdate()
    {
        if (_manager?.Client != null && _manager.Client.IsConnected)
        {
            _manager.Client.Send(SyncServerPlayer.Channel, SyncServerPlayer.Build(this));
        }
    }

    private void HandleMouseLook()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            _eyeX += Input.GetAxis("Mouse X") * mouseSensitivity;
            _eyeY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            
            // Clamp vertical rotation to prevent over-rotation
            _eyeY = Mathf.Clamp(_eyeY, -90f, 90f);
            
            transform.rotation = Quaternion.Euler(_eyeY, _eyeX, 0);
        }
    }

    private void HandleMovement()
    {
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");
        
        // Calculate movement relative to current rotation
        var movement = (transform.forward * vertical + transform.right * horizontal) * moveSpeed * Time.deltaTime;
        transform.position += movement;
    }
}
