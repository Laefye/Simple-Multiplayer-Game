using System;
using UnityEngine;

namespace Net
{
    /// <summary>
    /// Utility class for validating network input and data
    /// </summary>
    public static class NetworkValidator
    {
        /// <summary>
        /// Validates a username for network use
        /// </summary>
        /// <param name="username">Username to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;
                
            if (username.Length > NetworkConstants.MaxUsernameLength)
                return false;
                
            // Check for invalid characters (basic validation)
            foreach (char c in username)
            {
                if (char.IsControl(c) || c == '\n' || c == '\r' || c == '\t')
                    return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Validates a packet size to prevent memory issues
        /// </summary>
        /// <param name="packetSize">Size of the packet in bytes</param>
        /// <returns>True if valid size, false otherwise</returns>
        public static bool IsValidPacketSize(long packetSize)
        {
            return packetSize > 0 && packetSize <= NetworkConstants.MaxPacketSize;
        }
        
        /// <summary>
        /// Validates a player position to prevent unrealistic values
        /// </summary>
        /// <param name="position">Position to validate</param>
        /// <param name="maxDistance">Maximum distance from origin (default 1000)</param>
        /// <returns>True if valid position, false otherwise</returns>
        public static bool IsValidPosition(Vector3 position, float maxDistance = 1000f)
        {
            // Check for NaN or infinity
            if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z))
                return false;
                
            if (float.IsInfinity(position.x) || float.IsInfinity(position.y) || float.IsInfinity(position.z))
                return false;
                
            // Check if position is within reasonable bounds
            return position.magnitude <= maxDistance;
        }
        
        /// <summary>
        /// Validates a rotation quaternion
        /// </summary>
        /// <param name="rotation">Rotation to validate</param>
        /// <returns>True if valid rotation, false otherwise</returns>
        public static bool IsValidRotation(Quaternion rotation)
        {
            // Check for NaN or infinity
            if (float.IsNaN(rotation.x) || float.IsNaN(rotation.y) || float.IsNaN(rotation.z) || float.IsNaN(rotation.w))
                return false;
                
            if (float.IsInfinity(rotation.x) || float.IsInfinity(rotation.y) || float.IsInfinity(rotation.z) || float.IsInfinity(rotation.w))
                return false;
                
            // Check if quaternion is normalized (within tolerance)
            float magnitude = Mathf.Sqrt(rotation.x * rotation.x + rotation.y * rotation.y + rotation.z * rotation.z + rotation.w * rotation.w);
            return Mathf.Abs(magnitude - 1.0f) < 0.01f;
        }
        
        /// <summary>
        /// Sanitizes a username for safe use
        /// </summary>
        /// <param name="username">Username to sanitize</param>
        /// <returns>Sanitized username</returns>
        public static string SanitizeUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return "Player";
                
            username = username.Trim();
            
            if (username.Length > NetworkConstants.MaxUsernameLength)
                username = username.Substring(0, NetworkConstants.MaxUsernameLength);
                
            // Remove control characters
            string sanitized = "";
            foreach (char c in username)
            {
                if (!char.IsControl(c) && c != '\n' && c != '\r' && c != '\t')
                    sanitized += c;
            }
            
            return string.IsNullOrWhiteSpace(sanitized) ? "Player" : sanitized;
        }
    }
}