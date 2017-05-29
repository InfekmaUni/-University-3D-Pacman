using OpenTK;
using GameEngine.Components;
using GameEngine.Objects;
using System.Collections.Generic;

namespace GameEngine.Systems
{
    public class SystemPhysics : BaseSystem, IUpdateableSystem
    {
        /* Edited Alex DS */
        const ComponentTypes VELOCITYMASK = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_VELOCITY);
        const ComponentTypes MOTIONMASK = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_MOTION);

        List<Entity> velocityEntities = new List<Entity>();
        List<Entity> motionEntities = new List<Entity>();
        public SystemPhysics(List<Entity> entityList) : base("SystemPhysics")
        {
            ProcessEntityMask(entityList, velocityEntities, VELOCITYMASK);
            ProcessEntityMask(entityList, motionEntities, MOTIONMASK);
        }

        /* Author: Alex DS */
        public void OnUpdate(float dt)
        {
            //remove all entities that have Dispose true flag
            velocityEntities.RemoveAll(en => en.Dispose == true);
            motionEntities.RemoveAll(en => en.Dispose == true);

            foreach (Entity ent in velocityEntities)
            {
                ComponentTransform posComponent = (ComponentTransform)ent.GetComponent(ComponentTypes.COMPONENT_TRANSFORM);
                ComponentVelocity velComponent = (ComponentVelocity)ent.GetComponent(ComponentTypes.COMPONENT_VELOCITY);
                Move(posComponent, velComponent, dt);
            }

            foreach (Entity ent in motionEntities)
            {
                ComponentMotion motComponent = (ComponentMotion)ent.GetComponent(ComponentTypes.COMPONENT_MOTION);
                ComponentTransform posComponent = (ComponentTransform)ent.GetComponent(ComponentTypes.COMPONENT_TRANSFORM);

                Motion(posComponent, motComponent, dt);
            }
        }

        /* Edited: Alex DS */
        public void Move(ComponentTransform pos, ComponentVelocity vel, float dt)
        {
            pos.Position = pos.Position + vel.Velocity * dt;
            vel.Velocity = vel.Velocity / 1.1f ;
        }

        /* Author: Alex DS */
        public void Motion(ComponentTransform pos, ComponentMotion motVel, float dt)
        {
            motVel.CheckCurrentDirection(); // check current motion direction
            Vector3 addPos = new Vector3(motVel.Motion.X, motVel.Motion.Y, motVel.Motion.Z) * dt;
            if (!motVel.UpwardsMotion) // determine whether to invert the motion direction based on the current direction
                addPos = -addPos;

            // add position to components
            pos.Position += addPos;
            motVel.AppliedMotion = motVel.AppliedMotion + addPos;
        }
    }
}
