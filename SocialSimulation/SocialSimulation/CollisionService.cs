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
            return rect1.x < rect2.x + rect2.width &&
                   rect1.x + rect1.width > rect2.x &&
                   rect1.y < rect2.y + rect2.height &&
                   rect1.y + rect1.height > rect2.y;
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