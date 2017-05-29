using OpenTK;
using GameEngine.Components;
using GameEngine.Objects;

namespace OpenGL_Game.Objects
{
    /* Author: Alex DS */
    class RenderedObject : Entity
    {
        protected ComponentTransform posComp;
        protected ComponentTexture textComp;
        /* Author: Alex DS */
        protected Vector3 Position
        {
            get { return posComp.Position;  }
            set { posComp.Position = value; }
        }

        /* Author: Alex DS */
        protected Vector3 Rotation
        {
            get { return posComp.Rotation; }
            set { posComp.Rotation = value; }
        }

        /* Author: Alex DS */
        protected Vector3 Scale
        {
            get { return posComp.Scale; }
            set { posComp.Scale = value; }
        }

        /* Author: Alex DS */
        protected int TextureScale
        {
            get { return textComp.Scale; }
            set { textComp.Scale = value; }
        }
        /* Author: Alex DS */
        public RenderedObject(string name, Vector3 pos, Vector3 rot, Vector3 scale, string geoPath, string textPath)
            : base(name)
        {
            // minimum requirements to render an object onto the screen involves these three components
            base.AddComponent(posComp = new ComponentTransform(pos, rot, scale));
            base.AddComponent(new ComponentGeometry("Geometry/" + geoPath));
            base.AddComponent(textComp = new ComponentTexture("Textures/" + textPath));
            base.AddComponent(new ComponentRender("vs", "fs"));
         //   base.AddComponent(new ComponentVelocity(new Vector3(0.5f, 0, 0)));
        }

    }
}
