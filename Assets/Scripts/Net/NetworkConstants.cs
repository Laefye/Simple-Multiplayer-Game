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
        
        /// <summary>
        /// Maximum packet size (to prevent memory issues)
        /// </summary>
        public const int MaxPacketSize = 1024 * 1024; // 1MB
        
        /// <summary>
        /// Connection timeout in milliseconds
        /// </summary>
        public const int ConnectionTimeoutMs = 30000; // 30 seconds
        
        /// <summary>
        /// Network thread sleep time in milliseconds
        /// </summary>
        public const int NetworkThreadSleepMs = 1;
        
        /// <summary>
        /// Maximum username length
        /// </summary>
        public const int MaxUsernameLength = 32;
        
        /// <summary>
        /// Packet channel identifiers
        /// </summary>
        public static class Channels
        {
            public const string SyncServerPlayer = "sync_server_player";
            public const string ShadowPlayerSync = "shadow_player_sync";
            public const string ShadowPlayerDisconnect = "shadow_player_disconnect";
            public const string GetAllPlayers = "get_all_players";
        }
    }
}