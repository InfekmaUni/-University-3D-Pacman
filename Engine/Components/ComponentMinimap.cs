using OpenTK;

namespace GameEngine.Components
{
    /* Author: Alex DS */
    public class ComponentMinimap : IComponent
    {
        private float minimapSize;
        private Vector2 offset;

        public float Size
        {
            get { return minimapSize; }
            set { minimapSize = value; }
        }
        public Vector2 Offset
        {
            get { return offset; }
            set { offset = value; }
        }
        public ComponentMinimap(float size, Vector2 inOffset)
        {
            minimapSize = size;
            offset = inOffset;
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_MINIMAP; }
        }
    }
}