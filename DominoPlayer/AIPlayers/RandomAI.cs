namespace DominoPlayer.AI;

public class RandomAI : DominoPlayer
{
    public RandomAI(int playerID, DominoGame game)
    : base(playerID, game) { }

    public override Move GetMove()
    {
        var possiblePieces = GameReference.GetPlayablePieces(Hand ?? throw new DominoException("Game not started"));

        Random rnd = new();
        int index = rnd.Next(possiblePieces.Count());
        var randomPieces = possiblePieces.ToArray();
        return new Move(PlayerID, randomPieces[index].piece, false, randomPieces[index].right);
    }
}