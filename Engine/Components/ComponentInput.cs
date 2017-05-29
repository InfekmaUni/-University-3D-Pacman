namespace GameEngine.Components
{
    /* Author: Alex DS */
    public class ComponentInput : IComponent
    {
        /* Author: Alex DS */
        public ComponentInput()
        {
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_INPUT; }
        }
    }
}
