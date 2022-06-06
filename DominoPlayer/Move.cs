namespace DominoPlayer;

public struct Move
{
    public int playerID;
    public bool passed;
    public Piece piecePlaced;
    public bool placedOnRight;

    public Move(int playerID, Piece piece, bool passed, bool placedOnRight)
    => (this.playerID, this.piecePlaced, this.passed, this.placedOnRight)
        = (playerID, piece, passed, placedOnRight);

    public static Move CreateMove(int playerID, Piece piece, bool right) => new(playerID, piece, false, right);
    public static Move CreatePass(int playerID) => new(playerID, default, true, default);
}