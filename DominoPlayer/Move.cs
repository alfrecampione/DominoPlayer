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
}