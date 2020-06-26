using System;

namespace CanFlux.Store.GameOfLife
{
    public class GameOfLifeState
    {
        public GameOfLifeState(int boardSize, int squareSize)
        {
            SquareSize = squareSize;
            BoardSize = boardSize;
            ArraySize = BoardSize / SquareSize;
            GenerationCount = 0;
            OldGeneration = new bool[ArraySize,ArraySize];
            OldGeneration[30,30] = true;
            OldGeneration[30,31] = true;
            OldGeneration[30,32] = true; 
            OldGeneration[31,30] = true;
            OldGeneration[32,30] = true; 
            OldGeneration[33,31] = true;
            OldGeneration[34,32] = true;
            

        }

        public GameOfLifeState(bool[,] newGeneration, int boardSize, int squareSize, int newGenerationCount)
        {
            OldGeneration = newGeneration;
            BoardSize = boardSize;
            SquareSize = squareSize;
            ArraySize = BoardSize/ SquareSize;
            GenerationCount = newGenerationCount;
        }

        public bool[,] OldGeneration { get; }
        public int GenerationCount { get; }
        public int BoardSize { get; }
        public int SquareSize { get; }
        public int ArraySize { get; }
        //public Timer Tick { get; }
    }
}
