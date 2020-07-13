using System.Drawing;

namespace CanFlux.Store.GameOfLife
{
    public class GameOfLifeState
    {
        public string[,] Cells { get; }
        public int GenerationCount { get; }
        public int BoardSize { get; }
        public int SquareSize { get; }
        public int ArraySize { get; }

        public GameOfLifeState(int boardSize, int squareSize)
        {
            SquareSize = squareSize;
            BoardSize = boardSize;
            ArraySize = BoardSize / SquareSize;
            GenerationCount = 0;
            Cells = new string[ArraySize, ArraySize];
            Cells[30, 30] = "Red";
            Cells[30, 31] = "Red";
            Cells[30, 32] = "Red";
            Cells[31, 30] = "Green";
            Cells[32, 30] = "Green";
            Cells[33, 31] = "Red";
            Cells[34, 32] = "Red";

            //OldGeneration[10, 10] = true;
            //OldGeneration[11, 10] = true;
            //OldGeneration[10, 11] = true;
            //OldGeneration[11, 11] = true;

            //OldGeneration[20, 20] = true;
            //OldGeneration[21, 20] = true;
            //OldGeneration[20, 21] = true;
            //OldGeneration[21, 21] = true;

            //OldGeneration[30,30] = true;
            //OldGeneration[31,30] = true; 
            //OldGeneration[30,31] = true;
            //OldGeneration[31,31] = true;

            //OldGeneration[40,40] = true;
            //OldGeneration[41,40] = true;
            //OldGeneration[40,41] = true;
            //OldGeneration[41,41] = true;
        }

        public GameOfLifeState(GameOfLifeState state, bool gameStarted)
            : this(state.Cells, state.BoardSize, state.SquareSize, state.GenerationCount)
        { }

        public GameOfLifeState(string[,] newGeneration, int boardSize, int squareSize, int newGenerationCount)
        {
            Cells = newGeneration;
            BoardSize = boardSize;
            SquareSize = squareSize;
            ArraySize = BoardSize / SquareSize;
            GenerationCount = newGenerationCount;
        }


    }
}
