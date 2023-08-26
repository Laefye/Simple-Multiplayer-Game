using UnityEngine;

namespace Net
{
    public class SyncServerPlayer : ServerNetworkManager.IServerReceiver
    {
        public static string Channel = "sync_server_player";
        
        public void Handle(ServerPlayer player, Packet packet)
        {
            NetworkManager.Execute((e) =>
            {
                player.transform.position = packet.ReadVector3();
                player.transform.rotation = packet.ReadQuaternion();
                var pt = ClientShadowPlayer.Build(player);
                var players = Object.FindObjectsByType<ServerPlayer>(FindObjectsSortMode.None);
                foreach (var pl in players)
                {
                    pl.Send(ClientShadowPlayer.Channel, pt);
                }
            });
        }

        public static Packet Build(ClientPlayer player)
        {
            var packet = Packet.Empty();
            packet.Write(player.transform.position);
            packet.Write(player.transform.rotation);
            return packet;
        }
    }
}