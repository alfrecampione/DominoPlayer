namespace DominoPlayer.AI;

public class BotaGordaAI : DominoPlayer
{
    public int PlayerID { get; set; }
    public List<Piece>? hand;
    public DominoGame GameReference { get; set; }

    public BotaGordaAI(int playerID, DominoGame game)
    {
        this.PlayerID = playerID;
        this.GameReference = game;
    }

    public void StartGame(List<Piece> startingHand)
    {
        hand = new(startingHand);
    }
    public List<Piece> GetCurrentHand() => hand ?? throw new DominoException("Game not started");
    public Move GetMove()
    {
        var possiblePieces = GameReference.GetPlayablePieces(hand ?? throw new DominoException("Game not started"));

        (Piece piece, bool right) gordastPiece = possiblePieces.MaxBy(p => p.piece[0] + p.piece[1]);

        return new Move(this.PlayerID, gordastPiece.piece, false, gordastPiece.right);
    }
}