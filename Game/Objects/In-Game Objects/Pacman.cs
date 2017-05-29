using OpenTK;
using GameEngine.Components;
using GameEngine.Objects;
using System.Collections.Generic;

namespace OpenGL_Game.Objects
{
    /* Author: Alex DS */
    class Pacman : Entity
    {
        /* Author: Alex DS */
        private ComponentSphereCollider sphereCollider;
        private ComponentTransform posComp;
        private ComponentAudio audioComp;
        List<string> audioList = new List<string>();
        private Vector3 Scale
        {
            get { return posComp.Scale; }
        }

        public Vector3 Position
        {
            get { return posComp.Position; }
        }
        public Pacman(Vector3 pos, Matrix4 projection, bool debug = false)
            : base("Pacman")
        {
            base.AddComponent(posComp = new ComponentTransform(pos, new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
            base.AddComponent(new ComponentAudioSource());

            // all audio files attached to pacman
            string[] temp = new string[] {"pickup_energy.wav", "pickup_power.wav", "bird.wav", "player_died.wav",  "player_eats.wav" };
            foreach(string s in temp)
            {
                audioList.Add(s);
            }
            base.AddComponent(audioComp = new ComponentAudio(temp, false, false));

            base.AddComponent(new ComponentVelocity(0,0,0));
            base.AddComponent(new ComponentInput());
            base.AddComponent(new ComponentCamera(pos, projection));
            base.AddComponent(new ComponentPointLight(1,0.22f, 0.20f, 1));
            base.AddComponent(sphereCollider = new ComponentSphereCollider((Entity)this, this.Scale));
            base.AddComponent(new ComponentMinimapTexture("Textures/Minimap/pacman.png", 4,1,0));
            sphereCollider.sCollided = Collided;
            sphereCollider.bCollided = WallCollided;

            if (debug) // if debug, draw a geometry sphere
            {
                base.AddComponent(new ComponentGeometry("Geometry/SphereGeometry.txt"));
                base.AddComponent(new ComponentTexture("Textures/spaceship.png"));
                base.AddComponent(new ComponentRender("vs", "fs"));
            }
        }

        private Vector3 prevPos;
        public void Update(float dt)
        {
            prevPos = posComp.Position;
        }
        private bool frenzyMode = false;
        public bool Frenzy
        {
            get { return frenzyMode; }
            set { frenzyMode = value; ToggleFrenzySound(value); }
        }
        public void Collided(ComponentSphereCollider other)
        {
            // collided with sphere, shouldn't do anyhting in here thought
            string entName = other.Entity.Name;
            if (entName.Contains("Energy")) // collided with energy
            {
                audioComp.Play(0, true);
            }else if(entName.Contains("Power")) // collided with power
            {
                audioComp.Play(1, true);
            }
            else if (entName.Contains("Ghost"))
            {
                if (frenzyMode)
                {
                    audioComp.Play(3, true); // player eats ghost
                }
                else
                {
                    audioComp.Play(4, true); // ghost eats player
                }
            }
        }

        public void ToggleFrenzySound(bool frenzy)
        {
            if (frenzy)
            {
                audioComp.Play(2, true, true);
            }
            else
                audioComp.Play(2, false, false);
        }

        public void WallCollided(ComponentBoxCollider other)
        {
            // reset position to previous update since we've collided with wall
            posComp.Position = prevPos;
        }
    }
}