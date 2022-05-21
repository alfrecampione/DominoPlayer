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

    readonly Player[] players;
    readonly List<Piece> gamePieces;
    readonly List<Piece> undistributedPieces;
    readonly int piecesPerHand;
    public DominoGame(int numberOfPlayers, int piecesPerHand, int maxValue)
    {
        this.players = new Player[numberOfPlayers];
        this.undistributedPieces = new List<Piece>();
        this.gamePieces = new List<Piece>();
        this.piecesPerHand = piecesPerHand;

        CreateAllPieces(maxValue);

        if (numberOfPlayers * piecesPerHand > undistributedPieces.Count)
            throw new ArgumentOutOfRangeException(nameof(numberOfPlayers), "Too many players.");
    }
    void CreateAllPieces(int maxValue)
    {
        for (int i = 0; i <= maxValue; i++)
            for (int e = i; e <= maxValue; e++)
                this.undistributedPieces.Add(new Piece(i, e));
    }

    public List<Piece> CreateHand()
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
}
