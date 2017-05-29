using OpenTK;

namespace GameEngine.Components
{
    /* Author: Alex DS */
    public class ComponentUIScreen : IComponent
    {
        private Vector2 screenSize;
        private Matrix4 projection;

        public Matrix4 Projection
        {
            get { return projection; }
            set { projection = value; }
        }
        public float Width
        {
            get { return screenSize.X; }
            set { screenSize.X = value; }
        }
        public float Height
        {
            get { return screenSize.Y; }
            set { screenSize.Y = value; }
        }
        public Vector2 Size
        {
            get { return screenSize; }
            set { screenSize = value; }
        }
        private void CreateUiProjection()
        {
            projection = Matrix4.CreateOrthographicOffCenter(0, screenSize.X, screenSize.Y, 0, -1.0f, +1.0f);
        }
        public ComponentUIScreen(Vector2 size)
        {
            screenSize = size;
            CreateUiProjection();
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_UI_SCREEN; }
        }
    }
}