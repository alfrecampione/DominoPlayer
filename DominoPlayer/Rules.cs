namespace DominoPlayer
{
    static class Rules
    {
        //si el equipo no lleva +teamMissing
        const double teamMissing = 0.15;
        //si el contrario no lleva +opponentMissing
        const double opponentMissing = 0.25;
        //data (n*sameNumber)/t   n=tamaño de la data    t= cantidad de fichas en la mano
        const double sameNumber = 0.50;
        public static double[,] GetValues(List<Piece> hand, DominoGame board, List<List<double>> missingTeamNumbers, List<List<double>> missingOpponentNumbers)
        {
            Dictionary<int, List<int>> dict = new();
            List<Piece> newHand = new();
            #region Choosing the pieces
            for (int i = 0; i < hand.Count; i++)
            {
                try
                {
                    List<int> list = new();
                    list.Add(i);
                    dict.Add(hand[i][0], list);
                }
                catch
                {
                    dict[hand[i][0]].Add(i);
                }
                try
                {
                    List<int> list = new();
                    list.Add(i);
                    dict.Add(hand[i][1], list);
                }
                catch
                {
                    dict[hand[i][1]].Add(i);
                }
            }
            if (dict.Keys.Contains(board[0][0]))
            {
                foreach (var index in dict[board[0][0]])
                {
                    if (!newHand.Contains(hand[index]))
                        newHand.Add(hand[index]);
                }
            }
            if (dict.Keys.Contains(board[board.PiecesInGame - 1][1]))
            {
                foreach (var index in dict[board[board.PiecesInGame - 1][1]])
                {
                    if (!newHand.Contains(hand[index]))
                        newHand.Add(hand[index]);
                }
            }
            #endregion

            double[,] result = new double[2, newHand.Count];
            PointsForTeamMate(hand, missingTeamNumbers, ref result);
            PointsForOponnent(hand, missingOpponentNumbers, ref result);
            PointForSameNumber(dict, hand, ref result);
            return result;
        }
        static void PointsForTeamMate(List<Piece> hand, List<List<double>> missingTeamNumbers, ref double[,] result)
        {
            //asco este triple for
            for (int i = 0; i < missingTeamNumbers.Count; i++)
            {
                for (int j = 0; j < missingTeamNumbers[i].Count; j++)
                {
                    for (int k = 0; k < hand.Count; k++)
                    {
                        if (hand[k][0] == missingTeamNumbers[i][j])
                            result[0, k] += teamMissing;
                        if (hand[k][1] == missingTeamNumbers[i][j])
                            result[1, k] += teamMissing;
                    }
                }
            }
        }
        static void PointsForOponnent(List<Piece> hand, List<List<double>> missingOpponentNumbers, ref double[,] result)
        {
            //asco este triple for
            for (int i = 0; i < missingOpponentNumbers.Count; i++)
            {
                for (int j = 0; j < missingOpponentNumbers[i].Count; j++)
                {
                    for (int k = 0; k < hand.Count; k++)
                    {
                        if (hand[k][0] == missingOpponentNumbers[i][j])
                            result[0, k] += opponentMissing;
                        if (hand[k][1] == missingOpponentNumbers[i][j])
                            result[1, k] += opponentMissing;
                    }
                }
            }
        }
        static void PointForSameNumber(Dictionary<int, List<int>> dict, List<Piece> hand, ref double[,] result)
        {
            int[] keys = new int[dict.Count];
            List<int> numbers = dict.Keys.ToList();
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = dict[i].Count;
            }
            for (int i = 0; i < result.GetLength(1); i++)
            {
                if (dict[hand[i][0]].Count != 1)
                {
                    result[0, i] += (sameNumber * keys[numbers.IndexOf(hand[i][0])]) / hand.Count;
                }
            }
        }
    }
}
