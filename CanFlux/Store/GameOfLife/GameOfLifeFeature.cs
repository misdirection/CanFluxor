using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanFlux.Store.GameOfLife
{
    public class GameOfLifeFeature : Feature<GameOfLifeState>
    {
        public override string GetName() => "Game of Life";

        protected override GameOfLifeState GetInitialState() => new GameOfLifeState(800, 10);
    }
}
