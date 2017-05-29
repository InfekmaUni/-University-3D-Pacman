using GameEngine.Systems;

namespace GameEngine.Components
{
    /* Author: Alex DS */
    public class ComponentPointLight : IComponent
    {
        private PointLight light;
        public PointLight Light
        {
            get { return light; }
        }
        /* Author: Alex DS */
        public ComponentPointLight(float constant, float linear, float quadratic, float intensity)
        {
            light = new PointLight();
            light.Init(constant, linear, quadratic, intensity);
        }

        // constructor which creates a default light
        public ComponentPointLight()
        {
            light = new PointLight();
            light.Default();
        }
        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_LIGHT_POINT; }
        }
    }
}
