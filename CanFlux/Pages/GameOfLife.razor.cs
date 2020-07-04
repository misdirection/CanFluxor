﻿using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using CanFlux.Store.GameOfLife;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;
using System.Timers;

namespace CanFlux.Pages
{
    public partial class GameOfLife : FluxorComponent
    {
        private Canvas2DContext _context;
        private Timer _timer;

        protected BECanvasComponent _canvasReference;
        private string _disabled = "false";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _context = await _canvasReference.CreateCanvas2DAsync();
                await _context.StrokeRectAsync(0, 0, _canvasReference.Width, _canvasReference.Height);
                await DrawBoard();
                _timer = new Timer(200);
                _timer.Elapsed += UpdateBoard;
                GameOfLifeHistoryState.StateChanged += RedDraw;
            }
        }

        private async void RedDraw(object sender, GameOfLifeHistoryState e)
        {
            await _context.BeginBatchAsync();
            await ClearBoard(GameOfLifeHistoryState.Value.Present.BoardSize);
            await DrawBoard();
            await _context.EndBatchAsync();
        }

        private async void UpdateBoard(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Dispatch(new PopulateAction());
            await _context.BeginBatchAsync();
            await ClearBoard(GameOfLifeHistoryState.Value.Present.BoardSize);
            await DrawBoard();
            await _context.EndBatchAsync();
        }

        #region Drawing
        private async Task ClearBoard(int boardSize) => await _context.ClearRectAsync(0, 0, boardSize, boardSize);

        private async Task DrawSquare(int x, int y)
        {
            var xCoord = x * GameOfLifeHistoryState.Value.Present.SquareSize;
            var yCoord = y * GameOfLifeHistoryState.Value.Present.SquareSize;
            await _context.FillRectAsync(xCoord, yCoord, GameOfLifeHistoryState.Value.Present.SquareSize, GameOfLifeHistoryState.Value.Present.SquareSize);
        }

        private async Task DrawBoard()
        {
            for (var x = 0; x < GameOfLifeHistoryState.Value.Present.ArraySize; x++)
            {
                for (var y = 0; y < GameOfLifeHistoryState.Value.Present.ArraySize; y++)
                {
                    if (GameOfLifeHistoryState.Value.Present.Cells[x, y])
                        await DrawSquare(x, y);
                }
            }
        }
        #endregion

        //private async Task DrawAt(int x, int y)
        //{
        //    var xCoord = x;
        //    var yCoord = y;
        //    await _context.FillRectAsync(xCoord, yCoord, BoardState.Value.SquareSize, BoardState.Value.SquareSize);
        //}
        //protected void CanvasClicked(MouseEventArgs args)
        //{
        //    DrawAt((int)args.ClientX, (int)args.ClientY);
        //}

        protected void Start(MouseEventArgs args)
        {
            Dispatcher.Dispatch(new StartGameOfLifeAction());
            _timer.Start();
            _disabled = "true";
        }

        protected void Stop(MouseEventArgs args)
        {
            _disabled = "false";
            Dispatcher.Dispatch(new StopGameOfLifeAction());
            _timer.Stop();
        }
        protected void Undo(MouseEventArgs args) => Dispatcher.Dispatch(new UndoAction());

        protected void Redo(MouseEventArgs args) => Dispatcher.Dispatch(new RedoAction());
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Dispatcher.Dispatch(new StopGameOfLifeAction());
            _timer.Elapsed -= UpdateBoard;
            _timer.Dispose();
            _context.Dispose();
        }
    }
}