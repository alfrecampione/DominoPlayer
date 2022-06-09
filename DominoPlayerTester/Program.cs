using DominoPlayer.AI;
namespace DominoPlayer.Tester;

public class Program
{
    static void Main()
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

        public bool GameOverCondition(DominoGame game, out int winner)
        {
            var players = game.Players;
            foreach (var player in players)
                if (player.PiecesInHand == 0)
                {
                    winner = player.PlayerID;
                    return true;
                }

            int passCount = 0;
            for (int i = game.history.Count - 1; i >= 0; i--)
            {
                if (game.history[i].passed)
                    passCount++;
                else
                    break;
            }

            if (passCount >= players.Count)
            {
                winner = players.OrderBy(p => p.GetPlayerScore()).First().PlayerID;
                return true;
            }

            winner = -1;
            return false;
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

        public double GetHandScore(DominoGame game, IEnumerable<Piece> hand)
         => hand.Sum(p => p.Left + p.Right);
    }
    static void PlayDomino()
    {
        dominoBoard.Clear();
        IRules classicRules = new ClassicGame(10, 9, 4, 2);
        game = new(classicRules);

        game.OnMoveMade += OnDominoMove;
        DominoPlayer smartAI_0 = new SmartAI(0, game);
        DominoPlayer smartAI_1 = new SmartAI(1, game);
        DominoPlayer smartAI_2 = new SmartAI(2, game);
        DominoPlayer smartAI_3 = new SmartAI(3, game);


        smartAI_0.SetOpponents(1, 3);
        smartAI_1.SetOpponents(0, 2);
        smartAI_2.SetOpponents(1, 3);
        smartAI_3.SetOpponents(0, 2);

        smartAI_0.SetPartners(2);
        smartAI_1.SetPartners(3);
        smartAI_2.SetPartners(0);
        smartAI_3.SetPartners(1);

        game.StartGame(smartAI_0, smartAI_1, smartAI_2, smartAI_3);

        while (true)
        {
            game.NextTurn();

            //Console.ReadLine();
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
        System.Console.WriteLine($"It's over, Player {winner} won!!!!");
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
        if (game == null) return;

        Console.Clear();
        Console.WriteLine($@"Players: {game.Players.Count}
            Current Player: {game.CurrentPlayer}");

        Console.WriteLine();

        foreach (var piece in dominoBoard)
        {
            Console.Write(PaintPiece(piece));
        }
        System.Console.WriteLine("\n");

        foreach (var player in game.Players) //if (player.GetType().IsSubclassOf(typeof(AIPlayer)))
        {
            if (player is not AIPlayer ai) continue;

            System.Console.WriteLine($"Player {player.PlayerID} Hand:");
            foreach (var piece in ai.GetHand())
            {
                Console.Write(PaintPiece(piece));
            }
            System.Console.WriteLine("\n");
        }

    }
    static string PaintPiece(Piece piece)
        => $"[{NumberToEmoji(piece.Left)} |{NumberToEmoji(piece.Right)} ]";
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