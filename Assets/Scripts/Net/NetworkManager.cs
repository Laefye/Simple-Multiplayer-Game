using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Net
{
    /// <summary>
    /// Central manager for networking operations, handling both client and server functionality
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        /// <summary>
        /// Client network manager instance
        /// </summary>
        public readonly ClientNetworkManager Client = new ClientNetworkManager();
        
        /// <summary>
        /// Server network manager instance
        /// </summary>
        public readonly ServerNetworkManager Server = new ServerNetworkManager();
        
        // Thread-safe queue for actions that need to run on the main thread
        private static readonly ConcurrentQueue<Action<NetworkManager>> Actions = new ConcurrentQueue<Action<NetworkManager>>();
        
        [Header("Player Prefabs")]
        [SerializeField] public GameObject clientPlayer;
        [SerializeField] public GameObject serverPlayer;
        [SerializeField] public GameObject shadowPlayer;

        /// <summary>
        /// Process queued actions on the main thread
        /// </summary>
        public void FixedUpdate()
        {
            // Process all queued actions
            while (Actions.TryDequeue(out var action))
            {
                try
                {
                    action.Invoke(this);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error executing network action: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Queues an action to be executed on the main thread during the next FixedUpdate
        /// </summary>
        /// <param name="action">Action to execute on the main thread</param>
        public static void Execute(Action<NetworkManager> action)
        {
            if (action != null)
            {
                Actions.Enqueue(action);
            }
        }

        /// <summary>
        /// Clean up networking resources when the manager is destroyed
        /// </summary>
        private void OnDestroy()
        {
            try
            {
                Client?.Disconnect();
                Server?.Stop();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error during NetworkManager cleanup: {ex.Message}");
            }
        }
    }
}
