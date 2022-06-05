namespace DominoPlayer;

public struct Move
{
    public int playerID;
    public bool passed;
    public Piece piecePlaced;
    public bool placedOnRight;

    public Move(int playerID, Piece piece, bool passed, bool placedOnRight)
    {
        this.playerID = playerID;
        this.piecePlaced = piece;
        this.passed = passed;
        this.placedOnRight = placedOnRight;
    }

    public static Move CreateMove(int playerID, Piece piece, bool right) => new(playerID, piece, false, right);
    public static Move CreatePass(int playerID) => new(playerID, default, true, default);
}