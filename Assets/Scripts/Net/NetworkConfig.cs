using UnityEngine;

namespace Net
{
    /// <summary>
    /// Scriptable object for configuring network settings
    /// </summary>
    [CreateAssetMenu(fileName = "NetworkConfig", menuName = "Network/Network Configuration")]
    public class NetworkConfig : ScriptableObject
    {
        [Header("Connection Settings")]
        [Tooltip("Default port for server communication")]
        [Range(1024, 65535)]
        public int port = NetworkConstants.DefaultPort;
        
        [Tooltip("Buffer size for network packet reception")]
        [Range(512, 8192)]
        public int bufferSize = NetworkConstants.BufferSize;
        
        [Tooltip("Maximum number of concurrent server connections")]
        [Range(1, 100)]
        public int maxConnections = NetworkConstants.MaxConnections;
        
        [Header("Player Settings")]
        [Tooltip("Default spawn position for new players")]
        public Vector3 defaultSpawnPosition = NetworkConstants.DefaultSpawnPosition;
        
        [Tooltip("Default spawn rotation for new players")]
        public Vector3 defaultSpawnRotation = Vector3.zero;
        
        [Header("Gameplay Settings")]
        [Tooltip("Mouse sensitivity for player camera control")]
        [Range(0.1f, 10f)]
        public float mouseSensitivity = 2f;
        
        [Tooltip("Player movement speed")]
        [Range(1f, 20f)]
        public float moveSpeed = 5f;
        
        [Tooltip("Send rate for player position updates (per second)")]
        [Range(10, 60)]
        public int updateRate = 20;
        
        /// <summary>
        /// Gets the spawn rotation as a quaternion
        /// </summary>
        public Quaternion DefaultSpawnRotationQuaternion => Quaternion.Euler(defaultSpawnRotation);
        
        /// <summary>
        /// Gets the time interval between position updates
        /// </summary>
        public float UpdateInterval => 1f / updateRate;
    }
}