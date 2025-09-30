namespace _Scripts {
    using Mirror;
    using UnityEngine;

    public class ShogiPlayer : NetworkBehaviour
    {
        [SyncVar]
        public bool isPlayerOne;
    
        [SyncVar]
        public bool isMyTurn = false;

        private BoardManager board;

        void Start()
        {
            board = BoardManager.GetInstance();
        
            if (isServer && isPlayerOne)
            {
                isMyTurn = true;
            }
        }

        [Command]
        public void CmdMovePiece(int fromX, int fromY, int toX, int toY)
        {
            if (!isMyTurn) return;
        
            // Validate and execute the move on the server
            if (board.TryMovePiece(fromX, fromY, toX, toY))
            {
                // Broadcast the move to all clients
                RpcMovePiece(fromX, fromY, toX, toY);
            
                // Switch turns
                SwitchTurns();
            }
        }

        [ClientRpc]
        void RpcMovePiece(int fromX, int fromY, int toX, int toY)
        {
            if (!isServer)
            {
                board.TryMovePiece(fromX, fromY, toX, toY);
            }
        }

        void SwitchTurns()
        {
            // Find both players and switch turns
            ShogiPlayer[] players = FindObjectsOfType<ShogiPlayer>();
            foreach (ShogiPlayer player in players)
            {
                player.isMyTurn = !player.isMyTurn;
            }
        }

        public bool CanMove()
        {
            return isLocalPlayer && isMyTurn;
        }
    }
}