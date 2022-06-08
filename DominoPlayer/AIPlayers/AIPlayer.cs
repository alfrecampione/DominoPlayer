using System.Collections.Generic;

namespace DominoPlayer.AI
{
    public abstract class AIPlayer : DominoPlayer
    {
        public AIPlayer(int playerID, DominoGame game) : base(playerID, game) { }

        public IEnumerable<Piece> GetHand() => Hand;
    }
}