using System;
using System.Collections.Generic;
using System.Linq;


namespace DominoPlayer.AI
{
    public class SmartAI : AIPlayer
    {
        //This can't be readonly, it need to be changed in the middle of the game
        private const double TEAM_MISSING = 0.15;
        private const double OPPONENT_MISSING = 0.25;
        private const double SAME_NUMBER = 0.20;
        private const double OPPONENT_PLACED = 0.20;
        private const double DOUBLE = 1.0;

        public SmartAI(int playerID, DominoGame game) :
        base(playerID, game)
        {
            game.OnMoveMade += WatchMoves;
        }
        private void WatchMoves(Move move)
        {
            //try
            //{
            if (move.playerID == PlayerID)
                return;
            if (move.passed)
            {
                if (valuesPartnerNotHave.ContainsKey(move.playerID))
                {
                    if (move.placedOnRight)
                        valuesPartnerNotHave[move.playerID].Add(GameReference.history[GameReference.history.Count - 2].piecePlaced.Right);
                    else
                        valuesPartnerNotHave[move.playerID].Add(GameReference.history[GameReference.history.Count - 2].piecePlaced.Left);
                }
                else
                {
                    if (move.placedOnRight)
                        valuesOpponentNothave[move.playerID].Add(GameReference.history[GameReference.history.Count - 2].piecePlaced.Right);
                    else
                        valuesOpponentNothave[move.playerID].Add(GameReference.history[GameReference.history.Count - 2].piecePlaced.Left);
                }
            }
            //}
            //catch (KeyNotFoundException e)
            //{
            //  var temp = e;
            //throw new DominoException("Not all players are in this player database");
            //}
        }
        protected override Move InternalGetMove()
        {
            (Piece leftPiece, Piece rightPiece) =
                (
                    GameReference.GetPieceOnExtreme(false),
                    GameReference.GetPieceOnExtreme(true)
                );
            //I only need if it can be played, not need for what side, it will be checked after
            var possiblePieces = GameReference.GetPlayablePieces(Hand).Select(p => p.piece);
            if (possiblePieces == null || !possiblePieces.Any())
                return Move.CreatePass(PlayerID);

            var smartPieces = possiblePieces.ToList();

            double[][] pieceValues = new double[possiblePieces.Count()][];
            for (int i = 0; i < possiblePieces.Count(); i++)
            {
                pieceValues[i] = new double[2];
            }
            //------------------------
            //Assigning values
            //The highest value in a piece will be the prefered side to join with the piece on board
            List<double> valuesPartner = new List<double>();
            foreach (var key in valuesPartnerNotHave.Keys)
            {
                for (int i = 0; i < valuesPartnerNotHave[key].Count; i++)
                {
                    if (!valuesPartner.Contains(valuesPartnerNotHave[key][i]))
                        valuesPartner.Add(valuesPartnerNotHave[key][i]);
                }
            }
            List<double> valuesOpponent = new List<double>();
            foreach (var key in valuesOpponentNothave.Keys)
            {
                for (int i = 0; i < valuesOpponentNothave[key].Count; i++)
                {
                    if (!valuesPartner.Contains(valuesOpponentNothave[key][i]))
                        valuesPartner.Add(valuesOpponentNothave[key][i]);
                }
            }
            PointsForTeamMate(smartPieces, valuesPartner, pieceValues);
            PointsForOpponnent(smartPieces, valuesOpponent, pieceValues);
            PointForDouble(smartPieces, pieceValues);
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

            Piece pieceToPlay = new Piece(-1, -1, GameReference.gameRules);

            bool placedOnRight = default;

            //Check the side to put the piece with the highest side value
            for (int i = 0; i < sortedValues.Count; i++)
            {
                if (pieceToPlay.Left != -1)
                    break;
                for (int j = 0; j < pieceValues.Length; j++)
                {
                    if (sortedValues[i] == pieceValues[j][0])
                    {

                        if (smartPieces[j].Left == leftPiece.Left || leftPiece.Left == -1)
                        {
                            pieceToPlay = smartPieces[j];
                            pieceToPlay.Reverse();
                            placedOnRight = false;
                            break;
                        }
                        if (smartPieces[j].Left == rightPiece.Right)
                        {
                            pieceToPlay = smartPieces[j];
                            placedOnRight = true;
                            break;
                        }
                    }
                    if (sortedValues[i] == pieceValues[j][1])
                    {
                        if (smartPieces[j].Right == leftPiece.Left || leftPiece.Left == -1)
                        {
                            pieceToPlay = smartPieces[j];
                            placedOnRight = false;
                            break;
                        }
                        if (smartPieces[j].Right == rightPiece.Right)
                        {
                            pieceToPlay = smartPieces[j];
                            pieceToPlay.Reverse();
                            placedOnRight = true;
                            break;
                        }
                    }
                }
            }
            return Move.CreateMove(PlayerID, pieceToPlay, placedOnRight);
        }

        void PointsForTeamMate(List<Piece> Hand, List<double> valuesPartnerNotHave, double[][] pieceValues)
        {
            for (int i = 0; i < valuesPartnerNotHave.Count; i++)
            {
                for (int k = 0; k < Hand.Count; k++)
                {
                    if (Hand[k].Left == valuesPartnerNotHave[i])
                        pieceValues[k][0] += TEAM_MISSING;
                    if (Hand[k].Right == valuesPartnerNotHave[i])
                        pieceValues[k][1] += TEAM_MISSING;
                }
            }
        }
        void PointsForOpponnent(List<Piece> Hand, List<double> valuesOpponentNothave, double[][] pieceValues)
        {
            for (int i = 0; i < valuesOpponentNothave.Count; i++)
            {
                for (int k = 0; k < Hand.Count; k++)
                {
                    if (Hand[k].Left == valuesOpponentNothave[i])
                        pieceValues[k][1] += OPPONENT_MISSING;
                    if (Hand[k].Right == valuesOpponentNothave[i])
                        pieceValues[k][0] += OPPONENT_MISSING;
                }
            }
        }
        void PointForSameNumber(List<Piece> Hand, double[][] pieceValues)
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
            for (int i = 0; i < Hand.Count; i++)
            {
                int index = keys.IndexOf(Hand[i].Left);
                pieceValues[i][1] += dict[keys[index]] * SAME_NUMBER;
                index = keys.IndexOf(Hand[i].Right);
                pieceValues[i][0] += dict[keys[index]] * SAME_NUMBER;
            }
        }
        void PointForDouble(List<Piece> Hand, double[][] pieceValues)
        {
            for (int i = 0; i < Hand.Count; i++)
            {
                if (Hand[i].Left == Hand[i].Right)
                {
                    pieceValues[i][0] += DOUBLE;
                    pieceValues[i][1] += DOUBLE;
                }
            }
        }
        void PointForPlaced(List<Piece> Hand, double[][] pieceValues)
        {
            for (int i = 0; i < Hand.Count; i++)
            {
                if (Hand[i].Left == GameReference.history[GameReference.leftExtreme].piecePlaced.Left || Hand[i].Left == GameReference.history[GameReference.rightExtreme].piecePlaced.Right)
                    pieceValues[i][0] += OPPONENT_PLACED;
                if (Hand[i].Right == GameReference.history[GameReference.leftExtreme].piecePlaced.Left || Hand[i].Right == GameReference.history[GameReference.rightExtreme].piecePlaced.Right)
                    pieceValues[i][1] += OPPONENT_PLACED;
            }
        }
    }
}