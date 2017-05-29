namespace GameEngine.Components
{
    public enum ComponentTypes {
        COMPONENT_NONE = 0,
	    COMPONENT_TRANSFORM = 1 << 0,
        COMPONENT_GEOMETRY = 1 << 1,
        COMPONENT_TEXTURE  = 1 << 2,
        COMPONENT_VELOCITY = 1 << 3,
        COMPONENT_MOTION = 1 << 4,
        COMPONENT_AUDIO = 1 << 5,
        COMPONENT_AUDIO_SOURCE = 1 << 6,
        COMPONENT_RENDER = 1 << 7,
        COMPONENT_CAMERA = 1 << 8,
        COMPONENT_INPUT = 1 << 9,
        COMPONENT_COLLISION_UI = 1 << 10,
        COMPONENT_UI_SCREEN = 1 << 11,
        COMPONENT_LIGHT_POINT = 1 << 12,
        COMPONENT_UI_TEXT = 1 << 13,
        COMPONENT_GHOST = 1 << 14,
        COMPONENT_MINIMAP = 1 << 15,
        COMPONENT_BOX_COLLIDER = 1 << 16,
        COMPONENT_SPHERE_COLLIDER = 1 << 17,
        COMPONENT_MINIMAP_TEXTURE = 1 << 18
    }

    public interface IComponent
    {
        ComponentTypes ComponentType
        {
            get;
        }
    }
}
