using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GameEngine.Components;
using GameEngine;

namespace OpenGL_Game.Objects
{
    /* Author: Alex DS */
    class PrimitiveGeometry : RenderedObject
    {
        /* Author: Alex DS */
        public PrimitiveGeometry(string name, Vector3 pos, Vector3 rot, Vector3 scale, string geometryName, string texturePath, int textureScale = 1)
            : base("Primitive_" + geometryName + "_" + name, pos, rot, scale, geometryName + "Geometry.txt", texturePath)
        {
            base.TextureScale = textureScale;
        }
    }
}
