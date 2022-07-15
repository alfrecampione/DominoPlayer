using System.Collections.Generic;
using System.Linq;
using DominoPlayer.AI;

// ReSharper disable CompareOfFloatsByEqualityOperator

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace DominoPlayer
{
    public class Observer
    {
        private int _id;
        private DominoGame _game;
        private Dictionary<int, List<int>> _valuesPartnerNotHave = new Dictionary<int, List<int>>();
        private Dictionary<int, List<int>> _valuesOpponentNothave = new Dictionary<int, List<int>>();

        private List<int> SetOpponents() => _game.Players[_id].GetOpponents();
        private List<int> SetPartners() => _game.Players[_id].GetPartners();

        public Observer(int id, DominoGame dominoGame)
        {
            _id = id;
            _game = dominoGame;
            foreach (var opponent in SetOpponents())
            {
                _valuesOpponentNothave.Add(opponent, new List<int>());
            }

            foreach (var partner in SetPartners())
            {
                _valuesPartnerNotHave.Add(partner, new List<int>());
            }

            dominoGame.OnMoveMade += WatchMoves;
        }

        private void WatchMoves(Move move)
        {
            try
            {
                if (move.playerID == _id)
                    return;
                if (move.passed)
                {
                    if (_valuesPartnerNotHave.ContainsKey(move.playerID))
                    {
                        if (move.placedOnRight)
                            _valuesPartnerNotHave[move.playerID].Add(_game.GetPieceOnExtreme(true).Right);
                        else
                            _valuesPartnerNotHave[move.playerID].Add(_game.GetPieceOnExtreme(false).Left);
                    }
                    else
                    {
                        if (move.placedOnRight)
                            _valuesOpponentNothave[move.playerID].Add(_game.GetPieceOnExtreme(true).Right);
                        else
                            _valuesOpponentNothave[move.playerID].Add(_game.GetPieceOnExtreme(false).Left);
                    }
                }
            }
            catch (KeyNotFoundException)
            {
                throw new DominoException("Not all players are in this player database");
            }
        }

        public double GetValue(List<Piece> hand, Piece selectedPiece)
        {
            var pieces = GetScores(hand);
            if (!pieces.Any())
                return -1;
            return 1 - (pieces.IndexOf(selectedPiece) / pieces.Count);
        }

        private List<Piece> GetScores(List<Piece> hand)
        {
            (Piece leftPiece, Piece rightPiece) =
            (
                _game.GetPieceOnExtreme(false),
                _game.GetPieceOnExtreme(true)
            );
            //I only need if it can be played, not need for what side, it will be checked after
            var possiblePieces = _game.GetPlayablePieces(hand).Select(p => p.piece);
            var enumerable = possiblePieces.ToList();
            if (!enumerable.Any())
                return new List<Piece>();

            var smartPieces = enumerable.ToList();

            double[][] pieceValues = new double[enumerable.Count()][];
            for (int i = 0; i < enumerable.Count(); i++)
            {
                pieceValues[i] = new double[2];
            }

            //------------------------
            //Assigning values
            //The highest value in a piece will be the prefered side to join with the piece on board
            List<int> valuesPartner = new List<int>();
            foreach (var key in _valuesPartnerNotHave.Keys)
            {
                for (int i = 0; i < _valuesPartnerNotHave[key].Count; i++)
                {
                    if (!valuesPartner.Contains(_valuesPartnerNotHave[key][i]))
                        valuesPartner.Add(_valuesPartnerNotHave[key][i]);
                }
            }

            List<int> valuesOpponent = new List<int>();
            foreach (var key in _valuesOpponentNothave.Keys)
            {
                for (int i = 0; i < _valuesOpponentNothave[key].Count; i++)
                {
                    if (!valuesOpponent.Contains(_valuesOpponentNothave[key][i]))
                        valuesOpponent.Add(_valuesOpponentNothave[key][i]);
                }
            }

            SmartAI.PointsForTeamMate(smartPieces, valuesPartner, pieceValues, _game);
            SmartAI.PointsForOpponnent(smartPieces, valuesOpponent, pieceValues, _game);
            SmartAI.PointForDouble(smartPieces, pieceValues);
            SmartAI.PointForSameNumber(smartPieces, pieceValues);
            SmartAI.PointForPlaced(smartPieces, pieceValues, _game, _valuesOpponentNothave.Keys.ToList());
            //Finish assigning
            //------------------------


            List<double> sortedValues = new List<double>();
            for (int i = 0; i < enumerable.Count(); i++)
            {
                sortedValues.AddRange(pieceValues[i]);
            }

            //Any way to make this prettier?
            sortedValues.Sort();
            sortedValues.Reverse();
            //------------------------

            Piece pieceToPlay;

            List<Piece> sortedPieces = new List<Piece>();
            //Check the side to put the piece with the highest side value
            foreach (var t in sortedValues)
            {
                for (int j = 0; j < pieceValues.Length; j++)
                {
                    if (t != pieceValues[j][0] && t != pieceValues[j][1]) continue;
                    if (leftPiece.CanMatch(smartPieces[j], false, out var reversed) || leftPiece.Left == -1)
                    {
                        pieceToPlay = smartPieces[j];
                        if (reversed)
                            pieceToPlay.Reverse();
                        if (!sortedPieces.Any(x => (x == pieceToPlay)))
                            sortedPieces.Add(pieceToPlay);
                    }

                    if (!rightPiece.CanMatch(smartPieces[j], true, out reversed)) continue;
                    pieceToPlay = smartPieces[j];
                    if (reversed)
                        pieceToPlay.Reverse();
                    if (!sortedPieces.Any(x => (x == pieceToPlay)))
                        sortedPieces.Add(pieceToPlay);
                }
            }

            return sortedPieces;
        }
    }
}