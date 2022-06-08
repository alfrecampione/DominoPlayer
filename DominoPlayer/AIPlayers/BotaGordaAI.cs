using System.Linq;
using System;

namespace DominoPlayer.AI
{
    public class BotaGordaAI : AIPlayer
    {
        public BotaGordaAI(int playerID, DominoGame game)
        : base(playerID, game) { }

        protected override Move InternalGetMove()
        {
            var possiblePieces = GameReference.GetPlayablePieces(Hand);

            if (possiblePieces == null || !possiblePieces.Any())
                return Move.CreatePass(PlayerID);

            (Piece piece, bool canMatchLeft, bool canMatchRight, bool reverseLeft, bool reverseRight)
            = possiblePieces.OrderByDescending(p => p.piece.Left + p.piece.Right).First();

            if (canMatchRight && reverseRight || canMatchLeft && reverseLeft)
                piece.Reverse();

            if (canMatchLeft && canMatchRight)
            {
                bool right = new Random().Next(2) == 0;
                return Move.CreateMove(PlayerID, piece, right);
            }
            return Move.CreateMove(PlayerID, piece, canMatchRight);
        }
    }
}