using System;
using System.Threading;

namespace SocialSimulation
{
    public class Interaction
    {
        public Guid InteractionId { get; } = Guid.NewGuid();
        private Timer _timer;
        private bool _interactionStarted;

        public event InteractionTerminatedEventHandler InteractionEnded;

        public Interaction(Entity entitySource, Entity entityTarget, float interactionIntensity)
        {
            Entity1 = entitySource;
            Entity2 = entityTarget;
            var duration = (int)Math.Round(interactionIntensity * 10, MidpointRounding.AwayFromZero);
            if (duration < 2) duration = 2;
            _timer = new Timer(InteractionEnd, duration, duration * 1000, Timeout.Infinite);
            _interactionStarted = true;
        }

        private void InteractionEnd(object state)
        {
            if (!_interactionStarted)
                return;

            _interactionStarted = false;
            _timer?.Dispose();
            _timer = null;
            OnInteractionEnded();
        }

        public Entity Entity1 { get; }
        public Entity Entity2 { get; }

        protected virtual void OnInteractionEnded()
        {
            InteractionEnded?.Invoke(this);
        }
    }
}