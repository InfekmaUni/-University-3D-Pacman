using OpenTK;
using GameEngine.Components;
using GameEngine.Objects;

namespace OpenGL_Game.Objects
{
    /* Author: Alex DS */
    class EnergyBall : PrimitiveGeometry
    {
        private ComponentAudio compAudio;
        private ComponentSphereCollider sphereCollider;
        /* Author: Alex DS */
        public bool pickedUp = false;
        public EnergyBall(Vector3 pos)
            : base("EnergyBall", pos, new Vector3(0, 0, 0), new Vector3(0.6f , 0.6f, 0.6f), "Sphere", "energyBall.png")
        {
            base.AddComponent(new ComponentMotion(new Vector3(0, 0.1f, 0)));
            base.AddComponent(sphereCollider = new ComponentSphereCollider((Entity)this, this.Scale));
            base.AddComponent(new ComponentMinimapTexture("Textures/energyBall.png", 1,1,1));
            sphereCollider.sCollided = Collided;
        }

        /* Author: Alex DS & Adam */
        public bool collided = false;
        public void Collided(ComponentSphereCollider other)
        {
            if (other.Entity.Name.Contains("Pacman"))
            {
                collided = true;
            }
        }
    }
}