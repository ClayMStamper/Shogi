namespace _Scripts {
    using Mirror;
    using UnityEngine;

    public class ShogiNetworkManager : NetworkManager
    {
        [Header("Shogi Settings")]
        public GameObject boardManagerPrefab;
    
        private int playersReady = 0;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // Spawn the player
            GameObject player = Instantiate(playerPrefab);
            NetworkServer.AddPlayerForConnection(conn, player);
        
            playersReady++;
        
            // When both players are connected, set their roles
            if (playersReady == 1)
            {
                player.GetComponent<ShogiPlayer>().isPlayerOne = true;
            }
            else if (playersReady == 2)
            {
                player.GetComponent<ShogiPlayer>().isPlayerOne = false;
            }
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            playersReady--;
            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            playersReady = 0;
            base.OnStopServer();
        }
    }
}