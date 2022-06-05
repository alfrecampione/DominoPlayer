using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominoPlayer
{
    public struct Piece
    {
        private readonly int sideA;
        private readonly int sideB;
        public int Left => reversed ? sideB : sideA;
        public int Right => reversed ? sideA : sideB;

        private bool reversed;

        private readonly IRules gameRules;

        public Piece(int valueA, int valueB, IRules rules)
        {
            sideA = valueA;
            sideB = valueB;
            reversed = false;
            gameRules = rules;
        }
        public void Reverse()
        {
            reversed = !reversed;
        }

        public bool CanMatch(Piece other, bool right, out bool reversed)
        {
            reversed = false;
            bool nonReversedComparison = gameRules.CanPiecesMatch(this, other, right);

            if (nonReversedComparison)
                return true;


            reversed = true;
            other.Reverse();

            return gameRules.CanPiecesMatch(this, other, right);
        }


        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType().BaseType != typeof(Piece))
                return base.Equals(obj);

            Piece p = (Piece)obj;

            return this == p;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator ==(Piece a, Piece b)
        {
            var eq = (Piece l, Piece r) => l.Left == r.Left && l.Right == r.Right;

            bool initialComparison = eq(a, b);

            if (initialComparison)
                return true;
            else
            {
                a.Reverse();
                return eq(a, b);
            }
        }
        public static bool operator !=(Piece a, Piece b)
        {
            return !(a == b);
        }
    }
}
