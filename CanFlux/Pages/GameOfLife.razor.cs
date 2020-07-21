using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using CanFlux.Models;
using CanFlux.Store.GameOfLife;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Timers;

namespace CanFlux.Pages
{
    public partial class GameOfLife : FluxorComponent
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private IState<GameOfLifeHistoryState> GameOfLifeHistoryState { get; set; }

        [Inject]
        private IDispatcher Dispatcher { get; set; }

        private Canvas2DContext _context;
        private Timer _timer;
#pragma warning disable CS0649
        private ElementReference divCanvas;
#pragma warning disable CS0649
        private BECanvasComponent _canvasReference;
        private bool _gameStarted = false;
        private int _boardSize;
        private int _squareSíze;
        private Dictionary<string, int> _colorCount = new Dictionary<string, int>();

        protected override void OnInitialized()
        {
            _boardSize = GameOfLifeHistoryState.Value.Present.BoardSize;
            _squareSíze = GameOfLifeHistoryState.Value.Present.SquareSize;
        }

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
            await ClearBoard(_boardSize);
            await DrawBoard();
            await _context.EndBatchAsync();
            StateHasChanged();
        }

        private void UpdateBoard(object sender, ElapsedEventArgs e) => Dispatcher.Dispatch(new PopulateAction());

        #region Drawing
        private async Task ClearBoard(int boardSize) => await _context.ClearRectAsync(0, 0, boardSize, boardSize);

        private async Task DrawSquare(int x, int y)
        {
            var xCoord = x * _squareSíze;
            var yCoord = y * _squareSíze;
            var color = GameOfLifeHistoryState.Value.Present.Cells[x, y].ToColorCode();
            await _context.SetFillStyleAsync(color);
            await _context.FillRectAsync(xCoord, yCoord, _squareSíze, _squareSíze);
            if (!_colorCount.ContainsKey(color))
            {
                _colorCount.Add(color, 1);
            }
            else
            {
                _colorCount[color]++;
            }
        }

        private async Task DrawBoard()
        {
            _colorCount = new Dictionary<string, int>();
            await _context.StrokeRectAsync(0, 0, _canvasReference.Width, _canvasReference.Height);
            for (var x = 0; x < GameOfLifeHistoryState.Value.Present.ArraySize; x++)
            {
                for (var y = 0; y < GameOfLifeHistoryState.Value.Present.ArraySize; y++)
                {
                    if (GameOfLifeHistoryState.Value.Present.Cells[x, y] != Color.White)
                    {
                        await DrawSquare(x, y);
                    }
                }
            }
        }        

        private async Task DrawAt(int x, int y)
        {
            var data = await JSRuntime.InvokeAsync<string>("getDivCanvasOffsets", new object[] { divCanvas });
            var offsets = (JObject)JsonConvert.DeserializeObject(data);
            var xCoord = ((x - (int)offsets.Value<double>("offsetLeft")) / _squareSíze) * _squareSíze;
            var yCoord = ((y - (int)offsets.Value<double>("offsetTop")) / _squareSíze) * _squareSíze;
            if (xCoord >= 0 && xCoord < _boardSize && yCoord >= 0 && yCoord < _boardSize)
            {
                if (GameOfLifeHistoryState.Value.Present.Cells[xCoord / _squareSíze, yCoord / _squareSíze] == Color.White)
                {
                    GameOfLifeHistoryState.Value.Present.Cells[xCoord / _squareSíze, yCoord / _squareSíze] = _pickedColor.ToColor();
                    await _context.SetFillStyleAsync(_pickedColor);
                    AddToColorCount(_pickedColor);
                }
                else if (GameOfLifeHistoryState.Value.Present.Cells[xCoord / _squareSíze, yCoord / _squareSíze] == _pickedColor.ToColor())
                {
                    GameOfLifeHistoryState.Value.Present.Cells[xCoord / _squareSíze, yCoord / _squareSíze] = Color.White;
                    await _context.SetFillStyleAsync("White");
                    SubtractFromColorCount(_pickedColor);
                }
                else
                {
                    SubtractFromColorCount(GameOfLifeHistoryState.Value.Present.Cells[xCoord / _squareSíze, yCoord / _squareSíze].ToColorCode());
                    GameOfLifeHistoryState.Value.Present.Cells[xCoord / _squareSíze, yCoord / _squareSíze] = _pickedColor.ToColor();
                    await _context.SetFillStyleAsync(_pickedColor);
                    AddToColorCount(_pickedColor);
                }
                await _context.FillRectAsync(xCoord, yCoord, _squareSíze, _squareSíze);
                
                StateHasChanged();
            }
        }
        #endregion

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
            //base.Dispose(disposing);
            _timer.Elapsed -= UpdateBoard;
            GameOfLifeHistoryState.StateChanged -= ReDraw;
            _timer.Dispose();
            _context.Dispose();
        }

        #region ColorHelperFunctions

        private void AddToColorCount(string color)
        {
            if (!_colorCount.ContainsKey(color))
            {
                _colorCount.Add(color, 1);
            }
            else
            {
                _colorCount[color]++;
            }
        }

        private void SubtractFromColorCount(string color)
        {
            if (_colorCount[color] == 1)
            {
                _colorCount.Remove(color);
            }
            else
            {
                _colorCount[color]--;
            }
        }
        #endregion

        #region ColorPicker
#pragma warning disable CS0414
        private bool _colorPickerisOpened = false;
        private string _pickedColor = "#212121";

        private void OpenColorPicker() => _colorPickerisOpened = true;

        private void ClosedColorPickerEvent(string value)
        {
            _pickedColor = value;
            _colorPickerisOpened = false;
        }
        #endregion
    }
}
