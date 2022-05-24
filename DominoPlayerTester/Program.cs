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
    static void PlayDomino()
    {
        DominoGame game = new(10, 6);
        game.StartGame(new IDominoPlayer[]
        {
            new RandomAI(0, game),
            new BotaGordaAI(1, game)
        });

        while(true){
            
        }
    }
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