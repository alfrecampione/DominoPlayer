using System;
using System.Linq;

namespace DominoPlayer.AI
{
    public class BotaGordaAI : AIPlayer
    {
        public BotaGordaAI(int playerID, DominoGame game)
            : base(playerID, game)
        {
        }

        protected override Move InternalGetMove()
        {
            var possiblePieces = GameReference.GetPlayablePieces(Hand);

            if (possiblePieces == null || !possiblePieces.Any())
                return Move.CreatePass(PlayerID);

            (Piece piece, bool canMatchLeft, bool canMatchRight, bool reverseLeft, bool reverseRight)
                = possiblePieces.OrderByDescending(p => p.piece.Left + p.piece.Right).First();
            
            if (canMatchLeft && canMatchRight)
            {
                Random rnd = new Random();
                int side = rnd.Next(0, 2);
                if (side == 0)
                    canMatchRight = false;
                if (side == 1)
                    canMatchLeft = false;
            }

            if (canMatchLeft && reverseLeft)
            {
                piece.Reverse();
                return Move.CreateMove(PlayerID, piece, false);
            }

            if (canMatchLeft)
                return Move.CreateMove(PlayerID, piece, false);
            if (canMatchRight && reverseRight)
            {
                piece.Reverse();
                return Move.CreateMove(PlayerID, piece, true);
            }
            else
                return Move.CreateMove(PlayerID, piece, true);
        }
    }
}