using OpenTK;

namespace GameEngine.Components
{
    /* Author: Alex DS */
    public class ComponentUICollision : IComponent
    {
        private Vector2 size;
        public Vector2 Size
        {
            get { return size; }
        }
        /* Author: Alex DS */
        public ComponentUICollision(Vector2 inSize)
        {
            size = inSize;
        }
        /* Author: Alex DS */
        public delegate void OnHover();
        public delegate void OnHoverExit();
        public delegate void OnClick();
        public OnClick clicked;
        public OnHover hover;
        public OnHoverExit hoverExit;
        public void OnHoveredExit(){
            if(hoverExit != null)
                hoverExit();
        }
        public void OnHovered()
        {
            if(hover != null )
                hover();
        }
        public void OnClicked()
        {
            if (clicked != null)
                clicked();
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_COLLISION_UI; }
        }
    }
}
