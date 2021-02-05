using System.Collections.Generic;
using System.Linq;

namespace SocialSimulation
{
    public class CollisionService
    {
        public void ComputeCollision(Entity.Entity entity, List<Entity.Entity> copy)
        {
            var collidingOthers =
                copy.Where(c => c != entity)
                    .Where(o => Collide(entity.PersonalSpace.Bound, o.PersonalSpace.Bound))
                    .ToList();

            entity.CollidingEntities = collidingOthers;
        }

        private bool Collide(BoundBox rect1, BoundBox rect2)
        {
            return rect1 != null && rect2 != null && 
                   rect1.X < rect2.X + rect2.Width &&
                   rect1.X + rect1.Width > rect2.X &&
                   rect1.Y < rect2.Y + rect2.Height &&
                   rect1.Y + rect1.Height > rect2.Y;
        }
    }
}