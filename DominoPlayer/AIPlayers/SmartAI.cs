using System.Collections.Generic;
using System.Linq;

namespace DominoPlayer.AI
{
    public class SmartAI : DominoPlayer
    {
        private readonly List<List<double>> valuesPartnerNotHave;
        private readonly List<List<double>> valuesOpponentNothave;
        private const double TEAM_MISSING = 0.15;
        private const double OPPONENT_MISSING = 0.25;
        private const double SAME_NUMBER = 0.50;

        public SmartAI(int playerID, DominoGame game, List<List<double>> valuesPartnerNotHave, List<List<double>> valuesOpponentNothave) :
        base(playerID, game)
        {
            this.valuesPartnerNotHave = valuesPartnerNotHave;
            this.valuesOpponentNothave = valuesOpponentNothave;
        }

        public override Move GetMove()
        {
            (Piece leftPiece, Piece rightPiece) =
                (
                    GameReference.GetPieceOnExtreme(false),
                    GameReference.GetPieceOnExtreme(true)
                );

            //I only need if it can be played, not need for what side, it will checked after
            var possiblePieces = GameReference.GetPlayablePieces(Hand).Select(p => p.piece);

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


            List<double> sortedValues = new List<double>();
            for (int i = 0; i < possiblePieces.Count(); i++)
            {
                sortedValues.AddRange(pieceValues[i]);
            }
            //Any way to make this prettier?
            sortedValues.Sort();
            sortedValues.Reverse();
            //------------------------

            Piece pieceToPlay = new Piece();

            bool placedOnRight = default;

            //Check the side to put the piece with the highest side value
            for (int i = 0; i < sortedValues.Count; i++)
            {
                for (int j = 0; j < pieceValues.Length; j++)
                {
                    if (sortedValues[i] == pieceValues[j][0])
                    {
                        if (smartPieces[j].Left == leftPiece.Left)
                        {
                            pieceToPlay = smartPieces[j];
                            placedOnRight = false;
                            break;
                        }
                        if (smartPieces[j].Left == leftPiece.Right)
                        {
                            pieceToPlay = smartPieces[j];
                            placedOnRight = true;
                            break;
                        }
                    }
                    if (sortedValues[i] == pieceValues[j][1])
                    {
                        if (smartPieces[j].Right == leftPiece.Left)
                        {
                            pieceToPlay = smartPieces[j];
                            placedOnRight = false;
                            break;
                        }
                        if (smartPieces[j].Right == leftPiece.Right)
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

        static void PointsForTeamMate(List<Piece> Hand, List<List<double>> valuesPartnerNotHave, double[][] pieceValues)
        {
            for (int i = 0; i < valuesPartnerNotHave.Count; i++)
            {
                for (int j = 0; j < valuesPartnerNotHave[i].Count; j++)
                {
                    for (int k = 0; k < Hand.Count; k++)
                    {
                        if (Hand[k].Left == valuesPartnerNotHave[i][j])
                            pieceValues[0][k] += TEAM_MISSING;
                        if (Hand[k].Right == valuesPartnerNotHave[i][j])
                            pieceValues[1][k] += TEAM_MISSING;
                    }
                }
            }
        }
        static void PointsForOponnent(List<Piece> Hand, List<List<double>> valuesOpponentNothave, double[][] pieceValues)
        {
            for (int i = 0; i < valuesOpponentNothave.Count; i++)
            {
                for (int j = 0; j < valuesOpponentNothave[i].Count; j++)
                {
                    for (int k = 0; k < Hand.Count; k++)
                    {
                        if (Hand[k].Left == valuesOpponentNothave[i][j])
                            pieceValues[1][k] += OPPONENT_MISSING;
                        if (Hand[k].Right == valuesOpponentNothave[i][j])
                            pieceValues[0][k] += OPPONENT_MISSING;
                    }
                }
            }
        }
        static void PointForSameNumber(List<Piece> Hand, double[][] pieceValues)
        {
            Dictionary<double, int> dict = new Dictionary<double, int>();
            foreach (var piece in Hand)
            {
                try
                {
                    dict[piece.Left] += 1;
                }
                catch
                {
                    dict.Add(piece.Left, 1);
                }
                try
                {
                    dict[piece.Right] += 1;
                }
                catch
                {
                    dict.Add(piece.Right, 1);
                }
            }
            var keys = dict.Keys.ToList();
            for (int i = 0; i < pieceValues.GetLength(1); i++)
            {
                int up = 1;
                int index = keys.IndexOf(Hand[i].Left);
                if (index == -1)
                {
                    index = keys.IndexOf(Hand[i].Right);
                    up = 0;
                }
                pieceValues[index][up] += dict[keys[i]] * SAME_NUMBER;
            }
        }
    }
}