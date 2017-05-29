using OpenTK;

namespace GameEngine.Components
{
    /* Author: Alex DS */
    public class ComponentMotion : IComponent
    {
        Vector3 motionVel;
        public Vector3 AppliedMotion = new Vector3(0, 0, 0);
        public bool UpwardsMotion = true;
        private bool LastMotionWasDifferent = false;
        /* Author: Alex DS */
        public void CheckCurrentDirection()
        {
            if (UpwardsLimit() || DownwardsLimit())
            { // check if reached up/down-wards limit;
                UpwardsMotion = !UpwardsMotion; // invert direction
                AppliedMotion = new Vector3(0, 0, 0);
            }
        }
        /* Author: Alex DS */
        private bool UpwardsLimit()
        {
            return (AppliedMotion.Y > Motion.Y);
        }
        /* Author: Alex DS */
        private bool DownwardsLimit()
        {
            return (AppliedMotion.Y < -Motion.Y);
        }
        /* Author: Alex DS */
        public ComponentMotion(Vector3 vel)
        {
            motionVel = vel;
        }
        /* Author: Alex DS */
        public Vector3 Motion
        {
            get { return motionVel; }
            set { motionVel = value; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_MOTION; }
        }
    }
}
