using System.Collections.Generic;
namespace DominoPlayer
{
    public abstract class DominoPlayer
    {
        public int PlayerID { get; }
        protected DominoGame GameReference { get; }
        protected List<Piece> Hand { get; set; }
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
        //We need a way to have the pieces for GameOver conditions, it's not useful make a function foreach case
        public Piece[] GetPieces()
        {
            Piece[] copy = new Piece[Hand.Count];
            Hand.CopyTo(copy);
            return copy;
        }
    }
}