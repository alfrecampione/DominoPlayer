using DominoPlayer.AI;
namespace DominoPlayer.Tester;

public class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            int selected = SendMenu("Main Menu:", new string[] { "New Game", "Exit" });

            switch (selected)
            {
                case 0:
                    PlayDomino();
                    break;
                default:
                    Console.WriteLine("Bye bye!");
                    Thread.Sleep(500);
                    return;
            }
        }
    }
    const int TURNS_PAUSE = 500;
    static DominoGame? game;
    public class ClassicGame : IRules
    {
        public int PiecesPerHand { get; }
        public int MaxPieceValue { get; }
        public int MaxPlayers { get; }
        public int MinPlayers { get; }
        public ClassicGame(int piecesPerHand, int maxPieceValue, int maxPlayer, int minPlayers)
        {
            this.PiecesPerHand = piecesPerHand;
            this.MaxPieceValue = maxPieceValue;
            this.MaxPlayers = maxPlayer;
            this.MinPlayers = minPlayers;
        }

        //If winner == -1 anybody win
        //Missing draw implementation
        public bool GameOverCondition(DominoGame game, out int winner)
        {
            var players = game.Players;
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].Count == 0)
                {
                    winner = i;
                    return true;
                }
            }
            int passCount = 0;
            for (int i = game.history.Count - 1; i >= 0; i--)
            {
                if (game.history[i].passed)
                    passCount++;
                else
                    break;
            }
            if (passCount == players.Length)
            {
                var sum = (from player in players select player.GetPieces().Sum(p => p.Left + p.Right)).ToList();
                int min = int.MaxValue;
                int index = -1;
                for (int i = 0; i < players.Length; i++)
                {
                    if (sum[i] < min)
                    {
                        min = sum[i];
                        index = i;
                    }
                }
                winner = index;
                return true;
            }
            else
            {
                winner = -1;
                return false;
            }

        }
        public bool CanPiecesMatch(Piece a, Piece b, bool leftToRight)
        {
            if (leftToRight)
            {
                if (a.Right == b.Left)
                    return true;
            }
            else
            {
                if (a.Left == b.Right)
                    return true;
            }
            return false;
        }
    }
    static void PlayDomino()
    {
        IRules classicRules = new ClassicGame(10, 9, 4, 2);
        game = new(classicRules);

        game.OnMoveMade += OnDominoMove;

        game.StartGame(new DominoPlayer[]
        {
            new RandomAI(0, game),
            new BotaGordaAI(1, game)
        });

        while (true)
        {
            game.NextTurn();

            Thread.Sleep(TURNS_PAUSE);

            if (game.IsGameOver(out int winner))
            {
                DominoGameOver(winner);
                return;
            }
        }
    }
    static void DominoGameOver(int winner)
    {

    }
    static readonly List<Piece> dominoBoard = new();
    static void OnDominoMove(Move move)
    {
        if (!move.passed)
            if (move.placedOnRight)
                dominoBoard.Add(move.piecePlaced);
            else
                dominoBoard.Insert(0, move.piecePlaced);

        RepaintDominoBoard();
    }
    static void RepaintDominoBoard()
    {
        Console.Clear();
        Console.WriteLine($@"Players: {2}
            Current Player: {game?.CurrentPlayer}");

        Console.WriteLine();

        foreach (var piece in dominoBoard)
        {
            Console.Write(PaintPiece(piece));
        }
    }
    static string PaintPiece(Piece piece)
        => $"[{NumberToEmoji(piece.Left)}|{NumberToEmoji(piece.Right)}]";
    static string NumberToEmoji(int num) => num switch
    {
        0 => "0️⃣",
        1 => "1️⃣",
        2 => "2️⃣",
        3 => "3️⃣",
        4 => "4️⃣",
        5 => "5️⃣",
        6 => "6️⃣",
        7 => "7️⃣",
        8 => "8️⃣",
        9 => "9️⃣",
        _ => ""
    };
    static string AskForInput(
        string message,
        bool writeLine = true,
        bool character = false,
        bool intercept = false)
    {
        if (writeLine)
            Console.WriteLine(message);
        else
            Console.Write(message);

        if (character)
            return Console.ReadKey(intercept).KeyChar.ToString();
        else
            return Console.ReadLine() ?? "";
    }
    static int SendMenu(string menuTitle, string[] options, Action[]? callbacks = null)
    {
        Console.WriteLine(menuTitle);
        for (int i = 0; i < options.Length; i++)
            Console.WriteLine(i + 1 + " - " + options[i]);

        int selected = int.Parse(AskForInput($"Select item (1-{options.Length}): ", writeLine: false)) - 1;

        if (callbacks != null)
        {
            callbacks[selected]();
            return 0;
        }
        else
            return selected;
    }
}