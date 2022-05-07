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
        public Piece(int number1, int number2)
        {
            numbers = new int[2];
            numbers[0] = number1;
            numbers[1] = number2;
        }
        public void Reverse()
        {
            int temp = numbers[0];
            numbers[0] = numbers[1];
            numbers[1] = temp;
        }
        public int this[int i]
        {
            get { return numbers[i]; }
        }
    }
}
