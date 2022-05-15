using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominoPlayer
{
    public class Piece
    {
        private int[] numbers;
        public Piece(params int[] numbers)
        {
            this.numbers = numbers;
        }
        public void Reverse()
        {
            int mid = numbers.Length / 2;
            for (int i = 0; i < mid; i++)
            {
                int temp = numbers[i];
                numbers[i] = numbers[numbers.Length - i - 1];
                numbers[numbers.Length - 1 - i] = temp;
            }
        }
        public int this[int i]
        {
            get { return numbers[i]; }
        }
        public static bool operator ==(Piece a, Piece b)
        {
            Dictionary<int, int> aDict = new();
            Dictionary<int, int> bDict = new();
            for (int i = 0; i < a.numbers.Length; i++)
            {
                try
                {
                    aDict[a[i]]++;
                }
                catch (KeyNotFoundException)
                {
                    aDict.Add(a[i], 1);
                }
                try
                {
                    aDict[b[i]]++;
                }
                catch (KeyNotFoundException)
                {
                    aDict.Add(b[i], 1);
                }
            }
            for (int i = 0; i < aDict.Count; i++)
            {
                if (aDict.Keys.ToArray()[i] != bDict.Keys.ToArray()[i] || aDict[aDict.Keys.ToArray()[i]] != bDict[bDict.Keys.ToArray()[i]])
                    return false;
            }
            return true;
        }
        public static bool operator !=(Piece a, Piece b)
        {
            return !(a == b);
        }
    }
}
