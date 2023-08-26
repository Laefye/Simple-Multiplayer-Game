using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Net
{
    public class NetworkManager : MonoBehaviour
    {
        public readonly ClientNetworkManager Client = new ClientNetworkManager();
        public readonly ServerNetworkManager Server = new ServerNetworkManager();
        private static readonly Stack<Action<NetworkManager>> Actions = new Stack<Action<NetworkManager>>();
        public GameObject clientPlayer;
        public GameObject serverPlayer;
        public GameObject shadowPlayer;

        public void FixedUpdate()
        {
            while (Actions.Count > 0)
            {
                Actions.Pop().Invoke(this);
            }
        }

        public static void Execute(Action<NetworkManager> action)
        {
            Actions.Push(action);
        }
    }
}
