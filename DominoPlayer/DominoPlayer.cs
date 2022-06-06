using System.Collections.Generic;
namespace DominoPlayer
{
    public abstract class DominoPlayer
    {
        public int PlayerID { get; }
        protected DominoGame GameReference { get; }
        protected List<Piece> Hand { get; set; }

        public DominoPlayer(int playerID, DominoGame game)
        => (PlayerID, GameReference, Hand) = (playerID, game, new List<Piece>());
        public abstract Move GetMove();
        public virtual int PiecesInHand() => Hand.Count;
        public virtual void StartGame(IEnumerable<Piece> startingHand)
        {
            Hand.Clear();
            Hand.AddRange(startingHand);
        }
    }
}