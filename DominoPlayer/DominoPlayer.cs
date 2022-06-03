namespace DominoPlayer;

public abstract class DominoPlayer
{
    public int PlayerID { get; }
    protected DominoGame GameReference { get; }
    protected List<Piece> Hand { get; set; }

    public DominoPlayer(int playerID, DominoGame game)
    {
        this.PlayerID = playerID;
        this.GameReference = game;
        this.Hand = new();
    }

    public abstract Move GetMove();
    public virtual int PiecesInHand() => Hand.Count;
    public virtual void StartGame(List<Piece> startingHand)
    {
        Hand.Clear();
        Hand.AddRange(startingHand);
    }
}