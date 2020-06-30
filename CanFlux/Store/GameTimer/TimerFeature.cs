using Fluxor;

namespace CanFlux.Store.GameTimer
{
    public class TimerFeature : Feature<TimerState>
    {
        public override string GetName() => "GameTimer";

        protected override TimerState GetInitialState() => new TimerState(10);
    }
}
