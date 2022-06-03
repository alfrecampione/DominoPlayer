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
        private bool reversed;
        public Piece(int valueA, int valueB)
        {
            this.sideA = valueA;
            this.sideB = valueB;
            this.reversed = false;
        }
        public void Reverse()
        {
            reversed = !reversed;
        }
        public bool CanMatch(Piece other, bool right, out bool reversed)
        {
            var compare = (Piece a, Piece b) =>
            (right ? a.GetRight() : a.GetLeft()) == (right ? other.GetLeft() : other.GetRight());

            bool nonReversedComparison = compare(this, other);

            if (nonReversedComparison)
            {
                reversed = false;
                return true;
            }
            else
            {
                reversed = true;
                other.Reverse();

                return compare(this, other);
            }
        }
        public int GetLeft() => reversed ? sideB : sideA;
        public int GetRight() => reversed ? sideA : sideB;

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
            var eq = (Piece l, Piece r) => l.GetLeft() == r.GetLeft() && l.GetRight() == r.GetRight();

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
