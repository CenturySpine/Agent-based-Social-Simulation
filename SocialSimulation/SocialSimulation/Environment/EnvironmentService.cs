using SocialSimulation.Core;
using SocialSimulation.SimulationParameters;
using System;

namespace SocialSimulation.Environment
{
    public class EnvironmentService
    {
        private readonly GlobalSimulationParameters _simParams;
        private float _passed;

        public EnvironmentService(GlobalSimulationParameters simParams)
        {
            _simParams = simParams;
            ViewModel = new EnvironmentViewModel();
        }

        public void UpdateTime(float elapsed)
        {
            _passed += elapsed;
            int hour = (int)Math.Round(_passed / (_simParams.SecondsToHour * 1000));
            ViewModel.TimeOfDay = hour;
            if (hour == 12)
                _passed = 0;
        }

        public EnvironmentViewModel ViewModel { get; set; }
    }

    public class EnvironmentViewModel : NotifierBase
    {
        private int _timeOfDay;

        public int TimeOfDay
        {
            get => _timeOfDay;
            set { _timeOfDay = value; OnPropertyChanged(); }
        }
    }
}