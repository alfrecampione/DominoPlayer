using System.Collections.Generic;
using System.Linq;
using System;

namespace DominoPlayer
{
    public sealed class DominoGame
    {
        public int PiecesInGame => history.Count;
        public int CurrentPlayer { get; private set; }

        public int leftExtreme;
        public int rightExtreme;

        public List<DominoPlayer> Players => players;

        private readonly List<DominoPlayer> players;
        private readonly List<Piece> undistributedPieces;

        internal readonly IRules gameRules;

        public List<Move> history;
        public event Action<Move>? OnMoveMade;

        public DominoGame(IRules rules)
        {
            leftExtreme = 0;
            rightExtreme = 0;
            gameRules = rules;
            history = new List<Move>();
            undistributedPieces = new List<Piece>();
            players = new List<DominoPlayer>(rules.MaxPlayers);

            for (int i = 0; i <= rules.MaxPieceValue; i++)
                for (int e = i; e <= rules.MaxPieceValue; e++)
                    undistributedPieces.Add(new Piece(i, e, rules));
        }
        private List<Piece> CreateHand()
        {
            Random random = new Random();
            List<Piece> hand = new List<Piece>();

            if (gameRules.PiecesPerHand > undistributedPieces.Count)
                throw new IndexOutOfRangeException("Can't create hand. Insufficient pieces.");

            for (int i = 0; i < gameRules.PiecesPerHand; i++)
            {
                int index = random.Next(undistributedPieces.Count);
                hand.Add(undistributedPieces[index]);
                undistributedPieces.RemoveAt(index);
            }
            return hand;
        }
        public void StartGame(params DominoPlayer[] players)
        {
            if (players.Length > gameRules.MaxPlayers || players.Length < gameRules.MinPlayers)
                throw new DominoException($"Tried to start game with {players.Length} players, this is not allowed by current rules.");

            this.players.Clear();
            this.players.AddRange(players);

            foreach (var player in this.players)
                player.StartGame(CreateHand());

            CurrentPlayer = new Random().Next(this.players.Count);
        }

        public bool IsGameOver(out int winner)
            => gameRules.GameOverCondition(this, out winner);


        public IEnumerable<(Piece piece, bool canMatchLeft, bool canMatchRight, bool reverseLeft, bool reverseRight)> GetPlayablePieces(IEnumerable<Piece> hand)
        {
            (Piece leftPiece, Piece rightPiece) =
            (
                GetPieceOnExtreme(false),
                GetPieceOnExtreme(true)
            );
            if (leftPiece.Left == -1)
            {
                foreach (Piece p in hand)
                    yield return (p, true, true, false, false);
            }
            else
            {
                foreach (Piece p in hand)
                {
                    bool matchLeft = leftPiece.CanMatch(p, false, out bool reverseLeft);
                    bool matchRight = rightPiece.CanMatch(p, true, out bool reverseRight);

                    if (matchLeft || matchRight)
                        yield return (p, matchLeft, matchRight, reverseLeft, reverseRight);
                }
            }
        }
        public void MakeMove(Move move)
        {
            history.Add(move);
            if (!move.passed)
            {
                if (move.placedOnRight)
                    rightExtreme = history.Count - 1;
                else
                    leftExtreme = history.Count - 1;
            }
            OnMoveMade?.Invoke(move);
        }
        public void NextTurn()
        {
            if (players is null)
                throw new DominoException("Tried to call NextTurn but game has not started yet.");

            MakeMove(players[CurrentPlayer].GetMove());
            CurrentPlayer++;

            if (CurrentPlayer >= players.Count)
                CurrentPlayer = 0;
        }
        public Piece GetPieceOnExtreme(bool right)
        {
            if (history.Count == 0)
                return new Piece(-1, -1, gameRules);
            return (right) ? history[rightExtreme].piecePlaced : history[leftExtreme].piecePlaced;
        }
    }
}