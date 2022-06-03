using System.Linq;
using System.Collections.Generic;
using System;

namespace DominoPlayer;

public class DominoGame
{
    public int PiecesInGame
    {
        get { return gamePieces.Count; }
    }
    public Piece this[int i]
    {
        get { return gamePieces[i]; }
    }

    DominoPlayer[]? players;
    readonly List<Piece> gamePieces;
    readonly List<Piece> undistributedPieces;
    readonly int piecesPerHand;
    public int CurrentPlayer { get; private set; }

    public event Action<Move>? OnMoveMade;

    public DominoGame(int piecesPerHand, int maxValue)
    {
        this.undistributedPieces = new List<Piece>();
        this.gamePieces = new List<Piece>();
        this.piecesPerHand = piecesPerHand;

        CreateAllPieces(maxValue);
    }
    void CreateAllPieces(int maxValue)
    {
        for (int i = 0; i <= maxValue; i++)
            for (int e = i; e <= maxValue; e++)
                this.undistributedPieces.Add(new Piece(i, e));
    }
    public void StartGame(DominoPlayer[] players)
    {
        this.players = players;
        foreach (var player in this.players)
            player.StartGame(CreateHand());

        CurrentPlayer = new Random().Next(players.Length);
    }
    public bool IsGameOver(out int winner)
    {
        //TODO: Many things.
        winner = 0;
        return false;
    }

    public IEnumerable<(Piece piece, bool right)> GetPlayablePieces(IEnumerable<Piece> hand)
    {
        (Piece leftPiece, Piece rightPiece) =
            (
                GetPieceOnExtreme(false),
                GetPieceOnExtreme(true)
            );
        return from piece in hand
               where leftPiece.CanMatch(piece, false) || rightPiece.CanMatch(piece, true)
               select (piece, rightPiece.CanMatch(piece, true));
    }
    public void MakeMove(Move move)
    {
        if (!move.passed)
            if (move.placedOnRight)
                gamePieces.Add(move.piecePlaced);
            else
                gamePieces.Insert(0, move.piecePlaced);

        OnMoveMade?.Invoke(move);
    }
    public void NextTurn()
    {
        if (players is null)
            throw new DominoException("Game not started");

        MakeMove(players[CurrentPlayer].GetMove());
        CurrentPlayer++;

        if (CurrentPlayer >= players.Length)
            CurrentPlayer = 0;
    }
    List<Piece> CreateHand()
    {
        Random random = new();
        List<Piece> hand = new();

        if (piecesPerHand > undistributedPieces.Count)
            throw new IndexOutOfRangeException("Can't create hand. Insufficient pieces.");

        for (int i = 0; i < piecesPerHand; i++)
        {
            int index = random.Next(undistributedPieces.Count);
            hand.Add(undistributedPieces[index]);
            undistributedPieces.RemoveAt(index);
        }
        return hand;
    }

    public Piece GetPieceOnExtreme(bool right)
    {
        return gamePieces[right ? ^1 : 0];
    }
}
