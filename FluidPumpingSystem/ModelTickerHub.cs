using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace FluidPumpingSystem
{
    public class ModelTickerHub : Hub
    {
        private readonly ModelTicker _modelTicker;

        public ModelTickerHub(ModelTicker modelTicker) => _modelTicker = modelTicker;

        public async Task StartSimulation()
        {
            await _modelTicker.StartSimulation();
        }
        public async Task StopSimulation()
        {
            await _modelTicker.StopSimulation();
        }
    }
}
