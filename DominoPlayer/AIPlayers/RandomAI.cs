namespace DominoPlayer;

public class RandomAI : Player
{
    public RandomAI(int playerID, List<Piece> startingHand, DominoGame game) :
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

        Random rnd = new();
        int index = rnd.Next(possiblePieces.Count());
        var randomPieces = possiblePieces.ToArray();
        return new Move(playerID, randomPieces[index].Item1, false, randomPieces[index].Item2);
    }
}