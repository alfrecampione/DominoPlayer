namespace DominoPlayer;

public interface IDominoPlayer
{
    public int PlayerID { get; }
    public DominoGame GameReference { get; }

    public Move GetMove();
    public List<Piece> GetCurrentHand();
    public void StartGame(List<Piece> startingHand);
}