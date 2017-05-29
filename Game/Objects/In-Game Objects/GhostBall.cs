using OpenTK;
using GameEngine.Objects;
using GameEngine.Components;
using System.Collections.Generic;
using OpenGL_Game.Managers;
using System;
namespace OpenGL_Game.Objects.In_Game_Objects
{
    //author adam deere. following the structure laid out by alex ds
    class GhostBall : PrimitiveGeometry
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_VELOCITY | ComponentTypes.COMPONENT_INPUT);
        private ComponentSphereCollider sphereCollider;
        private Vector3 PrevPos;
        ComponentTransform posComponent;
        MyGame.PacmanState temp = MyGame.PacmanState.Default;
        ComponentVelocity compVel;
        public Vector3 pacmanPos;

        public bool collided = false;
        public GhostBall(Vector3 pos)
            : base("GhostBall", pos, new Vector3(0, 0, 0), new Vector3(2 ,2, 2), "Sphere", "ghostBall.png")
        {
            base.AddComponent(new ComponentMotion(new Vector3(0, 0.1f, 0)));
            base.AddComponent(posComponent = new ComponentTransform(pos, new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
            base.AddComponent(sphereCollider = new ComponentSphereCollider(this, this.Scale));
            base.AddComponent(new ComponentMinimapTexture("Textures/Minimap/ghost.png", 4,1,0));
            base.AddComponent(compVel = new ComponentVelocity(0, 0, 0));
            sphereCollider.sCollided = Collided;
            sphereCollider.bCollided = WallCollided;
        }

        /* Author: Alex DS & Adam */
        public void Collided(ComponentSphereCollider other)
        {
            if (other.Entity.Name.Contains("Pacman"))
            {
                collided = true;
                //compAudio.Play(true); // play death soud
                /*increment score */
            }
        }

        Vector3 TargetPos;

        Vector3 WaypointTargetPos;
        int currentWayPointID = -1;
        List<int> waypointList = new List<int>();
        Random rnd = new Random();
        public int FindClosestWaypoint(Vector3 currentPos)
        {
            int curClose = -1;
            for (int i = 0; i < MyGame.Waypoints.Count; i++)
            {
                int r = rnd.Next();
                if (i == currentWayPointID || waypointList.Contains(i) || (r % 2) == 0)
                    continue;

                Vector3 currentWaypointsPos = MyGame.Waypoints[i];
                if (curClose == -1 || (currentPos - currentWaypointsPos).Length < (currentPos - MyGame.Waypoints[curClose]).Length)
                {
                    curClose = i;
                }
            }
            return curClose;
        }


        // author John-Ross Williamson
        public void SetTargetLocation(Vector3 pacPos) // Method to set the target of the ghost between the home base to the Pac Man
        {
            if (temp == MyGame.PacmanState.Frenzy) //(GhostTarget == Target.Home_Base)
            {
                TargetPos = MyGame.ghostPos[0];
            }

            else if (temp == MyGame.PacmanState.Default)
            {
                Vector3 ghostPos = base.Position;

                bool inRange = (ghostPos - WaypointTargetPos).Length < this.Scale.X;
                if (currentWayPointID == -1 || inRange)
                {
                    int closestWaypointId = FindClosestWaypoint(ghostPos);
                    currentWayPointID = closestWaypointId;

                   waypointList.Add(currentWayPointID);
                    if (waypointList.Count >= 15) // if list has more then 4 entries, remove first entry
                        waypointList.RemoveAt(0);

                    WaypointTargetPos = MyGame.Waypoints[closestWaypointId];
                    TargetPos = WaypointTargetPos;
                }
            }
        }

        /*
         if pos == waypoint
         then randomly 1 and 4
         if you cant go in that direction 
         try another direction until you can go
         */
        public void MoveGhost(Vector3 pacPos)
        {
            temp = MyGame.pacState;
            PrevPos = posComponent.Position;
            // Get the ghost's position
            Vector3 ghostPos = ((ComponentTransform)this.GetComponent(ComponentTypes.COMPONENT_TRANSFORM)).Position;

            // Get Pac Man's position
            SetTargetLocation(pacPos);

            Vector3 Direction = Vector3.Normalize(TargetPos - ghostPos);
            ComponentVelocity velocity = ((ComponentVelocity)base.GetComponent(ComponentTypes.COMPONENT_VELOCITY));
            velocity.Velocity = Direction * 3; // The 3 should be replaced by a movement speed.
        }

        public void WallCollided(ComponentBoxCollider other)
        {
            // reset position to previous update since we've collided with wall
            posComponent.Position = PrevPos;
        }
    }
}
