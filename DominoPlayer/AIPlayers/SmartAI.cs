namespace DominoPlayer.AI;

public class SmartAI : IDominoPlayer
{
    public int PlayerID { get; set; }
    private List<Piece>? hand;
    public DominoGame GameReference { get; set; }
    List<List<int>> valuesPartnerNotHave;
    List<List<int>> valuesOpponentNothave;

    public SmartAI(int playerID, DominoGame game, List<List<int>> valuesPartnerNotHave, List<List<int>> valuesOpponentNothave)
    {
        this.PlayerID = playerID;
        this.GameReference = game;
        this.valuesPartnerNotHave = valuesPartnerNotHave;
        this.valuesOpponentNothave = valuesOpponentNothave;
    }

    public void StartGame(List<Piece> startingHand)
    {
        hand = new(startingHand);
    }
    public List<Piece> GetCurrentHand() => hand ?? throw new DominoException("Game not started");
    public Move GetMove()
    {
        (Piece leftPiece, Piece rightPiece) =
            (
                GameReference.GetPieceOnExtreme(false),
                GameReference.GetPieceOnExtreme(true)
            );

        //I only need if it can be played, not need for what side, it will checked after
        var possiblePieces = from piece in hand
                             where leftPiece.CanMatch(piece, false) || rightPiece.CanMatch(piece, true)
                             select piece;

        var smartPieces = possiblePieces.ToList();

        int[][] pieceValues = new int[possiblePieces.Count()][];
        for (int i = 0; i < possiblePieces.Count(); i++)
        {
            pieceValues[i] = new int[2];
        }

        //------------------------
        //Assigning values
        //(Alfredo: I will do it)
        //The highest value in a piece will be the prefered side to join with the piece on board
        //Finish assigning
        //------------------------


        List<int> sortedValues = new();
        for (int i = 0; i < possiblePieces.Count(); i++)
        {
            sortedValues.AddRange(pieceValues[i]);
        }

        //Any way to make this prettier?
        sortedValues.Sort();
        sortedValues.Reverse();
        //------------------------

        Piece pieceToPlay = new();
        bool placedOnRight = default;

        //Check the side to put the piece with the highest side value
        for (int i = 0; i < sortedValues.Count; i++)
        {
            for (int j = 0; j < pieceValues.Length; j++)
            {
                if (sortedValues[i] == pieceValues[j][0])
                {
                    if (smartPieces[j][0] == leftPiece[0])
                    {
                        pieceToPlay = smartPieces[j];
                        placedOnRight = false;
                        break;
                    }
                    if (smartPieces[j][0] == rightPiece[1])
                    {
                        pieceToPlay = smartPieces[j];
                        placedOnRight = true;
                        break;
                    }
                }
                if (sortedValues[i] == pieceValues[j][1])
                {
                    if (smartPieces[j][1] == leftPiece[0])
                    {
                        pieceToPlay = smartPieces[j];
                        placedOnRight = false;
                        break;
                    }
                    if (smartPieces[j][1] == rightPiece[1])
                    {
                        pieceToPlay = smartPieces[j];
                        placedOnRight = true;
                        break;
                    }
                }
            }
        }
        return new Move(PlayerID, pieceToPlay, false, placedOnRight);
    }
}