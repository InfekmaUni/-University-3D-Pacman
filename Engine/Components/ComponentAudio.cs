using GameEngine.Managers;
using OpenTK;
using OpenTK.Audio.OpenAL;
using System.Collections.Generic;

namespace GameEngine.Components
{
    /* Author: Alex DS */
    public struct AudioInfo
    {
        public int ID;
        public int audio;
        public int audioSource;
        public bool paused;
        public bool looping;
        public string audioPath;
        public AudioInfo(int AUDIOID, string inAudioPath, int inAudio, int inAudioSource, bool inPause, bool inLoop)
        {
            audioPath = inAudioPath;
            audio = inAudio;
            audioSource = inAudioSource;
            paused = inPause;
            looping = inLoop;
            ID = AUDIOID;
        }

        public bool CheckAudioName(string name)
        {
            return audioPath.Contains(name);
        }
    }
    public class ComponentAudio : IComponent
    {
        public List<AudioInfo> audioList = new List<AudioInfo>();

        /* Author: Alex DS */
        public ComponentAudio(string audioName, bool loop, bool active)
        {
            SetupAudio(audioName, loop, active); // setup audio and save it into the audio struct
        }

        public ComponentAudio(string[] audioNames,  bool loop, bool active){
            foreach(string s in audioNames){
                SetupAudio(s, loop, active); // setup audio and save it into the audio struct
            }
        }

        public void SetupAudio(string audioName, bool inLoop, bool active)
        {
            int audio = ResourceManager.LoadAudio("Audio/" + audioName); // load audio file

            // Create a sounds source using the audio clip
            int audioSource = AL.GenSource(); // gen a Source Handle
            AL.Source(audioSource, ALSourcei.Buffer, audio); // attach the buffer to a source

            // audio properties
            AL.Source(audioSource, ALSourceb.Looping, inLoop); // loop sound based on loop param

            audioList.Add(new AudioInfo(audioList.Count, audioName, audio, audioSource, inLoop, active)); // setup audio and save it into the audio struct
            Play(audioList[audioList.Count - 1], active, inLoop);
        }

        /* Author: Alex DS */
        public void Play(AudioInfo inf, bool play, bool loop = false)
        {
            inf.paused = !play;
            inf.looping = loop;
            //AL.Source(inf.audioSource, ALSourcei.Buffer, inf.audio); // attach the buffer to a source
            AL.Source(inf.audioSource, ALSourceb.Looping, inf.looping); // loop sound based on loop param

            if (!inf.paused) // if not paused, play sound
                AL.SourcePlay(inf.audioSource);
            else // if paused, pause sound
                AL.SourcePause(inf.audioSource);
        }

        public void Play(bool play, bool loop = false)
        {
            Play(audioList[0], play, loop);
        }

        public void Play(int id, bool play, bool loop = false)
        {
            Play(audioList[id], play, loop);
        }

        /* Author: Alex DS */
        public void UpdatePosition(AudioInfo inf, Vector3 newPos)
        {
            AL.Source(inf.audioSource, ALSource3f.Position, ref newPos);
        }

        // get audio by name
        public int Audio(string name)
        {
            foreach(AudioInfo inf in audioList){
                if(inf.CheckAudioName(name))
                    return inf.audio;
            }
            return 0;
        }
        // get audio by order in list
        public int Audio(int id)
        {
            foreach(AudioInfo inf in audioList){
                if(inf.ID == id)
                    return inf.audio;
            }
            return 0;
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_AUDIO; }
        }   
    }
}
