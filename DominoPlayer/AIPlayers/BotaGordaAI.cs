using System.IO.MemoryMappedFiles;
namespace DominoPlayer.AI;

public class BotaGordaAI : DominoPlayer
{
    public BotaGordaAI(int playerID, DominoGame game)
    : base(playerID, game) { }

    public override Move GetMove()
    {
        var possiblePieces = GameReference.GetPlayablePieces(Hand);

        if (possiblePieces == null || !possiblePieces.Any())
            return Move.CreatePass(PlayerID);

        var (piece, canMatchLeft, canMatchRight, reverseLeft, reverseRight) = possiblePieces.MaxBy(p => p.piece.Left + p.piece.Right);

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