using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Net
{
    public class ClientShadowPlayer : ClientNetworkManager.IClientReceiver
    {
        public static string Channel = "shadow_player_sync";
        
        public void Handle(Packet packet)
        {
            var id = new Guid(packet.Reader.ReadBytes(16));
            var username = packet.Reader.ReadString();
            var position = packet.ReadVector3();
            var rotation = packet.ReadQuaternion();
            NetworkManager.Execute((manager) =>
            {
                var client = Object.FindAnyObjectByType<ClientPlayer>().GetComponent<Player>();
                if (client.ID == id)
                {
                    return;
                }
                var player = Object.FindObjectsByType<ShadowPlayer>(FindObjectsSortMode.None).FirstOrDefault(pl => pl.GetComponent<Player>().ID == id);
                if (player != null)
                {
                    player.Position = position;
                    player.Rotation = rotation;
                }
                else
                {
                    Player.Create(manager.shadowPlayer,
                        position,
                        rotation, id, username);
                }
            });
        }

        public static Packet Build(ServerPlayer player)
        {
            var packet = Packet.Empty();
            var e = player.GetComponent<Player>();
            packet.Writer.Write(e.ID.ToByteArray());
            packet.Writer.Write(e.username);
            packet.Write(player.transform.position);
            packet.Write(player.transform.rotation);
            return packet;
        }   
    }
}