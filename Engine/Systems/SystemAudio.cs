using OpenTK;
using GameEngine.Components;
using GameEngine.Objects;
using OpenTK.Audio.OpenAL;
using System.Collections.Generic;

namespace GameEngine.Systems
{
    /* Author: Alex DS */
    public class SystemAudio : BaseSystem, IUpdateableSystem
    {
        const ComponentTypes AUDIO_MASK = (ComponentTypes.COMPONENT_AUDIO | ComponentTypes.COMPONENT_TRANSFORM);
        const ComponentTypes SOURCE_MASK = (ComponentTypes.COMPONENT_AUDIO_SOURCE | ComponentTypes.COMPONENT_TRANSFORM);

        private bool paused = false;
        // OpenAL listeners
        private Vector3 listenerPosition = new Vector3(0, 0, 0);
        private Vector3 listenerDirection = new Vector3(0, 0, 0);
        private Vector3 listenerUp = Vector3.UnitY;
        /* Author: Alex DS*/
        public bool Paused
        {
            get { return paused; }
            set { paused = value; }
        }

        List<Entity> audioEntities = new List<Entity>();
        List<Entity> audioSourceEntities = new List<Entity>();
        public SystemAudio(List<Entity> entityList) : base("SystemAudio")
        {
            UpdateListeners(new Vector3(0, 0, 0));
            ProcessEntityMask(entityList, audioEntities, AUDIO_MASK);
            ProcessEntityMask(entityList, audioSourceEntities, SOURCE_MASK);
        }

        /* Author: Alex DS */
        public void OnUpdate(float dt)
        {
            audioEntities.RemoveAll(en => en.Dispose == true);
            audioSourceEntities.RemoveAll(en => en.Dispose == true);

            // for all entities in list
            foreach (Entity ent in audioEntities)
            {
                ComponentAudio audioComp = (ComponentAudio)ent.GetComponent(ComponentTypes.COMPONENT_AUDIO);
                ComponentTransform transformComp = (ComponentTransform)ent.GetComponent(ComponentTypes.COMPONENT_TRANSFORM);

                foreach (AudioInfo inf in audioComp.audioList)
                {
                    audioComp.UpdatePosition(inf, transformComp.Position);
                }
            }

            foreach (Entity ent in audioSourceEntities)
            {
                ComponentTransform transformComp = (ComponentTransform)ent.GetComponent(ComponentTypes.COMPONENT_TRANSFORM);
                UpdateListeners(-transformComp.Position, transformComp.Forward, -transformComp.Up);
            }
        }

        /* Author: Alex DS */
        public void PauseSounds(bool pause)
        {
            paused = pause;
        }
        /* Author: Alex DS*/
        private void UpdateListeners()
        {
            AL.Listener(ALListener3f.Position, ref listenerPosition);
            AL.Listener(ALListenerfv.Orientation, ref listenerDirection, ref listenerUp);
        }
        /* Author: Alex DS*/
        private void UpdateListeners(Vector3 pos)
        {
            listenerPosition = -pos;
            UpdateListeners();
        }
        /* Author: Alex DS*/
        private void UpdateListeners(Vector3 pos, Vector3 dir)
        {
            listenerPosition = -pos;
            listenerDirection = dir;
            UpdateListeners();
        }
        /* Author: Alex DS*/
        private void UpdateListeners(Vector3 pos, Vector3 dir, Vector3 up)
        {
            listenerPosition = -pos;
            listenerDirection = dir;
            listenerUp = up;
            UpdateListeners();
        }
    }
}
