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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;

namespace CanFlux.Pages
{
    public partial class GameOfLife : FluxorComponent
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private IState<CanFlux.Store.GameOfLife.GameOfLifeHistoryState> GameOfLifeHistoryState { get; set; }

        private Canvas2DContext _context;
        private Timer _timer;

        protected ElementReference divCanvas;
        protected BECanvasComponent _canvasReference;
        protected bool _gameStarted = false;
        protected int BoardSize { get; set; }
        protected int SquareSize { get; set; }
        protected Dictionary<string, int> ColorCount { get; set; } = new Dictionary<string, int>();

        protected override void OnInitialized()
        {
            BoardSize = GameOfLifeHistoryState.Value.Present.BoardSize;
            SquareSize = GameOfLifeHistoryState.Value.Present.SquareSize;
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
            await ClearBoard(BoardSize);
            await DrawBoard();
            await _context.EndBatchAsync();
            StateHasChanged();
        }

        private void UpdateBoard(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Dispatch(new PopulateAction());
        }

        #region Drawing
        private async Task ClearBoard(int boardSize) => await _context.ClearRectAsync(0, 0, boardSize, boardSize);

        private async Task DrawSquare(int x, int y)
        {
            var xCoord = x * SquareSize;
            var yCoord = y * SquareSize;
            var color = GetStringFromColor(GameOfLifeHistoryState.Value.Present.Cells[x, y]);
            await _context.SetFillStyleAsync(color);
            await _context.FillRectAsync(xCoord, yCoord, SquareSize, SquareSize);
            if (!ColorCount.ContainsKey(color))
            {
                ColorCount.Add(color, 1);
            }
            else
            {
                ColorCount[color]++;
            }
        }

        private async Task DrawBoard()
        {
            ColorCount = new Dictionary<string, int>();
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
            var xCoord = ((x - (int)offsets.Value<double>("offsetLeft")) / SquareSize) * SquareSize;
            var yCoord = ((y - (int)offsets.Value<double>("offsetTop")) / SquareSize) * SquareSize;
            if (xCoord >= 0 && xCoord < BoardSize && yCoord >= 0 && yCoord < BoardSize)
            {
                if (GameOfLifeHistoryState.Value.Present.Cells[xCoord / SquareSize, yCoord / SquareSize] == Color.White)
                {
                    GameOfLifeHistoryState.Value.Present.Cells[xCoord / SquareSize, yCoord / SquareSize] = GetColorFromString(PickedColor);
                    await _context.SetFillStyleAsync(PickedColor);
                    AddToColorCount(PickedColor);
                }
                else if (GameOfLifeHistoryState.Value.Present.Cells[xCoord / SquareSize, yCoord / SquareSize] == GetColorFromString(PickedColor))
                {
                    GameOfLifeHistoryState.Value.Present.Cells[xCoord / SquareSize, yCoord / SquareSize] = Color.White;
                    await _context.SetFillStyleAsync("White");
                    SubtractFromColorCount(PickedColor);
                }
                else
                {
                    SubtractFromColorCount(GetStringFromColor(GameOfLifeHistoryState.Value.Present.Cells[xCoord / SquareSize, yCoord / SquareSize]));
                    GameOfLifeHistoryState.Value.Present.Cells[xCoord / SquareSize, yCoord / SquareSize] = GetColorFromString(PickedColor);
                    await _context.SetFillStyleAsync(PickedColor);
                    AddToColorCount(PickedColor);
                }
                await _context.FillRectAsync(xCoord, yCoord, SquareSize, SquareSize);
                
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
            base.Dispose(disposing);
            _timer.Elapsed -= UpdateBoard;
            GameOfLifeHistoryState.StateChanged -= ReDraw;
            _timer.Dispose();
            _context.Dispose();
        }

        #region ColorHelperFunctions
        private string GetStringFromColor(Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        private Color GetColorFromString(string str)
        {
            var red = int.Parse(str.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            var green = int.Parse(str.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            var blue = int.Parse(str.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            return Color.FromArgb(255, red, green, blue);
        }

        private void AddToColorCount(string color)
        {
            if (!ColorCount.ContainsKey(color))
            {
                ColorCount.Add(color, 1);
            }
            else
            {
                ColorCount[color]++;
            }
        }

        private void SubtractFromColorCount(string color)
        {
            if (ColorCount[color] == 1)
            {
                ColorCount.Remove(color);
            }
            else
            {
                ColorCount[color]--;
            }
        }
        #endregion

        #region ColorPicker
        protected bool ColorPickerisOpened { get; set; } = false;
        protected string PickedColor { get; set; } = "#212121";

        void OpenColorPicker()
        {
            ColorPickerisOpened = true;
        }

        void ClosedColorPickerEvent(string value)
        {
            PickedColor = value;
            ColorPickerisOpened = false;
        }
        #endregion
    }
}
