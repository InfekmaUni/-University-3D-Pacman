using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GameEngine.Components;
using GameEngine.Objects;
using System.Drawing;
using System.Drawing.Imaging;

namespace GameEngine.Systems
{
    #region Deprecated Multi-Light structs
    /* Author: Alex DS */
    struct DirectionLight
    {
        public Vector3 direction;
        public Vector3 ambient;
        public Vector3 diffuse;
        public Vector3 specular;

        public void Init(Vector3 a, Vector3 d, Vector3 s, Vector3 dir)
        {
            ambient = a;
            diffuse = d;
            specular = s;
            direction = dir;
        }
    };
    /* Author: Alex DS */
    #endregion

    public struct PointLight
    {
        public float light_constant;
        public float light_linear;
        public float light_quadratic;
        public float intensity;
        public void Init(float val1, float val2, float val3, float val4)
        {
            light_constant = val1;
            light_linear = val2;
            light_quadratic = val3;
            intensity = val4;
        }
        public void Default()
        {
            light_constant = 1;
            light_linear = 0.09f;
            light_quadratic = 0.032f;
            intensity = 10f;
        }
    };

    /* Author: Alex DS */
    public struct RenderInformation
    {
        public int renderInformationID;
        public int pgmID;
        public int vsID;
        public int fsID;
        public string vShaderName;
        public string fShaderName;
        public int attribute_Pos;
        public int attribute_TexCoord;
        public int attribute_Normal;
        public int uniform_Projection;
        public int uniform_Model;
        public int uniform_View;
        public int uniform_Texture;
        public int uniform_TextureScale;
        public int uniform_EyePos;
        public int uniform_LightPos;
    }

