namespace SocialSimulation.Entity
{
    public class Social : NotifierBase
    {
        private float _currentSocialLatency;
        private float _socialLatencyThreshold;
        public float NeedForSociability { get; set; }

        public float Charisma { get; set; }

        public float SocialLatencyThreshold
        {
            get => _socialLatencyThreshold;
            set { _socialLatencyThreshold = value;OnPropertyChanged(); }
        }

        public float CurrentSocialLatency
        {
            get => _currentSocialLatency;
            set { _currentSocialLatency = value;OnPropertyChanged(); }
        }

        public float SocialLatencyRecoveryRate { get; set; }
    }
}