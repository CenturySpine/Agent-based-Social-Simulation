using SocialSimulation.Core;
using SocialSimulation.Environment;
using SocialSimulation.Game;
using SocialSimulation.GameLoop;
using SocialSimulation.SimulationParameters;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace SocialSimulation
{
    public class MainViewModel : NotifierBase
    {
        public GlobalSimulationParameters SimulationParams { get; }
        public EnvironmentService Environment { get; }
        public IGameSurface Surface { get; }
        private readonly Logger _logger;
        private readonly IGame _game;
        private readonly IGameLoop _gameLoop;

        public MainViewModel(
            GlobalSimulationParameters simulationParams,
            Logger logger,
            EnvironmentService environment,
            IGame game,
            IGameLoop gameLoop,
            IGameSurface surface)
        {
            SimulationParams = simulationParams;
            Environment = environment;
            Surface = surface;
            _logger = logger;
            _game = game;
            _gameLoop = gameLoop;

            GenerateCommand = new RelayCommand(ExecuteLoad);
            StartMoveCommand = new RelayCommand(ExecuteStart);
            StopMoveCommand = new RelayCommand(ExecuteStopMove);

            Logs = new ObservableCollection<string>();

            _logger.RegisterListener(InternalLog);
        }

        public ObservableCollection<string> Logs { get; set; }

        private void InternalLog(string obj)
        {
            //UiRenderer.Render(() =>
            //{
            //    Logs.Insert(0, obj);
            //});
        }

        public RelayCommand GenerateCommand { get; }

        public RelayCommand StartMoveCommand { get; }

        public RelayCommand StopMoveCommand { get; }

        private async void ExecuteLoad(object o)
        {
            await Task.Run(() =>
            {
                _game.Load();
                Surface.Update();
                _game.Render(SimLoopData.Elapsed);
                _logger.Log($"Generation completed");
            });
        }

        private void ExecuteStart(object o)
        {
            _logger.Log($"Starting");

            _gameLoop.Start(_game);
        }

        private void ExecuteStopMove(object o)
        {
            _logger.Log($"Stopping movement");
            _gameLoop.Stop(_game);
        }

        public void Unload()
        {
            _gameLoop.Stop(_game);
        }
    }
}