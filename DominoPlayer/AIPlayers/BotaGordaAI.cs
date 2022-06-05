namespace DominoPlayer.AI;

public class BotaGordaAI : DominoPlayer
{
    public BotaGordaAI(int playerID, DominoGame game)
    : base(playerID, game) { }

    public override Move GetMove()
    {
        var possiblePieces = GameReference.GetPlayablePieces(Hand);

        (Piece piece, bool right) = possiblePieces.MaxBy(p => p.piece.GetLeft() + p.piece.GetRight());

        return new Move(this.PlayerID, piece, false, right);
    }
}