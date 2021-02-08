using SocialSimulation.Collisions;
using SocialSimulation.Core;
using SocialSimulation.Entity;
using SocialSimulation.Environment;
using SocialSimulation.Interactions;
using SocialSimulation.Movement;
using SocialSimulation.SimulationParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SocialSimulation.Game
{
    public class MyGame : NotifierBase, IGame
    {
        private readonly MovementService _movement;
        private readonly CollisionService _collisions;
        private readonly GlobalSimulationParameters _simParams;
        private readonly EnvironmentService _environmentService;
        private readonly Logger _logger;
        private readonly InteractionService _interactService;
        private readonly IGameSurface _surface;
        private List<Entity.Entity> _entities;
        private readonly Random _rnd;
        //private readonly object _entitiesLock = new object();

        public List<Entity.Entity> Entities
        {
            get => _entities;
            private set { _entities = value; OnPropertyChanged(); }
        }

        public MyGame(
            MovementService movement,
            CollisionService collisions,
            GlobalSimulationParameters simParams,
            EnvironmentService environmentService,
            Logger logger,
            InteractionService interactService, IGameSurface surface)
        {
            _movement = movement;
            _collisions = collisions;
            _simParams = simParams;
            _environmentService = environmentService;
            _logger = logger;
            _interactService = interactService;
            _surface = surface;

            //_simParams.PropertyChanged += SimulationParamsOnPropertyChanged;
            _rnd = new Random(DateTime.Now.Millisecond);
            Entities = new List<Entity.Entity>();
        }

        private void ChangeDetermination()
        {
            //lock (_entitiesLock)
            {
                foreach (var entity in Entities)
                {
                    //_logger.Log($"Changing {nameof(_simParams.PersonalSpace)} to {_simParams.PersonalSpace}");
                    entity.PersonalSpace.Size = _simParams.PersonalSpace;
                }
            }
        }

        private void ChangeAudacity()
        {
            //lock (_entitiesLock)
            {
                foreach (var entity in Entities)
                {
                    //_logger.Log($"Changing {nameof(_simParams.Audacity)} to {_simParams.Audacity}");
                    entity.Audacity = _simParams.Audacity;
                }
            }
        }

        private void ChangeSpeed()
        {
            //lock (_entitiesLock)
            {
                foreach (var entity in Entities)
                {
                    //_logger.Log($"Changing {nameof(_simParams.Speed)} to {_simParams.Speed}");
                    entity.Movement.Speed = _simParams.Speed;
                }
            }
        }

        //private void SimulationParamsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == nameof(GlobalSimulationParameters.Speed))
        //        ChangeSpeed();

        //    if (e.PropertyName == nameof(GlobalSimulationParameters.PersonalSpace))
        //        ChangeDetermination();

        //    if (e.PropertyName == nameof(GlobalSimulationParameters.Audacity))
        //        ChangeAudacity();
        //}

        public void Load()
        {

            UiRenderer.Render(() => _surface.Surface.Children.Clear());
            _entityRenderCache.Clear();
            //lock (_entitiesLock)
            {
                Entities.Clear();
                var ets = new List<Entity.Entity>();
                for (int i = 0; i < _simParams.UnitsNumber; i++)
                {
                    var e = new Entity.Entity
                    { Id = i + 1, Audacity = _simParams.Audacity, SelfSize = _simParams.EntitySize };
                    e.Movement.Speed = _simParams.Speed;
                    var x = _rnd.Next(e.SelfSize, _simParams.SurfaceWidth - e.SelfSize) - e.SelfSize / 2;

                    var y = _rnd.Next(e.SelfSize, _simParams.SurfaceHeight - e.SelfSize) - e.SelfSize / 2;

                    e.Movement.Direction = (StartDirection)_rnd.Next(0, 4);
                    e.Position = new Vector2(x, y);

                    e.Social.Charisma = (float)_rnd.NextDouble();
                    e.Social.NeedForSociability = (float)_rnd.NextDouble();
                    e.Social.SocialLatencyThreshold = _rnd.Next(20, 80);
                    e.Social.CurrentSocialLatency = e.Social.SocialLatencyThreshold;
                    e.Social.SocialLatencyRecoveryRate = 0.005f;

                    e.PersonalSpace.Size = _simParams.PersonalSpace;
                    MoveBehavior.UpdatePersonalSpace(e);

                    ets.Add(e);
                }

                Entities = ets;
                Entities.ForEach(e => e.State = EntityState.Moving);
            }
        }

        public void Update(float elapsed)
        {
            //lock (_entitiesLock)
            {
                _surface.Update();
                ChangeAudacity();
                ChangeDetermination();
                ChangeSpeed();
                _environmentService.UpdateTime(elapsed);
                foreach (var entity in Entities)
                {
                    entity.SelfSize = _simParams.EntitySize;
                }

                var copy = Entities.ToList();
                foreach (var e in Entities)
                {
                    _movement.Update(e, elapsed);
                    _interactService.UpdateSocialLatency(e, elapsed);

                }
                //Parallel.ForEach(Entities, e =>
                //{
                //});

                foreach (var entity in Entities)
                {
                    _collisions.ComputeCollision(entity, copy, elapsed);
                }

                foreach (var entity in Entities.Where(e => e.CollidingEntities.Any()))
                {
                    entity.CollidingEntities.ForEach((e) => _interactService.Interact(entity, e, _rnd));
                }
            }
        }

        public void Render(float elapsed)
        {
            RenderEntities(elapsed);
        }

        private void RenderEntities(float elapsed)
        {
            UiRenderer.Render(() =>
            {
                //_surface.Surface.Children.Clear();
                //lock (_entitiesLock)
                {
                    foreach (var entity in Entities)
                    {
                        if (!_entityRenderCache.TryGetValue(entity.Id, out var renderData))
                        {
                            renderData = new EntityRender();
                            _entityRenderCache[entity.Id] = renderData;
                            RenderEntity(entity, renderData, elapsed);
                        }
                        else
                        {
                            UpdateEntitySprites(entity, renderData, elapsed);
                        }
                    }
                }
            });
        }

        private void UpdateEntitySprites(Entity.Entity entity, EntityRender renderData, float elapsed)
        {
            Canvas.SetTop(renderData.Entity, entity.Position.Y * elapsed - (float)entity.SelfSize / 2);
            Canvas.SetLeft(renderData.Entity, entity.Position.X * elapsed - (float)entity.SelfSize / 2);
            renderData.Entity.Height = entity.SelfSize;
            renderData.Entity.Width = entity.SelfSize;

            Canvas.SetTop(renderData.PersonalSpace, entity.PersonalSpace.Origin.Y * elapsed);
            Canvas.SetLeft(renderData.PersonalSpace, entity.PersonalSpace.Origin.X * elapsed);
            renderData.PersonalSpace.Background = EntityRenderResources.StateToBrush[entity.State];
            renderData.PersonalSpace.Width = entity.PersonalSpace.Size;
            renderData.PersonalSpace.Height = entity.PersonalSpace.Size;

        }


        private Dictionary<int, EntityRender> _entityRenderCache = new Dictionary<int, EntityRender>();

        private void RenderEntity(Entity.Entity entity, EntityRender entityRender, float elapsed)
        {
            //order is important here : latest visuals added are displayed on foreground
            RenderPersonalSpace(entity, entityRender, elapsed);

            Ellipse entitySprite = new Ellipse { Height = entity.SelfSize, Width = entity.SelfSize, Fill = EntityRenderResources.EntityBrush, Tag = SpriteType.Entity };
            _surface.Surface.Children.Add(entitySprite);
            Canvas.SetTop(entitySprite, entity.Position.Y * elapsed - (float)entity.SelfSize / 2);
            Canvas.SetLeft(entitySprite, entity.Position.X * elapsed - (float)entity.SelfSize / 2);
            entityRender.Entity = entitySprite;

        }

        private void RenderPersonalSpace(Entity.Entity entity, EntityRender entityRender, float elapsed)
        {
            Border personalSpaceSprite = new Border
            {
                Background = EntityRenderResources.StateToBrush[entity.State],
                BorderBrush = EntityRenderResources.PersonalSpaceBorder,
                BorderThickness = new Thickness(0.5),
                Width = entity.PersonalSpace.Size,
                Height = entity.PersonalSpace.Size,
                Tag = SpriteType.PersonalSpace
            };
            _surface.Surface.Children.Add(personalSpaceSprite);

            Canvas.SetTop(personalSpaceSprite, entity.PersonalSpace.Origin.Y* elapsed);
            Canvas.SetLeft(personalSpaceSprite, entity.PersonalSpace.Origin.X* elapsed);
            entityRender.PersonalSpace = personalSpaceSprite;
        }

        public void Unload()
        {
        }

        public void Input()
        {
            UiRenderer.Render(() =>
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    var currentPointer = Mouse.GetPosition(_surface.Surface);

                    if (currentPointer.X < 0 || currentPointer.Y < 0 || currentPointer.X > _surface.Surface.Width || currentPointer.Y > _surface.Surface.Height)
                        return;

                    //lock (_entitiesLock)
                    {
                        foreach (var entity in Entities)
                        {
                            var addedGoal = new Goal
                            { GoalPosition = new Vector2((float)currentPointer.X, (float)currentPointer.Y) };
                            entity.Goals.Add(addedGoal);
                            if (entity.CurrentGoal == null)
                            {
                                entity.CurrentGoal = addedGoal;
                            }

                            entity.MovementType = MovementType.TowardGoal;
                        }
                    }
                }
            });
        }
    }

    public static class EntityRenderResources
    {
        public static Dictionary<EntityState, SolidColorBrush> StateToBrush { get; } =
            new Dictionary<EntityState, SolidColorBrush>()
            {
                {EntityState.Talking, new SolidColorBrush(Colors.Beige)},
                {EntityState.Moving, new SolidColorBrush(Colors.Transparent)},
                {EntityState.Idle, new SolidColorBrush(Colors.Transparent)},
            };

        public static SolidColorBrush EntityBrush { get; } = new SolidColorBrush(Colors.Black);
        public static Brush PersonalSpaceBorder { get; set; } = new SolidColorBrush(Colors.Gray);
    }

    public class EntityRender
    {
        public Border PersonalSpace { get; set; }
        public Ellipse Entity { get; set; }
    }
    public enum SpriteType
    {
        Entity,
        PersonalSpace
    }
}