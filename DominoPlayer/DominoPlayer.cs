using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
namespace DominoPlayer
{
    public abstract class DominoPlayer
    {
        public int PlayerID { get; }
        protected DominoGame GameReference { get; }

        protected List<Piece> Hand { get; }
        public int Count { get { return Hand.Count; } }
        public DominoPlayer(int playerID, DominoGame game)
        => (PlayerID, GameReference, Hand) = (playerID, game, new List<Piece>());
        public double GetPlayerScore() => GameReference.gameRules.GetHandScore(GameReference, Hand);
        public Move GetMove()
        {
            Move move = InternalGetMove();
            if (!move.passed)
                Hand.RemoveAt(Hand.FindIndex(p => p == move.piecePlaced));
            return move;
        }
        protected abstract Move InternalGetMove();
        public virtual int PiecesInHand => Hand.Count;
        public virtual void StartGame(IEnumerable<Piece> startingHand)
        {
            Hand.Clear();
            Hand.AddRange(startingHand);
        }
    }
}