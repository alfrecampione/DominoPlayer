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
        int numberOfParts;
        int piecesPerHand;
        public Board(int numberOfPlayers, int piecesPerHand, int maxValue, int numberOfParts)
        {
            this.undistributedPieces = new List<Piece>();
            this.board = new List<Piece>();
            this.actualTurn = 1;
            this.piecesPerHand = piecesPerHand;
            this.numberOfParts = numberOfParts;
            int[] capacity = new int[numberOfParts];
            CreateAllPieces(maxValue, 0, capacity);
        }
        void CreateAllPieces(int maxValue, int numberOfPart, int[] values)
        {
            if (numberOfPart == values.Length - 1)
            {
                Piece p = new(values);
                bool found = false;
                for (int i = 0; i < undistributedPieces.Count; i++)
                {
                    if (undistributedPieces[i] == p)
                        found = true;
                }
                if (!found)
                    undistributedPieces.Add(p);
            }
            for (int i = 0; i <= maxValue; i++)
            {
                values[numberOfParts] = i;
                CreateAllPieces(maxValue, numberOfParts + 1, values);
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
