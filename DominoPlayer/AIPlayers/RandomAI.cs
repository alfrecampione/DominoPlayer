namespace DominoPlayer.AI;

public class RandomAI : IDominoPlayer
{
    public int PlayerID { get; set; }
    public List<Piece>? hand;
    public DominoGame GameReference { get; set; }

    public RandomAI(int PlayerID, DominoGame game)
    {
        this.PlayerID = PlayerID;
        this.GameReference = game;
    }

    public void StartGame(List<Piece> startingHand)
    {
        hand = new(startingHand);
    }
    public List<Piece> GetCurrentHand() => hand ?? throw new DominoException("Game not started");

    public Move GetMove()
    {
        var possiblePieces = GameReference.GetPlayablePieces(hand ?? throw new DominoException("Game not started"));

        Random rnd = new();
        int index = rnd.Next(possiblePieces.Count());
        var randomPieces = possiblePieces.ToArray();
        return new Move(PlayerID, randomPieces[index].piece, false, randomPieces[index].right);
    }
}