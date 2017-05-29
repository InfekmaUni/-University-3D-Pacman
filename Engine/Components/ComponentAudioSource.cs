namespace GameEngine.Components
{
    /* Author: Alex DS */
    public class ComponentAudioSource : IComponent
    {
        private bool active;
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        /* Author: Alex DS */
        public ComponentAudioSource(bool inActive = true)
        {
            active = inActive;
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_AUDIO_SOURCE; }
        }
    }
}
