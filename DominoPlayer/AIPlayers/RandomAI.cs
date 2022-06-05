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

        if (possiblePieces == null || !possiblePieces.Any())
            return Move.CreatePass(PlayerID);

        var (piece, canMatchLeft, canMatchRight, reverseLeft, reverseRight) = possiblePieces.ElementAt(generator.Next(possiblePieces.Count()));

        if (canMatchRight && reverseRight || canMatchLeft && reverseLeft)
            piece.Reverse();

        if (canMatchLeft && canMatchRight)
        {
            bool right = new Random().Next(2) == 0;

            return Move.CreateMove(PlayerID, piece, right);
        }

        return Move.CreateMove(PlayerID, piece, canMatchRight);
    }
}