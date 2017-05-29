using OpenTK;
using GameEngine.Components;

namespace OpenGL_Game.Objects
{
    /* Author: Alex DS */
    class Wall : PrimitiveGeometry
    {
        private ComponentAudio compAudio;
        private ComponentBoxCollider boxCollider;

        /* Author: Alex DS */
        public Wall(Vector3 pos)
                                    // pos + offset(offset relative to scale) rotation             scale
            : base("HorizontalWall", pos + new Vector3(0,2f,0), new Vector3(0, 0, 0), new Vector3(3, 3,3), "Cube", "Wall.png")
        {
            base.AddComponent(boxCollider = new ComponentBoxCollider(this, pos, this.Scale / 1.1f));
            base.AddComponent(new ComponentMinimapTexture("Textures/Wall.png", 2,1,1));
            boxCollider.sCollided = Collided;
        }

        public void Collided(ComponentSphereCollider other)
        {

        }
    }
}