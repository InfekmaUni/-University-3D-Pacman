using GameEngine.Objects;
using OpenTK;

namespace GameEngine.Components
{
    /* Author: Alex DS */
    public class ComponentSphereCollider : IComponent
    {
        public delegate void SphereCollided(ComponentSphereCollider other);
        public delegate void BoxCollided(ComponentBoxCollider other);
        public SphereCollided sCollided;
        public BoxCollided bCollided;
        private Entity entity;
        public Entity Entity
        {
            get { return entity; }
        }
        private Vector3 radius;
        public Vector3 Radius
        {
            get { return radius; }
            set { radius = value; }
        }
        private bool trigger = false;
        public bool Trigger
        {
            get { return trigger; }
            set { trigger = value; }
        }
        public ComponentSphereCollider(Entity ent, Vector3 inRadius, bool inTrigger = false)
        {
            radius = inRadius;
            trigger = inTrigger;
            entity = ent;
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
                return ComponentTypes.COMPONENT_SPHERE_COLLIDER;
            }
        }
    }
}
