using Microsoft.AspNetCore.SignalR;
using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace FluidPumpingSystem
{
    public class ModelTicker
    {
        private Timer _timer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
        private readonly SemaphoreSlim _simulationStateLock = new(1, 1);
        private readonly SemaphoreSlim _updateResultsLock = new(1, 1);
        private volatile SimulationState _simulationState;
        private readonly Subject<int> _subject = new Subject<int>();

        public ModelTicker(IHubContext<ModelTickerHub> hub)
        {
            Hub = hub;
            LoadFMU();
        }

        private IHubContext<ModelTickerHub> Hub
        {
            get;
            set;
        }

        public SimulationState SimulationState
        {
            get { return _simulationState; }
            private set { _simulationState = value; }
        }

        public IObservable<int> StreamResults()
        {
            return _subject;
        }

        private async void UpdateSimulationResults(object state)
        {
            await _updateResultsLock.WaitAsync();
            try
            {
            }
            finally
            {
                _updateResultsLock.Release();
            }
}

        public async Task StartSimulation()
        {
            await _simulationStateLock.WaitAsync();
            try
            {
                if (SimulationState != SimulationState.Started)
                {
                    _timer = new Timer(UpdateSimulationResults, null, _updateInterval, _updateInterval);

                    SimulationState = SimulationState.Started;

                    await BroadcastMarketStateChange(SimulationState.Started);
                }
            }
            finally
            {
                _simulationStateLock.Release();
            }
        }

        public async Task StopSimulation()
        {
            await _simulationStateLock.WaitAsync();
            try
            {
                if (SimulationState == SimulationState.Started)
                {
                    _timer?.Dispose();

                    SimulationState = SimulationState.Stopped;

                    await BroadcastMarketStateChange(SimulationState.Stopped);
                }
            }
            finally
            {
                _simulationStateLock.Release();
            }
        }

        private async Task BroadcastMarketStateChange(SimulationState marketState)
        {
            switch (marketState)
            {
                case SimulationState.Started:
                    await Hub.Clients.All.SendAsync("simulationStarted");
                    break;
                case SimulationState.Stopped:
                    await Hub.Clients.All.SendAsync("simulationStopped");
                    break;
                default:
                    break;
            }
        }

        private void LoadFMU()
        {
        }
    }

    public enum SimulationState
    {
        Started,
        Stopped
    }
}
