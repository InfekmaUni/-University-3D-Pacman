using OpenTK;
using GameEngine.Components;
using GameEngine.Objects;

namespace OpenGL_Game.Objects
{
    /* Author: Alex DS */
    class Image : Entity
    {
        protected ComponentTexture texture;
        protected ComponentGeometry geometry;
        protected ComponentTransform transform;
        protected ComponentRender render;
        public int TextScale
        {
            set { texture.Scale = value; }
        }
        /* Author: Alex DS */
        public Image(string name, Vector2 pos, Vector2 size, string text)
            : base(name)
        {
            // minimum requirements to render an object onto the screen involves these three components
            base.AddComponent(transform = new ComponentTransform(new Vector3(pos.X, pos.Y, 0), new Vector3(0, 0, 0), new Vector3(size.X, size.Y, 0)));
            base.AddComponent(geometry = new ComponentGeometry("Geometry/SquareGeometry.txt"));
            base.AddComponent(texture = new ComponentTexture("Textures/UI/" + text + ".png"));
            base.AddComponent(render = new ComponentRender("vs_UI", "fs_UI"));
        }
    }
}
