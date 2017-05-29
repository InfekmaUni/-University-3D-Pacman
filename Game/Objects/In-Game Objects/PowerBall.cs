using OpenTK;
using GameEngine.Components;
using GameEngine.Objects;

namespace OpenGL_Game.Objects
{
    /* Author: Alex DS */
    class PowerBall : PrimitiveGeometry
    {
        private ComponentAudio compAudio;
        private ComponentSphereCollider sphereCollider;

        /* Author: Alex DS */
        public PowerBall(Vector3 pos)
            : base("PowerBall", pos, new Vector3(0,0,0),new Vector3(1,1,1),"Sphere", "powerBall.png")
        {
            base.AddComponent(new ComponentMotion(new Vector3(0, 0.1f, 0)));
            base.AddComponent(compAudio = new ComponentAudio("power_ambient.wav", true, true));
            base.AddComponent(sphereCollider = new ComponentSphereCollider((Entity)this, this.Scale));
            base.AddComponent(new ComponentMinimapTexture("Textures/powerBall.png", 2,1,1));
            sphereCollider.sCollided = Collided;
        }

        /* Author: Alex DS & Adam */
        public bool collided = false;
        public void Collided(ComponentSphereCollider other)
        {
           if (other.Entity.Name.Contains("Pacman"))
            {
                collided = true;
                compAudio.Play(false);
                /* toggle pacman ghost eating frenzy */
            }
        }
    }
}
