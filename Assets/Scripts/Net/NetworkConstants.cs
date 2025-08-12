using UnityEngine;

namespace Net
{
    /// <summary>
    /// Contains constants for network configuration and magic numbers
    /// </summary>
    public static class NetworkConstants
    {
        /// <summary>
        /// Default port for server communication
        /// </summary>
        public const int DefaultPort = 2228;
        
        /// <summary>
        /// Buffer size for network packet reception
        /// </summary>
        public const int BufferSize = 2 * 1024; // 2KB
        
        /// <summary>
        /// Maximum number of concurrent server connections
        /// </summary>
        public const int MaxConnections = 10;
        
        /// <summary>
        /// Default spawn position for new players
        /// </summary>
        public static readonly Vector3 DefaultSpawnPosition = new Vector3(0, 3, 0);
        
        /// <summary>
        /// Default spawn rotation for new players
        /// </summary>
        public static readonly Quaternion DefaultSpawnRotation = Quaternion.identity;
    }
}