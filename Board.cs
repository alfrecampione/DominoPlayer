using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominoPlayer
{
    public class Board
    {
        int actualTurn;
        public int ActualTurn
        {
            get { return actualTurn; }
        }
        List<Piece> board;
        List<Piece> undistributedPieces;

        int piecesPerHand;
        public Board(int numberOfPlayers, int piecesPerHand, int maxValue)
        {
            this.undistributedPieces = new List<Piece>();
            this.board = new List<Piece>();
            this.actualTurn = 1;
            this.piecesPerHand = piecesPerHand;
            CreateAllPieces(maxValue);
        }

        void CreateAllPieces(int maxValue)
        {
            for (int i = 0; i <= maxValue; i++)
            {
                for (int j = i; j <= maxValue; j++)
                {
                    undistributedPieces.Add(new(i, j));
                }
            }
        }

        List<Piece> CreateHand()
        {
            Random random = new Random();
            List<Piece> hand = new();
            for (int i = 0; i < piecesPerHand; i++)
            {
                int index = random.Next(undistributedPieces.Count);
                hand.Add(undistributedPieces[index]);
                undistributedPieces.RemoveAt(index);
            }
            return hand;
        }
    }
}
