using System;
using System.Linq;

namespace DominoPlayer.AI
{
    public class RandomAI : AIPlayer
    {
        private readonly Random generator;

        public RandomAI(int playerID, DominoGame game)
        : base(playerID, game)
        {
            generator = new Random();
        }

        protected override Move InternalGetMove()
        {
            var possiblePieces = GameReference.GetPlayablePieces(Hand);

            if (possiblePieces == null || !possiblePieces.Any())
                return Move.CreatePass(PlayerID);

            (Piece piece, bool canMatchLeft, bool canMatchRight, bool reverseLeft, bool reverseRight)
            = possiblePieces.ElementAt(generator.Next(possiblePieces.Count()));

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