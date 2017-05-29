using OpenTK;
using GameEngine.Components;
using GameEngine.Objects;

namespace OpenGL_Game.Objects
{
    /* Author: Alex DS */
    class UIScreen : Entity
    {
        /* Author: Alex DS */
        public UIScreen(Vector2 size, bool minimap = false)
            : base("Screen")
        {
            base.AddComponent(new ComponentUIScreen(size));
            if (minimap)
            {
              //  base.AddComponent(new ComponentCamera(new Vector3(0, 0, 0), CameraState.Minimap));
                base.AddComponent(new ComponentMinimap(1.5f, new Vector2(560, 30)));
                base.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
                base.AddComponent(new ComponentRender("vs_UI", "fs_UI"));
            }
        }
    }
}
