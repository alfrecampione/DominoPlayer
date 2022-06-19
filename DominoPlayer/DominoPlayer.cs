using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
namespace DominoPlayer
{
    public abstract class DominoPlayer
    {//test
        public int PlayerID { get; }
        protected DominoGame GameReference { get; }

        protected List<Piece> Hand { get; }

        protected Dictionary<int, List<double>> valuesPartnerNotHave;
        protected Dictionary<int, List<double>> valuesOpponentNothave;
        public int Count { get { return Hand.Count; } }
        public DominoPlayer(int playerID, DominoGame game)
        {
            this.PlayerID = playerID;
            this.GameReference = game;
            this.Hand = new List<Piece>();
            this.valuesOpponentNothave = new Dictionary<int, List<double>>();
            this.valuesPartnerNotHave = new Dictionary<int, List<double>>();
        }
        //=> (PlayerID, GameReference, Hand) = (playerID, game, new List<Piece>());
        public double GetPlayerScore() => GameReference.gameRules.GetHandScore(GameReference, Hand);
        public Move GetMove()
        {
            Move move = InternalGetMove();
            if (!move.passed)
                Hand.RemoveAt(Hand.FindIndex(p => p == move.piecePlaced));
            return move;
        }
        protected abstract Move InternalGetMove();
        public virtual int PiecesInHand => Hand.Count;
        public virtual void StartGame(IEnumerable<Piece> startingHand)
        {
            Hand.Clear();
            Hand.AddRange(startingHand);
        }
        public void SetPartners(params int[] partnersID)
        {
            valuesPartnerNotHave.Clear();
            foreach (var ID in partnersID)
            {
                valuesPartnerNotHave.Add(ID, new List<double>());
            }
        }
        public void SetOpponents(params int[] opponentsID)
        {
            valuesOpponentNothave.Clear();
            foreach (var ID in opponentsID)
            {
                valuesOpponentNothave.Add(ID, new List<double>());
            }
        }
    }
}