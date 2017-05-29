using GameEngine.Managers;

namespace GameEngine.Components
{
    public class ComponentMinimapTexture : IComponent
    {
        int texture;
        float scale;
        float transparency;
        int zIndex = 0;
        public int Index { get { return zIndex; } }
        public ComponentMinimapTexture(string textureName)
        {
            texture = ResourceManager.LoadTexture(textureName);
            scale = 1;
        }
        public ComponentMinimapTexture(string textureName, float inScale)
        {
            texture = ResourceManager.LoadTexture(textureName);
            scale = inScale;
        }
        public ComponentMinimapTexture(string textureName, float inScale, float inTransparency, int inZIndex)
        {
            texture = ResourceManager.LoadTexture(textureName);
            scale = inScale;
            transparency = inTransparency;
            zIndex = inZIndex;
        }
        /* Author: Alex DS */
        public void ChangeTexture(string textureName)
        {
            texture = ResourceManager.LoadTexture(textureName);
        }
        public float Scale
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
            get { return ComponentTypes.COMPONENT_MINIMAP_TEXTURE; }
        }
    }
}
