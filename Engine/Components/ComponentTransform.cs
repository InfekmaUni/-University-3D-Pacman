using System;
using OpenTK;

namespace GameEngine.Components
{
    /*  Author: Alex DS */
    public class ComponentTransform : IComponent
    {
        private Vector3 position, rotation, scale;
        private Matrix4 modelSpace;
        public bool pickedUp;
        #region Constructors

        /*  Author: Alex DS */
        public ComponentTransform(Vector3 inPosition)
        {
            position = inPosition;
            rotation = new Vector3(0, 0, 0);
            scale = new Vector3(1, 1, 1);
            ConstructModelSpace();
        }

        /*  Author: Alex DS */
        public ComponentTransform(Vector3 inPosition, Vector3 inRotation)
        {
            position = inPosition;
            rotation = inRotation;
            scale = new Vector3(1, 1, 1);
            ConstructModelSpace();
        }
        

        /*  Author: Alex DS */
        public ComponentTransform(Vector3 inPosition, Vector3 inRotation, Vector3 inScale)
        {
            position = inPosition;
            rotation = inRotation;
            scale = inScale;
            ConstructModelSpace();
        }
        #endregion

        /* Author: Alex DS */
        private float DegreeToRadian(float angle)
        {
            return (float)Math.PI * angle / 180;
        }

        /*  Author: Alex DS */
        /// <summary>
        /// this method is called to re-construct the modelspace for this transform component, modelSpace is constructed using scale * rotation * translation
        /// </summary>
        private void ConstructModelSpace()
        {          
            Matrix4 spaceTranslate = Matrix4.CreateTranslation(position);
            Matrix4 spaceRotation = Matrix4.CreateRotationX(DegreeToRadian(rotation.X)) * Matrix4.CreateRotationY(DegreeToRadian(rotation.Y)) * Matrix4.CreateRotationZ(DegreeToRadian(rotation.Z));
            Matrix4 spaceScale = Matrix4.CreateScale(scale.X, scale.Y, scale.Z);
            modelSpace = spaceScale * spaceRotation * spaceTranslate;
        }

        /*  Author: Alex DS */
        public Matrix4 ModelSpace
        {
            get { return modelSpace; }
            set { modelSpace = value; }
        }

        /*  Author: Alex DS */
        public Vector3 Scale
        {
            get { return scale; }
            set { scale = value; ConstructModelSpace(); } // update model space
        }

        /*  Author: Alex DS */
        public Vector3 Position
        {
            get { return position; }
            set { position = value; ConstructModelSpace(); } // update model space
        }

        /*  Author: Alex DS */
        public Vector3 Rotation
        {
            get { return rotation; }
            set { rotation = value; ConstructModelSpace(); } // update model space
        }

        /*  Author: Alex DS */
        public Vector3 Right
        {
            get { return new Vector3(modelSpace.Column0.X, modelSpace.Column0.Y, modelSpace.Column0.Z); }
        }
        /*  Author: Alex DS */
        public Vector3 Up
        {
            get { return new Vector3(modelSpace.Column1.X, modelSpace.Column1.Y, modelSpace.Column1.Z); }
        }
        /*  Author: Alex DS */
        public Vector3 Forward
        {
            get { return new Vector3(modelSpace.Column2.X, modelSpace.Column2.Y, modelSpace.Column2.Z); }
        }
        /*  Author: Alex DS */
        public Vector3 Back
        {
            get { return new Vector3(modelSpace.Column3.X, modelSpace.Column3.Y, modelSpace.Column3.Z); }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_TRANSFORM; }
        }
    }
}
