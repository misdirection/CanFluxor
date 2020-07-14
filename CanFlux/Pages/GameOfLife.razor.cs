using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using CanFlux.Store.GameOfLife;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Timers;

namespace CanFlux.Pages
{
    public partial class GameOfLife : FluxorComponent
    {
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        IState<CanFlux.Store.GameOfLife.GameOfLifeHistoryState> GameOfLifeHistoryState { get; set; }

        private Canvas2DContext _context;
        private Timer _timer;

        protected ElementReference divCanvas;
        protected BECanvasComponent _canvasReference;
        protected bool _gameStarted = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {            
            if (firstRender)
            {
                _context = await _canvasReference.CreateCanvas2DAsync();
                await DrawBoard();
                _timer = new Timer(200);
                _timer.Elapsed += UpdateBoard;
                GameOfLifeHistoryState.StateChanged += ReDraw;
            }
            
        }

        private async void ReDraw(object sender, GameOfLifeHistoryState e)
        {
            await _context.BeginBatchAsync();
            await ClearBoard(GameOfLifeHistoryState.Value.Present.BoardSize);
            await DrawBoard();
            await _context.EndBatchAsync();
        }

        private void UpdateBoard(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Dispatch(new PopulateAction());
        }

        #region Drawing
        private async Task ClearBoard(int boardSize) => await _context.ClearRectAsync(0, 0, boardSize, boardSize);

        private async Task DrawSquare(int x, int y)
        {
            var xCoord = x * GameOfLifeHistoryState.Value.Present.SquareSize;
            var yCoord = y * GameOfLifeHistoryState.Value.Present.SquareSize;
            await _context.SetFillStyleAsync(GetColorString(GameOfLifeHistoryState.Value.Present.Cells[x, y]));
            await _context.FillRectAsync(xCoord, yCoord, GameOfLifeHistoryState.Value.Present.SquareSize, GameOfLifeHistoryState.Value.Present.SquareSize);
        }

        private string GetColorString(Color color)
        {
            return "#" + (color.R).ToString("X2") + (color.G).ToString("X2") + (color.B).ToString("X2");
        }

        private async Task DrawBoard()
        {
            await _context.StrokeRectAsync(0, 0, _canvasReference.Width, _canvasReference.Height);
            for (var x = 0; x < GameOfLifeHistoryState.Value.Present.ArraySize; x++)
            {
                for (var y = 0; y < GameOfLifeHistoryState.Value.Present.ArraySize; y++)
                {
                    if (GameOfLifeHistoryState.Value.Present.Cells[x, y] != Color.White)
                        await DrawSquare(x, y);
                }
            }
        }
        #endregion

        private async Task DrawAt(int x, int y)
        {
            string data = await JSRuntime.InvokeAsync<string>("getDivCanvasOffsets", new object[] { divCanvas });
            JObject offsets = (JObject)JsonConvert.DeserializeObject(data);
            var xCoord = x - x % GameOfLifeHistoryState.Value.Present.SquareSize - (int)offsets.Value<double>("offsetLeft");
            var yCoord = y - y % GameOfLifeHistoryState.Value.Present.SquareSize - (int)offsets.Value<double>("offsetTop");
            if (GameOfLifeHistoryState.Value.Present.Cells[xCoord / GameOfLifeHistoryState.Value.Present.SquareSize, yCoord / GameOfLifeHistoryState.Value.Present.SquareSize] == Color.White)
            {
                GameOfLifeHistoryState.Value.Present.Cells[xCoord / GameOfLifeHistoryState.Value.Present.SquareSize, yCoord / GameOfLifeHistoryState.Value.Present.SquareSize] = Color.Red;
                await _context.SetFillStyleAsync("Red");
            }
            else
            {
                GameOfLifeHistoryState.Value.Present.Cells[xCoord / GameOfLifeHistoryState.Value.Present.SquareSize, yCoord / GameOfLifeHistoryState.Value.Present.SquareSize] = Color.White;
                await _context.SetFillStyleAsync("White");
            }

            await _context.FillRectAsync(xCoord, yCoord, GameOfLifeHistoryState.Value.Present.SquareSize, GameOfLifeHistoryState.Value.Present.SquareSize);
        }
        protected async void CanvasClicked(MouseEventArgs args)
        {
            if (!_gameStarted)
            {
                await DrawAt((int)args.ClientX, (int)args.ClientY);
            }
        }

        protected void Start(MouseEventArgs args)
        {
            _gameStarted = !_gameStarted;
            _timer.Start();
        }

        protected void Stop(MouseEventArgs args)
        {
            _timer.Stop();
            _gameStarted = !_gameStarted;
        }
        protected void Undo(MouseEventArgs args) => Dispatcher.Dispatch(new UndoAction());

        protected void Redo(MouseEventArgs args) => Dispatcher.Dispatch(new RedoAction());
        protected void Restart(MouseEventArgs args) => Dispatcher.Dispatch(new RestartAction());

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _timer.Elapsed -= UpdateBoard;
            GameOfLifeHistoryState.StateChanged -= ReDraw;
            _timer.Dispose();
            _context.Dispose();
        }
    }
}
