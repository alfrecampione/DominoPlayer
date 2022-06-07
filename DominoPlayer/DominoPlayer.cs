using System.Collections.Generic;
namespace DominoPlayer
{
    public abstract class DominoPlayer
    {
        public int PlayerID { get; }
        protected DominoGame GameReference { get; }
        public List<Piece> Hand { get; set; }
        public int Count { get { return Hand.Count; } }
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