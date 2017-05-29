using OpenTK;
using GameEngine.Components;
using GameEngine.Objects;

namespace OpenGL_Game.Objects
{
    /* Author: Alex DS */
    class Sound : Entity
    {
        private ComponentAudio compAudio;
        /* Author: Alex DS */
        public Sound(string soundName, bool loop, bool active)
            : base("Audio")
        {
            base.AddComponent(compAudio = new ComponentAudio(soundName, loop, active));
            base.AddComponent(new ComponentTransform(new Vector3(0, 0, 0), new Vector3(0, 0, 0)));
        }
    }
}
