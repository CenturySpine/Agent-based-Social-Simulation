using System.Collections.Generic;
using System.Linq;

namespace SocialSimulation
{
    public class CollisionService
    {
        public void ComputeCollision(Entity entity, List<Entity> copy)
        {
            var collidingOthers =
                copy.Where(c => c != entity)
                    .Where(o => Collide(entity.Bound, o.Bound))
                    .ToList();

            entity.CollidingEntities = collidingOthers;
        }

        private bool Collide(BoundBox rect1, BoundBox rect2)
        {
            return rect1.X < rect2.X + rect2.Width &&
                   rect1.X + rect1.Width > rect2.X &&
                   rect1.Y < rect2.Y + rect2.Height &&
                   rect1.Y + rect1.Height > rect2.Y;
            //float d1x = b.min.X - a.max.X;
            //float d1y = b.min.Y - a.max.Y;
            //float d2x = a.min.X - b.max.X;
            //float d2y = a.min.Y - b.max.Y;

            //if (d1x > 0.0f || d1y > 0.0f)
            //    return false;

            //if (d2x > 0.0f || d2y > 0.0f)
            //    return false;

            //return true;
        }
    }
}