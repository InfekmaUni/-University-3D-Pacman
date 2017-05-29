using GameEngine.Objects;
using OpenTK;

namespace GameEngine.Components
{
    public class ComponentBoxCollider : IComponent
    {
        public delegate void SphereCollided(ComponentSphereCollider other);
        public delegate void BoxCollided(ComponentBoxCollider other);
        public BoxCollided bCollided;
        public SphereCollided sCollided;

        private Entity entity;
        public Entity Entity
        {
            get { return entity; }
        }
        public Vector3 startPoint;
        public Vector3 endPoint;
        public struct cornerCoords
        {
            public Vector2 corn1;
            public Vector2 corn2;
            public Vector2 corn3;
            public Vector2 corn4;
        }
        public cornerCoords corners;

        public ComponentBoxCollider(Entity ent, Vector3 pos, Vector3 scale)
        {
            entity = ent;
            corners = new cornerCoords();
            corners.corn1 = new Vector2(pos.X + scale.X, pos.Z + scale.Z);
            corners.corn2 = new Vector2(pos.X + scale.X, pos.Z - scale.Z);
            corners.corn3 = new Vector2(pos.X - scale.X, pos.Z + scale.Z);
            corners.corn4 = new Vector2(pos.X - scale.X, pos.Z - scale.Z);
        }

        /// <summary>
        /// When object collides with another object this will be called
        /// </summary>
        /// <param name="other">other collider</param>
        public void OnCollisionEnter(ComponentSphereCollider other)
        {
            if (sCollided != null)
                sCollided(other);
        }

        public void OnCollisionEnter(ComponentBoxCollider other)
        {
            if (bCollided != null)
                bCollided(other);
        }

        public ComponentTypes ComponentType
        {
            get
            {
                return ComponentTypes.COMPONENT_BOX_COLLIDER;
            }
        }
    }
}
