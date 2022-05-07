using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominoPlayer
{
    class Player
    {
        List<Piece> hand;
        Board board;
        List<List<int>> missingNumbersPerPlayer;
        int positionToPlay;
        int actualTurn;
        bool haveTeammate;
        public Player(List<Piece> hand, Board board, int positionToPlay, bool haveTeammate)
        {
            actualTurn = 1;
            this.hand = hand;
            this.board = board;
            this.positionToPlay = positionToPlay;
            this.haveTeammate = haveTeammate;
            missingNumbersPerPlayer = new();
        }
        public List<Piece> Hand
        {
            get { return hand; }
        }
        public int ActualTurn
        {
            get
            {
                actualTurn = board.ActualTurn;
                return actualTurn;
            }
        }
        public bool TurnToPlay()
        {
            if (ActualTurn % positionToPlay == 0)
                return true;
            else
                return false;
        }
        public void Play()
        {

        }
    }
}
