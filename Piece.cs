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
    }
}
