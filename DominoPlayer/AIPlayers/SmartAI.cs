using System.Collections.Generic;
using System.Linq;

// ReSharper disable CompareOfFloatsByEqualityOperator

// ReSharper disable CheckNamespace

// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable ParameterHidesMember

namespace DominoPlayer.AI
{
    public class SmartAI : AIPlayer
    {
        //This can't be readonly, it need to be changed in the middle of the game
        private const double TeamMissing = 0.15;
        private const double OpponentMissing = 0.25;
        private const double SameNumber = 0.20;
        private const double OpponentPlaced = 0.20;
        private const double Double = 1.0;

        public SmartAI(int playerID, DominoGame game) :
            base(playerID, game)
        {
            game.OnMoveMade += WatchMoves;
        }

        private void WatchMoves(Move move)
        {
            try
            {
                if (move.playerID == PlayerID)
                    return;
                if (move.passed)
                {
                    if (valuesPartnerNotHave.ContainsKey(move.playerID))
                    {
                        if (move.placedOnRight)
                            valuesPartnerNotHave[move.playerID].Add(GameReference.GetPieceOnExtreme(true).Right);
                        else
                            valuesPartnerNotHave[move.playerID].Add(GameReference.GetPieceOnExtreme(false).Left);
                    }
                    else
                    {
                        if (move.placedOnRight)
                            valuesOpponentNothave[move.playerID].Add(GameReference.GetPieceOnExtreme(true).Right);
                        else
                            valuesOpponentNothave[move.playerID].Add(GameReference.GetPieceOnExtreme(false).Left);
                    }
                }
            }
            catch (KeyNotFoundException)
            {
                throw new DominoException("Not all players are in this player database");
            }
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
            if (!possiblePieces.Any())
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
            List<int> valuesPartner = new List<int>();
            foreach (var key in valuesPartnerNotHave.Keys)
            {
                for (int i = 0; i < valuesPartnerNotHave[key].Count; i++)
                {
                    if (!valuesPartner.Contains(valuesPartnerNotHave[key][i]))
                        valuesPartner.Add(valuesPartnerNotHave[key][i]);
                }
            }

            List<int> valuesOpponent = new List<int>();
            foreach (var key in valuesOpponentNothave.Keys)
            {
                for (int i = 0; i < valuesOpponentNothave[key].Count; i++)
                {
                    if (!valuesOpponent.Contains(valuesOpponentNothave[key][i]))
                        valuesOpponent.Add(valuesOpponentNothave[key][i]);
                }
            }

            PointsForTeamMate(smartPieces, valuesPartner, pieceValues);
            PointsForOpponnent(smartPieces, valuesOpponent, pieceValues);
            PointForDouble(smartPieces, pieceValues);
            PointForSameNumber(smartPieces, pieceValues);
            PointForPlaced(smartPieces, pieceValues);
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
                    if (sortedValues[i] == pieceValues[j][0] || sortedValues[i] == pieceValues[j][1])
                    {
                        bool reversed = false;
                        if (leftPiece.CanMatch(smartPieces[j], false, out reversed) || leftPiece.Left == -1)
                        {
                            pieceToPlay = smartPieces[j];
                            if (reversed)
                                pieceToPlay.Reverse();
                            placedOnRight = false;
                            break;
                        }

                        if (rightPiece.CanMatch(smartPieces[j], true, out reversed))
                        {
                            pieceToPlay = smartPieces[j];
                            if (reversed)
                                pieceToPlay.Reverse();
                            placedOnRight = true;
                            break;
                        }
                    }
                }
            }

            if (pieceToPlay.Left == -1)
                throw new DominoException("No piece could be found to play");
            return Move.CreateMove(PlayerID, pieceToPlay, placedOnRight);
        }

        void PointsForTeamMate(List<Piece> hand, List<int> valuesPartnerNotHave, double[][] pieceValues)
        {
            for (int i = 0; i < valuesPartnerNotHave.Count; i++)
            {
                for (int k = 0; k < hand.Count; k++)
                {
                    Piece p = new Piece(valuesPartnerNotHave[i], valuesPartnerNotHave[i], GameReference.gameRules);
                    bool reversed = false;
                    if (p.CanMatch(hand[k], true, out reversed))
                    {
                        if (reversed)
                        {
                            pieceValues[k][1] += TeamMissing;
                        }
                        else
                            pieceValues[k][0] += TeamMissing;
                    }
                }
            }
        }

        void PointsForOpponnent(List<Piece> hand, List<int> valuesOpponentNothave, double[][] pieceValues)
        {
            for (int i = 0; i < valuesOpponentNothave.Count; i++)
            {
                for (int k = 0; k < hand.Count; k++)
                {
                    Piece p = new Piece(valuesOpponentNothave[i], valuesOpponentNothave[i], GameReference.gameRules);

                    bool reversed = false;
                    if (p.CanMatch(hand[k], true, out reversed))
                    {
                        if (reversed)
                        {
                            pieceValues[k][0] += OpponentMissing;
                        }
                        else
                            pieceValues[k][1] += OpponentMissing;
                    }
                }
            }
        }

        void PointForSameNumber(List<Piece> hand, double[][] pieceValues)
        {
            Dictionary<double, int> dict = new Dictionary<double, int>();
            foreach (var piece in hand)
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
            for (int i = 0; i < hand.Count; i++)
            {
                int index = keys.IndexOf(hand[i].Left);
                pieceValues[i][1] += dict[keys[index]] * SameNumber;
                index = keys.IndexOf(hand[i].Right);
                pieceValues[i][0] += dict[keys[index]] * SameNumber;
            }
        }

        void PointForDouble(List<Piece> hand, double[][] pieceValues)
        {
            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i].Left == hand[i].Right)
                {
                    pieceValues[i][0] += Double;
                    pieceValues[i][1] += Double;
                }
            }
        }

        void PointForPlaced(List<Piece> hand, double[][] pieceValues)
        {
            if (GameReference.history.Count == 0)
                return;
            for (int i = 0; i < hand.Count; i++)
            {
                if (GetOpponents().Contains(GameReference.history[GameReference.leftExtreme].playerID))
                {
                    if (GameReference.history[GameReference.leftExtreme].piecePlaced
                        .CanMatch(hand[i], false, out var reversed))
                    {
                        if (reversed)
                            pieceValues[i][0] += OpponentPlaced;
                        else
                            pieceValues[i][1] += OpponentPlaced;
                    }
                }

                if (GetOpponents().Contains(GameReference.history[GameReference.rightExtreme].playerID))
                {
                    if (GameReference.history[GameReference.rightExtreme].piecePlaced.CanMatch(hand[i],true, out var reversed))
                    {
                        if (reversed)
                            pieceValues[i][1] += OpponentPlaced;
                        else
                            pieceValues[i][0] += OpponentPlaced;
                    }
                }
            }
        }
    }
}