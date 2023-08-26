using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Net
{
    public class ClientShadowPlayerDisconnect : ClientNetworkManager.IClientReceiver
    {
        public static string Channel = "shadow_player_disconnect";
        
        public void Handle(Packet packet)
        {
            var id = new Guid(packet.Reader.ReadBytes(16));
            NetworkManager.Execute((manager) =>
            {
                var player = Object.FindObjectsByType<ShadowPlayer>(FindObjectsSortMode.None).FirstOrDefault(pl => pl.GetComponent<Player>().ID == id);
                if (player != null)
                {
                    Object.Destroy(player.gameObject);
                }
            });
        }

        public static Packet Build(ServerPlayer player)
        {
            var packet = Packet.Empty();
            var e = player.GetComponent<Player>();
            packet.Writer.Write(e.ID.ToByteArray());
            return packet;
        }   
    }
}