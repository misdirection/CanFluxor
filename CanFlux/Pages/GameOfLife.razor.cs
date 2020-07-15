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
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        IState<CanFlux.Store.GameOfLife.GameOfLifeHistoryState> GameOfLifeHistoryState { get; set; }

        private Canvas2DContext _context;
        private Timer _timer;
        private System.Random Random { get; set; } = new System.Random();

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
            await ClearBoard(GameOfLifeHistoryState.Value.Present.BoardSize);
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
                        await DrawSquare(x, y);
                }
            }
        }
        #endregion

        private async Task DrawAt(int x, int y)
        {
            var data = await JSRuntime.InvokeAsync<string>("getDivCanvasOffsets", new object[] { divCanvas });
            var offsets = (JObject)JsonConvert.DeserializeObject(data);
            var xCoord = x - (int)offsets.Value<double>("offsetLeft") - (x - (int)offsets.Value<double>("offsetLeft")) % SquareSize;
            var yCoord = y - (int)offsets.Value<double>("offsetTop") - (y - (int)offsets.Value<double>("offsetTop")) % SquareSize;
            if (xCoord >= 0 && xCoord < BoardSize && yCoord >= 0 && yCoord < BoardSize)
            {
                if (GameOfLifeHistoryState.Value.Present.Cells[xCoord / SquareSize, yCoord / SquareSize] == Color.White)
                {
                    GameOfLifeHistoryState.Value.Present.Cells[xCoord / SquareSize, yCoord / SquareSize] = GetColorFromString(pickedColor);
                    await _context.SetFillStyleAsync(pickedColor);
                    AddToColorCount(pickedColor);
                }
                else if (GameOfLifeHistoryState.Value.Present.Cells[xCoord / SquareSize, yCoord / SquareSize] == GetColorFromString(pickedColor))
                {
                    GameOfLifeHistoryState.Value.Present.Cells[xCoord / SquareSize, yCoord / SquareSize] = Color.White;
                    await _context.SetFillStyleAsync("White");
                    RemoveFromColorCount(pickedColor);
                }
                else
                {
                    RemoveFromColorCount(GetStringFromColor(GameOfLifeHistoryState.Value.Present.Cells[xCoord / SquareSize, yCoord / SquareSize]));
                    GameOfLifeHistoryState.Value.Present.Cells[xCoord / SquareSize, yCoord / SquareSize] = GetColorFromString(pickedColor);
                    await _context.SetFillStyleAsync(pickedColor);
                    AddToColorCount(pickedColor);
                }
                await _context.FillRectAsync(xCoord, yCoord, SquareSize, SquareSize);
                
                StateHasChanged();
            }
        }

        protected async void CanvasClicked(MouseEventArgs args)
        {
            if (!_gameStarted)
            {
                await DrawAt((int)args.ClientX, (int)args.ClientY);
            }
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

        private void RemoveFromColorCount(string color)
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


        private string GetStringFromColor(Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        private Color GetColorFromString(string str)
        {
            int red = int.Parse(str.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            int green = int.Parse(str.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            int blue = int.Parse(str.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            return Color.FromArgb(255, red, green, blue);
        }

        #region ColorPicker
        bool colorPickerisOpened = false;
        string pickedColor = "#F1F7E9";

        void OpenColorPicker()
        {
            colorPickerisOpened = true;
        }

        void ClosedColorPickerEvent(string value)
        {
            pickedColor = value;
            colorPickerisOpened = false;
        }
        #endregion
    }
}
