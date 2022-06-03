namespace DominoPlayer.AI;

public class BotaGordaAI : DominoPlayer
{
    public BotaGordaAI(int playerID, DominoGame game)
    : base(playerID, game) { }

    public override Move GetMove()
    {
        var possiblePieces = GameReference.GetPlayablePieces(Hand ?? throw new DominoException("Game not started"));

        (Piece piece, bool right) = possiblePieces.MaxBy(p => p.piece[0] + p.piece[1]);

        return new Move(this.PlayerID, piece, false, right);
    }
}