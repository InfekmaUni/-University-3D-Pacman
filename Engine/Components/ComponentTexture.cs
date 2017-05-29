using GameEngine.Managers;

namespace GameEngine.Components
{
    public class ComponentTexture : IComponent
    {
        int texture;
        int scale;
        float transparency;

        public ComponentTexture(string textureName)
        {
            texture = ResourceManager.LoadTexture(textureName);
            scale = 1;
        }
        public ComponentTexture(string textureName, int inScale)
        {
            texture = ResourceManager.LoadTexture(textureName);
            scale = inScale;
        }
        public ComponentTexture(string textureName, int inScale, float inTransparency)
        {
            texture = ResourceManager.LoadTexture(textureName);
            scale = inScale;
            transparency = inTransparency;
        }
        /* Author: Alex DS */
        public void ChangeTexture(string textureName)
        {
            texture = ResourceManager.LoadTexture(textureName);
        }
        public int Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public float Transparency
        {
            get { return transparency; }
            set { transparency = value; }
        }
        public int Texture
        {
            get { return texture; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_TEXTURE; }
        }
    }
}
