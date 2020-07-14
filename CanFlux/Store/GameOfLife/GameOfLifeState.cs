using System;
using System.Drawing;

namespace CanFlux.Store.GameOfLife
{
    public class GameOfLifeState
    {
        public Color[,] Cells { get; }
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
            Cells = new Color[ArraySize, ArraySize];
            for (int x = 0; x < ArraySize; x++)
            {
                for (int y = 0; y < ArraySize; y++)
                {
                    Cells[x, y] = Color.White;
                }
            }

            //Cells[30, 30] = Color.FromArgb(255, 83, 230, 27);
            //Cells[30, 31] = Color.FromArgb(255, 65, 15, 190);
            //Cells[30, 32] = Color.FromArgb(255, 207, 227, 184);
            //Cells[31, 30] = Color.FromArgb(255, 94, 147, 211);
            //Cells[32, 30] = Color.FromArgb(255, 181, 117, 179);
            //Cells[33, 31] = Color.FromArgb(255, 46, 66, 244);
            //Cells[34, 32] = Color.FromArgb(255, 35, 87, 109);

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

        public GameOfLifeState(Color[,] newGeneration, int boardSize, int squareSize, int newGenerationCount)
        {
            Cells = newGeneration;
            BoardSize = boardSize;
            SquareSize = squareSize;
            ArraySize = BoardSize / SquareSize;
            GenerationCount = newGenerationCount;
        }
    }
}
