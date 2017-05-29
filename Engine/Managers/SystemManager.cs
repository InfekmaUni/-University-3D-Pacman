using System.Collections.Generic;
using GameEngine.Systems;
using GameEngine.Objects;

namespace GameEngine.Managers
{
    /* Author: Alex DS */
    public class SystemManager
    {
        List<IUpdateableSystem> updateSystemList = new List<IUpdateableSystem>();
        List<IRenderableSystem> renderSystemList = new List<IRenderableSystem>();
        public SystemManager()
        {
        }
        /* Author: Alex DS */
        public void RenderSystems(EntityManager entityManager)
        {
            List<Entity> entityList = entityManager.Entities();
            foreach (IRenderableSystem system in renderSystemList)
            {
                system.OnRender();
            }
        }

        /* Author: Alex DS */
        public void UpdateSystems(EntityManager entityManager, float dt)
        {
            List<Entity> entityList = entityManager.Entities();
            foreach (IUpdateableSystem system in updateSystemList)
            {
                system.OnUpdate(dt);
            }
        }

        /* Author: Alex DS */
        public void PauseAllSounds(bool pause)
        {
            SystemAudio sysAudio = (SystemAudio)this.FindUpdateSystem("SystemAudio");
            sysAudio.PauseSounds(pause);
        }
        /* Author: Alex DS */
        public void AddUpdateSystem(IUpdateableSystem system)
        {
            IUpdateableSystem result = FindUpdateSystem(system.Name);
            //Debug.Assert(result != null, "System '" + system.Name + "' already exists");
            updateSystemList.Add(system);
        }
        /* Author: Alex DS */
        public void AddRenderSystem(IRenderableSystem system) {
            IRenderableSystem result = FindRenderSystem(system.Name);
            //Debug.Assert(result != null, "System '" + system.Name + "' already exists");
            renderSystemList.Add(system);
        }
        /* Author: Alex DS */
        private IUpdateableSystem FindUpdateSystem(string name)
        {
            return updateSystemList.Find(delegate (IUpdateableSystem system){ return system.Name == name; });
        }
        /* Author: Alex DS */
        private IRenderableSystem FindRenderSystem(string name)
        {
            return renderSystemList.Find(delegate (IRenderableSystem system) { return system.Name == name; });
        }
    }
}
