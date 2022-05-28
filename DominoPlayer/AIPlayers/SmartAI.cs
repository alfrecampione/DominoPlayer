namespace DominoPlayer.AI;

public class SmartAI : IDominoPlayer
{
    public int PlayerID { get; set; }
    private List<Piece>? hand;
    public DominoGame GameReference { get; set; }
    List<List<double>> valuesPartnerNotHave;
    List<List<double>> valuesOpponentNothave;
    const double teamMissing = 0.15;
    const double opponentMissing = 0.25;
    const double sameNumber = 0.50;

    public SmartAI(int playerID, DominoGame game, List<List<double>> valuesPartnerNotHave, List<List<double>> valuesOpponentNothave)
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

        double[][] pieceValues = new double[possiblePieces.Count()][];
        for (int i = 0; i < possiblePieces.Count(); i++)
        {
            pieceValues[i] = new double[2];
        }

        //------------------------
        //Assigning values
        //(Alfredo: I will do it)
        //The highest value in a piece will be the prefered side to join with the piece on board
        PointsForTeamMate(smartPieces, valuesPartnerNotHave, pieceValues);
        PointsForOponnent(smartPieces, valuesOpponentNothave, pieceValues);
        PointForSameNumber(smartPieces, pieceValues);
        //Finish assigning
        //------------------------


        List<double> sortedValues = new();
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

    static void PointsForTeamMate(List<Piece> hand, List<List<double>> valuesPartnerNotHave, double[][] pieceValues)
    {
        for (int i = 0; i < valuesPartnerNotHave.Count; i++)
        {
            for (int j = 0; j < valuesPartnerNotHave[i].Count; j++)
            {
                for (int k = 0; k < hand.Count; k++)
                {
                    if (hand[k][0] == valuesPartnerNotHave[i][j])
                        pieceValues[0][k] += teamMissing;
                    if (hand[k][1] == valuesPartnerNotHave[i][j])
                        pieceValues[1][k] += teamMissing;
                }
            }
        }
    }
    static void PointsForOponnent(List<Piece> hand, List<List<double>> valuesOpponentNothave, double[][] pieceValues)
    {
        for (int i = 0; i < valuesOpponentNothave.Count; i++)
        {
            for (int j = 0; j < valuesOpponentNothave[i].Count; j++)
            {
                for (int k = 0; k < hand.Count; k++)
                {
                    if (hand[k][0] == valuesOpponentNothave[i][j])
                        pieceValues[1][k] += opponentMissing;
                    if (hand[k][1] == valuesOpponentNothave[i][j])
                        pieceValues[0][k] += opponentMissing;
                }
            }
        }
    }
    static void PointForSameNumber(List<Piece> hand, double[][] pieceValues)
    {
        Dictionary<double, int> dict = new();
        foreach (var piece in hand)
        {
            try
            {
                dict[piece[0]] += 1;
            }
            catch
            {
                dict.Add(piece[0], 1);
            }
            try
            {
                dict[piece[1]] += 1;
            }
            catch
            {
                dict.Add(piece[1], 1);
            }
        }
        var keys = dict.Keys.ToList();
        for (int i = 0; i < pieceValues.GetLength(1); i++)
        {
            int up = 1;
            int index = keys.IndexOf(hand[i][0]);
            if (index == -1)
            {
                index = keys.IndexOf(hand[i][1]);
                up = 0;
            }
            pieceValues[index][up] += dict[keys[i]] * sameNumber;
        }
    }
}