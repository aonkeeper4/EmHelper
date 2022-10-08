using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.EmHelper.Entities
{
    class NoCollider : Collider //false hitbox to not make the walkeline grabbable
    {
        public override float Width { get; set; }
        public override float Height { get; set; }
        public override float Top { get; set; }
        public override float Bottom { get; set; }
        public override float Left { get; set; }
        public override float Right { get; set; }

        public override Collider Clone()
        {
            return new NoCollider();
        }

        public override bool Collide(Vector2 point)
        {
            return false;
        }

        public override bool Collide(Rectangle rect)
        {
            return false;
        }

        public override bool Collide(Vector2 from, Vector2 to)
        {
            return false;
        }

        public override bool Collide(Hitbox hitbox)
        {
            return false;
        }

        public override bool Collide(Grid grid)
        {
            return false;
        }

        public override bool Collide(Circle circle)
        {
            return false;
        }

        public override bool Collide(ColliderList list)
        {
            return false;
        }

        public override void Render(Camera camera, Color color)
        {

        }
    }
}
