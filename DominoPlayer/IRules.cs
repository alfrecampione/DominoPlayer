namespace DominoPlayer;

public interface IRules
{
    public int PiecesPerHand { get; }
    public int MaxPieceValue { get; }
    public int MaxPlayers { get; }
    public int MinPlayers { get; }

    /// <summary>
    /// Returns true if a game is over, also returning the winner of the game.
    /// </summary>
    public bool GameOverCondition(DominoGame game, out int winner);

    /// <summary>
    /// Returns true if two pieces can be matched exactly as they stand, without reversing;
    /// returns false otherwise.
    /// </summary>
    /// <param name="leftToRight">True if pieces are to be compared left to right,
    /// as A / B, false otherwise.</param>
    public bool CanPiecesMatch(Piece a, Piece b, bool leftToRight);
}