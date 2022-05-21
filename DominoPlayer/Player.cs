namespace DominoPlayer;

public abstract class Player
{
    protected readonly int playerID;
    protected List<Piece> hand;
    protected DominoGame gameReference;
    public Player(int playerID, List<Piece> startingHand, DominoGame game)
    {
        this.playerID = playerID;
        this.hand = startingHand;
        this.gameReference = game;
    }

    public abstract Move GetMove();
}