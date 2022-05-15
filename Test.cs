using DominoPlayer;

Board board = new(2, 10, 6, 2);
Player p1 = new(board.CreateHand(), board, 1, false);
Player p2 = new(board.CreateHand(), board, 2, false);