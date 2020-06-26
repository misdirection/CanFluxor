using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using CanFlux.Store.GameOfLife;
using CanFlux.Store.GameTimer;

namespace CanFlux.Pages
{
    public partial class Index
    {
        private Canvas2DContext _context;

        protected BECanvasComponent _canvasReference;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _context = await _canvasReference.CreateCanvas2DAsync();
                TimerState.Value.Tick.Elapsed += UpdateAsync;
            }
        }

        private async void UpdateAsync(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Dispatch(new PopulateAction());
            await _context.BeginBatchAsync();
            await ClearBoard(BoardState.Value.BoardSize);
            await DrawBoard();
            await _context.EndBatchAsync();
        }


        #region Drawing
        private async Task ClearBoard(int boardSize) => await _context.ClearRectAsync(0, 0, boardSize, boardSize);

        private async Task DrawSquare(int x, int y)
        {
            var xCoord = x * BoardState.Value.SquareSize;
            var yCoord = y * BoardState.Value.SquareSize;
            await _context.FillRectAsync(xCoord, yCoord, BoardState.Value.SquareSize, BoardState.Value.SquareSize);
        }

        private async Task DrawBoard()
        {
            for (var x = 0; x < BoardState.Value.ArraySize; x++)
            {
                for (var y = 0; y < BoardState.Value.ArraySize; y++)
                {
                    if (BoardState.Value.OldGeneration[x, y])
                        await DrawSquare(x, y);
                }
            }
        }
        #endregion

        protected void Start(MouseEventArgs args) => Dispatcher.Dispatch(new StartTimerAction());

        protected override void Dispose(bool disposing)
        {

            //TimerState.Value.Tick.Elapsed -= UpdateAsync;
            _context.Dispose();
            TimerState.Value.Tick.Dispose();
            base.Dispose(disposing);

        }

        #region Ausgelagertes Zeug
        //private bool[,] CalculateNextGeneration()
        //{
        //    var newGeneration = new bool[State.Value.ArraySize, State.Value.ArraySize];
        //    for (var x = 0; x < State.Value.ArraySize; x++)
        //    {
        //        for (var y = 0; y < State.Value.ArraySize; y++)
        //        {
        //            int count = IsNeighbourAlive(x - 1, y - 1)
        //                + IsNeighbourAlive(x - 1, y)
        //                + IsNeighbourAlive(x - 1, y + 1)
        //                + IsNeighbourAlive(x, y - 1)
        //                + IsNeighbourAlive(x, y + 1)
        //                + IsNeighbourAlive(x + 1, y - 1)
        //                + IsNeighbourAlive(x + 1, y)
        //                + IsNeighbourAlive(x + 1, y + 1);

        //            var shouldLive = false;
        //            var isAlive = State.Value.OldGeneration[x, y];
        //            if (isAlive && (count == 3 || count == 2))
        //            {
        //                shouldLive = true;
        //            }
        //            else if (!isAlive && count == 3)
        //            {
        //                shouldLive = true;
        //            }
        //            newGeneration[x, y] = shouldLive;
        //        }
        //    }
        //    return newGeneration;
        //}
        //protected async override Task OnInitializedAsync()
        //{
        //    InitBoard(800, 10);
        //    _oldGeneration[40, 30] = true;
        //    _oldGeneration[40, 31] = true;
        //    _oldGeneration[40, 32] = true;
        //    _oldGeneration[41, 30] = true;
        //    _oldGeneration[42, 30] = true;
        //    _oldGeneration[42, 31] = true;
        //    _oldGeneration[42, 32] = true;
        //    await base.OnInitializedAsync();
        //}
        //private bool[,] _oldGeneration;
        //private int _squareSize;
        //private int _boardSize;
        //private int _arraySize;

        //private void InitBoard(int boardSize, int squareSize)
        //{
        //    _boardSize = boardSize;
        //    _squareSize = squareSize;
        //    _arraySize = _boardSize / _squareSize;
        //    _oldGeneration = new bool[_arraySize, _arraySize];
        //}
        //private int IsNeighbourAlive(int x, int y)
        //{
        //    var result = 0;

        //    bool outOfBounds = x < 0 || x > BoardState.Value.ArraySize - 1 || y < 0 || y > BoardState.Value.ArraySize - 1;
        //    if (!outOfBounds)
        //    {
        //        result = BoardState.Value.OldGeneration[x, y] ? 1 : 0;
        //    }
        //    return result;
        //}
        #endregion
    }
}