    // Author/Edited: Alex DS
    public class SystemRender : BaseSystem, IRenderableSystem
    {
        const ComponentTypes RENDERMASK = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_GEOMETRY | ComponentTypes.COMPONENT_TEXTURE | ComponentTypes.COMPONENT_RENDER);
        const ComponentTypes SCREENMASK = (ComponentTypes.COMPONENT_UI_SCREEN);
        const ComponentTypes CAMERAMASK = (ComponentTypes.COMPONENT_CAMERA);
        const ComponentTypes POINTLIGHTMASK = (ComponentTypes.COMPONENT_LIGHT_POINT| ComponentTypes.COMPONENT_TRANSFORM);
        const ComponentTypes TEXTMASK = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_GEOMETRY | ComponentTypes.COMPONENT_RENDER | ComponentTypes.COMPONENT_UI_TEXT);
        const ComponentTypes MINIMAPMASK = (ComponentTypes.COMPONENT_MINIMAP | ComponentTypes.COMPONENT_UI_SCREEN | ComponentTypes.COMPONENT_RENDER);
        const ComponentTypes MINIMAPOBJECTMASK = (ComponentTypes.COMPONENT_MINIMAP_TEXTURE | ComponentTypes.COMPONENT_TRANSFORM);
        #region Deprecated Multi Lights
        // Author: Alex DS
        /*const ComponentTypes MASK_LIGHT_POINT = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_LIGHT_POINT);
        const ComponentTypes MASK_LIGHT_DIRECTIONAL = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_LIGHT_DIRECTIONAL);*/
        #endregion

        #region Variables & Accessors/Mutators
        // Author: Alex DS
        private Matrix4 UiProjection;
        private ComponentUIScreen UiScreen;
        private ComponentMinimap minimap;
        private Bitmap textBMP;
        private int textTexture;
        private Graphics textGFX;
        private bool initText = false;
        public List<RenderInformation> renderInfo = new List<RenderInformation>();
        private ComponentCamera viewCamera;
        private ComponentCamera minimapCamera;
        private ComponentTransform worldTransform;
        private ComponentTransform lightTransform; // object used for latern
        public ComponentCamera MinimapTarget
        {
            get { return minimapCamera; }
            set { minimapCamera = value; }
        }
        public ComponentCamera ViewTarget
        {
            get { return viewCamera; }
            set { viewCamera = value; }
        }
        private Matrix4 mainProjection;
        public Matrix4 MainProjection
        {
            get { return mainProjection; }
            set { mainProjection = value; }
        }

        /* Author: Alex DS */
        /// <summary>
        /// this method sets a world space
        /// </summary>
        /// <param name="ent"></param>
        public void SetWorldEntity(Entity ent)
        {
            if (ent.HasComponent(ComponentTypes.COMPONENT_TRANSFORM))
            {
                ComponentTransform transform = (ComponentTransform)ent.GetComponent(ComponentTypes.COMPONENT_TRANSFORM);

                worldTransform = transform;
            }
            else
                Console.WriteLine("[SYSTEM-RENDER] world target does not have transform, unable to find position");
        }

        public void SetLaternTarget(ComponentTransform transform)
        {
            lightTransform = transform;
        }
        #endregion

        List<Entity> renderEntities = new List<Entity>();
        List<Entity> screenEntities = new List<Entity>();
        List<Entity> cameraEntities = new List<Entity>();
        List<Entity> pointlightEntities = new List<Entity>();
        List<Entity> textEntities = new List<Entity>();
        List<Entity> minimapEntities = new List<Entity>();
        List<List<Entity>> minimapRenderEntities = new List<List<Entity>>();
        /* Edited: Alex DS */
        public SystemRender(List<Entity> entityList)
            : base("SystemRender")
        {
            ProcessEntityMask(entityList, renderEntities, RENDERMASK);
            ProcessEntityMask(entityList, screenEntities, SCREENMASK);
            ProcessEntityMask(entityList, cameraEntities, CAMERAMASK);
            ProcessEntityMask(entityList, pointlightEntities, POINTLIGHTMASK);
            ProcessEntityMask(entityList, textEntities, TEXTMASK);
            ProcessEntityMask(entityList, minimapEntities, MINIMAPMASK);

            // create 2 list for z index
            minimapRenderEntities.Add(new List<Entity>());
            minimapRenderEntities.Add(new List<Entity>());
            foreach (Entity ent in entityList)
            {
                if ((ent.Mask & MINIMAPOBJECTMASK) == MINIMAPOBJECTMASK)
                {
                    try { 
                        ComponentMinimapTexture texture = ((ComponentMinimapTexture)ent.GetComponent(ComponentTypes.COMPONENT_MINIMAP_TEXTURE));
                         minimapRenderEntities[texture.Index].Add(ent);
                    }
                    catch // if the above fails it means it tried to add an entity to a z index that is not supported < 0 or > 1
                    {
                        throw new Exception("Minimap texture z Index only supporst 0 or 1");
                    }
                }
            }
        }

        /* Author: Alex DS */
        public int CreateRendererForShader(string vertexShaderName, string fragmentShaderName)
        {
            RenderInformation info = new RenderInformation();
            info.renderInformationID = renderInfo.Count();
            info.pgmID = GL.CreateProgram();
            LoadShader("Shaders/" + vertexShaderName + ".glsl", ShaderType.VertexShader, info.pgmID, out info.vsID);
            LoadShader("Shaders/" + fragmentShaderName + ".glsl", ShaderType.FragmentShader, info.pgmID, out info.fsID);
            info.vShaderName = vertexShaderName; info.fShaderName = fragmentShaderName; // set struct shader names
            GL.LinkProgram(info.pgmID);
            Console.WriteLine(GL.GetProgramInfoLog(info.pgmID));

            info.attribute_Pos = GL.GetAttribLocation(info.pgmID, "a_position");
            info.attribute_TexCoord = GL.GetAttribLocation(info.pgmID, "a_texCoord");
            info.attribute_Normal = GL.GetAttribLocation(info.pgmID, "a_normal"); // currently empty
            info.uniform_Projection = GL.GetUniformLocation(info.pgmID, "projection");
            info.uniform_Texture = GL.GetUniformLocation(info.pgmID, "texture");
            info.uniform_TextureScale = GL.GetUniformLocation(info.pgmID, "textureScale");

            info.uniform_Model = GL.GetUniformLocation(info.pgmID, "model");
            info.uniform_View = GL.GetUniformLocation(info.pgmID, "view");

            info.uniform_EyePos = GL.GetUniformLocation(info.pgmID, "eyePosition");
            info.uniform_LightPos = GL.GetUniformLocation(info.pgmID, "lightPosition");

            if (info.attribute_Pos == -1 || info.attribute_TexCoord == -1 || info.uniform_Projection == -1 || info.uniform_Texture == -1 || info.uniform_Model == -1 || info.uniform_View == -1 || info.attribute_Normal == -1)
            {
                Console.WriteLine("Error binding attributes or uniforms for newly created shader program");
            }

            renderInfo.Add(info);
            return info.renderInformationID;
        }

        void LoadShader(String filename, ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

        /* Edited: Alex DS */
        public void OnRender()
        {
            renderEntities.RemoveAll(en => en.Dispose == true);
            screenEntities.RemoveAll(en => en.Dispose == true);
            cameraEntities.RemoveAll(en => en.Dispose == true);
            pointlightEntities.RemoveAll(en => en.Dispose == true);
            textEntities.RemoveAll(en => en.Dispose == true);
            minimapEntities.RemoveAll(en => en.Dispose == true);
            foreach (List<Entity> list in minimapRenderEntities)
                list.RemoveAll(en => en.Dispose == true);

            foreach (Entity ent in textEntities)
            {
                ComponentTransform transform = (ComponentTransform)ent.GetComponent(ComponentTypes.COMPONENT_TRANSFORM);
                ComponentUIText text = (ComponentUIText)ent.GetComponent(ComponentTypes.COMPONENT_UI_TEXT);
                Geometry geometry = ((ComponentGeometry)ent.GetComponent(ComponentTypes.COMPONENT_GEOMETRY)).Geometry();
                ComponentTransform comp = (ComponentTransform)ent.GetComponent(ComponentTypes.COMPONENT_TRANSFORM);
                ComponentRender rendComp = (ComponentRender)ent.GetComponent(ComponentTypes.COMPONENT_RENDER);

                CheckComponentForProgram(rendComp);

                if (UiProjection != null && UiScreen != null)
                    DrawText(rendComp.RendererID, comp, geometry, text);
            }

            foreach (Entity ent in renderEntities)
            {
                Geometry geometry = ((ComponentGeometry)ent.GetComponent(ComponentTypes.COMPONENT_GEOMETRY)).Geometry();
                ComponentTransform comp = (ComponentTransform)ent.GetComponent(ComponentTypes.COMPONENT_TRANSFORM);
                ComponentTexture texture = ((ComponentTexture)ent.GetComponent(ComponentTypes.COMPONENT_TEXTURE));
                ComponentRender rendComp = (ComponentRender)ent.GetComponent(ComponentTypes.COMPONENT_RENDER);

                CheckComponentForProgram(rendComp);

                bool isUI = ent.Name.Contains("UI") && (UiProjection != null);
                Draw(rendComp.RendererID, comp, geometry, texture, isUI);
            }

            foreach (Entity ent in screenEntities)
            {
                ComponentUIScreen screen = (ComponentUIScreen)ent.GetComponent(ComponentTypes.COMPONENT_UI_SCREEN);
                UiProjection = screen.Projection;
                UiScreen = screen;
            }

            foreach (Entity ent in cameraEntities)
            {
                ComponentCamera camera = (ComponentCamera)ent.GetComponent(ComponentTypes.COMPONENT_CAMERA);
                 ViewTarget = camera;
                 MainProjection = camera.Projection;
            }

            foreach (Entity ent in pointlightEntities)
            {
                ComponentPointLight light = (ComponentPointLight)ent.GetComponent(ComponentTypes.COMPONENT_LIGHT_POINT);
                ComponentTransform transform = (ComponentTransform)ent.GetComponent(ComponentTypes.COMPONENT_TRANSFORM);
                SetLaternTarget(transform);
            }

            foreach (Entity ent in minimapEntities)
            {
                ComponentMinimap minimapComp = (ComponentMinimap)ent.GetComponent(ComponentTypes.COMPONENT_MINIMAP);
                ComponentRender rendComp = (ComponentRender)ent.GetComponent(ComponentTypes.COMPONENT_RENDER);
                Geometry geometry = ((ComponentGeometry)ent.GetComponent(ComponentTypes.COMPONENT_GEOMETRY)).Geometry();
                minimap = minimapComp;
                CheckComponentForProgram(rendComp);

                foreach (List<Entity> list in minimapRenderEntities)
                {
                    foreach (Entity ent2 in list)
                    {
                        ComponentTransform comp = (ComponentTransform)ent2.GetComponent(ComponentTypes.COMPONENT_TRANSFORM);
                        ComponentMinimapTexture texture = ((ComponentMinimapTexture)ent2.GetComponent(ComponentTypes.COMPONENT_MINIMAP_TEXTURE));

                        DrawMinimap(rendComp.RendererID, comp, geometry, texture);
                    }
                }
            }

            #region Deprecated Multi-lights
            /*
        // Author: Alex DS
            if ((entity.Mask & MASK_LIGHT_POINT) == MASK_LIGHT_POINT)
            {
                List<IComponent> components = entity.Components;

                // transform component
                IComponent transformComponent = components.Find(delegate(IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
                });
                ComponentTransform transform = (ComponentTransform)transformComponent;

                // light component
                IComponent lightComponent = components.Find(delegate(IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_LIGHT_POINT;
                });
                ComponentPointLight light = (ComponentPointLight)lightComponent;

                AddPointLight(light.Light, transform.Position);
            }
        // Author: Alex DS
            if ((entity.Mask & MASK_LIGHT_DIRECTIONAL) == MASK_LIGHT_DIRECTIONAL)
            {
                List<IComponent> components = entity.Components;

                // transform component
                IComponent transformComponent = components.Find(delegate(IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
                });
                ComponentTransform transform = (ComponentTransform)transformComponent;

                 // light component
                IComponent lightComponent = components.Find(delegate(IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_LIGHT_DIRECTIONAL;
                });
                ComponentDirectionLight light = (ComponentDirectionLight)lightComponent;

                AddDirLight(light.Light, transform.Position);
            }
             */
            #endregion
        }

        /// <summary>
        /// this method setups the bitmap, graphics and texture that is re-used for each text, only called once
        /// </summary>
        private void InitText()
        {
            // Create Bitmap and OpenGL texture for rendering text
            // work out size of bitmap relative to string size
            textBMP = new Bitmap((int)UiScreen.Width, (int)UiScreen.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb); // match window size
            textGFX = Graphics.FromImage(textBMP);
            textTexture = GL.GenTexture();

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, textTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textBMP.Width, textBMP.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
            textBMP = new Bitmap((int)UiScreen.Width, (int)UiScreen.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb); // match window size
            textGFX = Graphics.FromImage(textBMP);

            initText = true; // set flag
        }
        /// <summary>
        /// This method draws text to the screen using a bitmap rendered to a texture on a UIProjection scaled bitmap and projection size
        /// </summary>
        /// <param name="rendererID">renderer ID</param>
        /// <param name="transform">transform component used to position text</param>
        /// <param name="geometry">goemetry which the text is textured on</param>
        /// <param name="text">text component</param>
        public void DrawText(int rendererID, ComponentTransform transform, Geometry geometry, ComponentUIText text)
        {
            RenderInformation info = renderInfo[rendererID];
            // use component program id
            GL.UseProgram(info.pgmID);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            if( !initText ) // if text was not previously initialised
                InitText();

            // Create Bitmap and OpenGL texture for rendering text
            // work out size of bitmap relative to string size
            textGFX.Clear(Color.FromArgb(0, 0,0,0)); // clear texture of previous draw

            #region texture & bitmap data
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, textTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textBMP.Width, textBMP.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);

            // drawstring to image
            textGFX.DrawString(text.Text, text.Font, Brushes.White, 0, 0);

            // generate color
            BitmapData data = textBMP.LockBits(new Rectangle(0, 0, textBMP.Width, textBMP.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)textBMP.Width, (int)textBMP.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            textBMP.UnlockBits(data);
            #endregion

            #region text uniforms
            GL.Uniform1(info.uniform_TextureScale, 1); // texture scale

            // models
            float textSize = 0;
            if(text.Centered)
                textSize = text.Text.Length * text.Font.Size; // calculate pixels needed to center the text, we subtract this from the final translation
            Matrix4 model = Matrix4.CreateScale(UiScreen.Width, UiScreen.Height, 1) * Matrix4.CreateTranslation(new Vector3(UiScreen.Width + transform.Position.X - textSize, UiScreen.Height + transform.Position.Y, 0));
            GL.UniformMatrix4(info.uniform_Model, false, ref model);

            GL.UniformMatrix4(info.uniform_Projection, false, ref UiProjection);
            #endregion

            geometry.Render(); // render geometry

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }

        /* Author: Alex DS */
        /// <summary>
        /// This method creates the shader program if they previously werent created
        /// </summary>
        /// <param name="rendComp">render component which houses shader information</param>
        public void CheckComponentForProgram(ComponentRender rendComp)
        {
            if (!rendComp.IsInit())
            { // if not been initialised before
                bool foundRenderer = false;
                foreach (RenderInformation info in renderInfo)
                { // check all structs within the list
                    if (info.vShaderName == rendComp.vertexShaderName && info.fShaderName == rendComp.fragmentShaderName)
                    { // if vertex and fragment shader match
                        rendComp.Init(info.renderInformationID); // copy the render ID to the component
                        foundRenderer = true;
                    }
                }

                if (!foundRenderer) // if not found earlier
                    rendComp.RendererID = CreateRendererForShader(rendComp.vertexShaderName, rendComp.fragmentShaderName); // creates renderer for shader
            }
        }

        /* Author/Edited: Alex DS */
        // currently not being used
        public void DrawMinimap(int rendererID, ComponentTransform transform, Geometry geometry, ComponentMinimapTexture text)
        {
            RenderInformation info = renderInfo[rendererID];
            GL.UseProgram(info.pgmID);

            GL.Uniform1(info.uniform_Texture, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, text.Texture);
            GL.Enable(EnableCap.Texture2D);
            GL.Uniform1(info.uniform_TextureScale, 1); // texture scale

            int xOffset = (int)minimap.Offset.X;
            int yOffset = (int)minimap.Offset.Y;
            float minimapSize = minimap.Size;
            Vector2 minimapPositionCentered = new Vector2(xOffset, yOffset);

            // models
            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(minimapPositionCentered.X, minimapPositionCentered.Y, 0)) * Matrix4.CreateTranslation(new Vector3(transform.Position.X, transform.Position.Z, 0));
            Matrix4 model = Matrix4.CreateScale(text.Scale) * translation * Matrix4.CreateScale(minimapSize);
            GL.UniformMatrix4(info.uniform_Model, false, ref model);

            Matrix4 proj = UiProjection;
            GL.UniformMatrix4(info.uniform_Projection, false, ref proj);

            geometry.Render();

            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }

        /* Author/Edited: Alex DS */
        /// <summary>
        /// Main method which handles the drawing of objects and UI elements
        /// </summary>
        /// <param name="rendererID">render ID to access shader information</param>
        /// <param name="transform">transform component for tranforms</param>
        /// <param name="geometry">goemetry</param>
        /// <param name="text">texture</param>
        /// <param name="UI">whether the draw is for UI</param>
        public void Draw(int rendererID, ComponentTransform transform, Geometry geometry, ComponentTexture text, bool UI)
        {
            RenderInformation info = renderInfo[rendererID];
            // use component program id
            GL.UseProgram(info.pgmID);
            #region Shared shader uniforms
            GL.Uniform1(info.uniform_Texture, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, text.Texture);
            GL.Enable(EnableCap.Texture2D);
            GL.Uniform1(info.uniform_TextureScale, text.Scale); // texture scale

            // models
            Matrix4 model = transform.ModelSpace; 
            GL.UniformMatrix4(info.uniform_Model, false, ref model);
            #endregion

            if (UI)
             // ui draw call
                UIDraw(info, transform, geometry);
           else
            { // object draw call
                if (viewCamera != null) // main object draw call
                    ObjectDraw(info, transform, geometry, viewCamera.View, MainProjection);
            }

            //GL.Disable(EnableCap.Blend);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }

        /* Author: Alex DS */
        /// <summary>
        /// This method draws the 3d objects into the world using view and projection previous set previously
        /// </summary>
        /// <param name="info">render information</param>
        /// <param name="transform">transform component for transform</param>
        public void ObjectDraw(RenderInformation info, ComponentTransform transform, Geometry geometry, Matrix4 view, Matrix4 projection)
        {
            #region Projection Uniforms
            Matrix4 worldSpace = Matrix4.CreateTranslation(0, 0, 0);
            if (worldTransform != null)
                worldSpace = Matrix4.CreateRotationY(worldTransform.Rotation.Y) * Matrix4.CreateTranslation(worldTransform.Position);

            int uniform_world = GL.GetUniformLocation(info.pgmID, "world");
            GL.UniformMatrix4(uniform_world, false, ref worldSpace);

            GL.UniformMatrix4(info.uniform_Projection, false, ref projection);
            GL.UniformMatrix4(info.uniform_View, false, ref view);

            #endregion

            #region Light Uniforms
            Vector3 viewPos = new Vector3(0, 1, 0);
            if (viewCamera != null)
                viewPos = viewCamera.View.ExtractTranslation();
            GL.Uniform3(info.uniform_EyePos, ref viewPos);

            Vector3 lightPos = new Vector3(0, 1, 0);  // default light position
            if (lightTransform != null) // if light target was set, use its transform position
                lightPos = lightTransform.Position;

            GL.Uniform3(info.uniform_LightPos, ref lightPos);
            #endregion

            geometry.Render();
        }

        /* Author: Alex DS */
        /// <summary>
        /// this method is to draw UI elements, not including text
        /// </summary>
        /// <param name="info">render information for shader</param>
        /// <param name="transform">transform component for transforms</param>
        public void UIDraw(RenderInformation info, ComponentTransform transform, Geometry geometry)
        {
            //GL.Enable(EnableCap.Blend);
            // black == transparent
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.UniformMatrix4(info.uniform_Projection, false, ref UiProjection);
            geometry.Render();
        }

        #region Deprecated Multi-Lights
        /*
        // Author: Alex DS
        public void ApplyLighSources(int pgmID)
        {
            foreach (KeyValuePair<DirectionLight, Vector3> light in dirLightDictionary)
                ApplyDirectionLight(pgmID, light.Value, light.Key);

            foreach (KeyValuePair<PointLight, Vector3> light in pointLightDictionary)
                ApplyPointLight(pgmID, light.Value, light.Key);
        }
        // Author: Alex DS
        public void ApplyDirectionLight(int pgmID, Vector3 pos, DirectionLight light)
        {
            int uniform_direction = GL.GetUniformLocation(pgmID, "DirectionLight");
            GL.Uniform3(uniform_direction, ref light.direction);
        }
        // Author: Alex DS
        public void ApplyPointLight(int pgmID, Vector3 pos, PointLight light)
        {
            /*
            int uniform_ambientLight = GL.GetUniformLocation(pgmID, "uLight[" + i + "].AmbientLight");
            //Vector3 ambientLight = light.Ambient;
            //GL.Uniform3(uniform_ambientLight, ref ambientLight);

            int uniform_diffuseLight = GL.GetUniformLocation(pgmID, "uLight[" + i + "].DiffuseLight");
            //Vector3 diffuseLight = light.Diffuse;
           // GL.Uniform3(uniform_diffuseLight, ref diffuseLight);

            int uniform_specularLight = GL.GetUniformLocation(pgmID, "uLight[" + i + "].SpecularLight");
            //Vector3 specularLight = light.Specular;
            //GL.Uniform3(uniform_specularLight, ref specularLight);

            int uniform_lightPos = GL.GetUniformLocation(pgmID, "uLight[" + i + "].Position");
            //GL.Uniform3(uniform_lightPos, ref pos);

            if (uniform_specularLight == -1 || uniform_diffuseLight == -1 || uniform_ambientLight == -1 || uniform_lightPos == -1)
            {
                Console.WriteLine("Unable to bind a light uniform");
            }
        }

        // list of lights
       // Author: Alex DS
        private Dictionary<PointLight, Vector3> pointLightDictionary = new Dictionary<PointLight, Vector3>();
        private Dictionary<DirectionLight, Vector3> dirLightDictionary = new Dictionary<DirectionLight, Vector3>();
        private void AddDirLight(DirectionLight light, Vector3 pos)
        {
            if (!dirLightDictionary.ContainsKey(light))
                dirLightDictionary.Add(light, pos);
        }

        // Author: Alex DS
        private void AddPointLight(PointLight light, Vector3 pos)
        {
            if (!pointLightDictionary.ContainsKey(light))
                pointLightDictionary.Add(light, pos);
        }
         */
        #endregion
    }
}