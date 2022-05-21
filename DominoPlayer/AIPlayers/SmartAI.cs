namespace DominoPlayer.AI;

public class SmartAI : Player
{
    List<List<int>> valuesPartnerNotHave;
    List<List<int>> valuesOpponentNothave;
    public SmartAI(int playerID, List<Piece> startingHand, DominoGame game, List<List<int>> valuesPartnerNotHave, List<List<int>> valuesOpponentNothave) :
        base(playerID, startingHand, game)
    {
        this.valuesPartnerNotHave = valuesPartnerNotHave;
        this.valuesOpponentNothave = valuesOpponentNothave;
    }

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

        int[,] pieceValues = new int[2, possiblePieces.Count()];

        //------------------------
        //Assigning values
        //
        //Finish assigning
        //------------------------

        int index = 0;
        int value = 0;
        for (int i = 0; i < pieceValues.GetLength(1); i++)
        {
            if (pieceValues[0, i] > value)
            {
                value = pieceValues[0, i];
                index = i;
            }
            if (pieceValues[1, i] > value)
            {
                value = pieceValues[1, i];
                index = i;
            }
        }
        var smartPieces = possiblePieces.ToArray();
        return new Move(playerID, smartPieces[index].Item1, false, rightPiece.CanMatch(smartPieces[index].Item1, true));
    }
}