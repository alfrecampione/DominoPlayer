namespace DominoPlayer.AI;

public class BotaGordaAI : Player
{
    public BotaGordaAI(int playerID, List<Piece> startingHand, DominoGame game) :
        base(playerID, startingHand, game)
    { }

    public override Move GetMove()
    {
        (Piece leftPiece, Piece rightPiece) =
            (
                gameReference.GetPieceOnExtreme(false),
                gameReference.GetPieceOnExtreme(true)
            );

        var possiblePieces = from piece in hand
                             where leftPiece.CanMatch(piece, false) || rightPiece.CanMatch(piece, true)
                             select (piece, rightPiece.CanMatch(piece, true));

        (Piece piece, bool right) gordastPiece = possiblePieces.MaxBy(p => p.piece[0] + p.piece[1]);

        return new Move(this.playerID, gordastPiece.piece, false, gordastPiece.right);
    }
}