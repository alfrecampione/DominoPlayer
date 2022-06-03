namespace DominoPlayer.AI;

public class RandomAI : DominoPlayer
{
    private readonly Random generator;

    public RandomAI(int playerID, DominoGame game)
    : base(playerID, game)
    {
        generator = new Random();
    }

    public override Move GetMove()
    {
        var possiblePieces = GameReference.GetPlayablePieces(Hand);

        var (piece, right) = possiblePieces.ElementAt(generator.Next(possiblePieces.Count()));

        return new Move(PlayerID, piece, false, right);
    }
}