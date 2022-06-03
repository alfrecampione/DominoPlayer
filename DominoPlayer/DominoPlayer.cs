namespace DominoPlayer;

public abstract class DominoPlayer
{
    public int PlayerID { get; }
    private DominoGame GameReference { get; }
    private List<Piece> Hand { get; set; }

    public DominoPlayer(int PlayerID, DominoGame game)
    {
        this.PlayerID = PlayerID;
        this.GameReference = game;
        this.Hand = new();
    }

    public abstract Move GetMove();
    public virtual int PiecesInHand() => Hand.Count;
    public virtual bool HasPlayablePieces() => GameReference.HasPlayablePieces(Hand);
    public virtual void StartGame(List<Piece> startingHand)
    {
        Hand.Clear();
        Hand.AddRange(startingHand);
    }
}