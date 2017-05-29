using GameEngine.Objects;
using System.Collections.Generic;
using GameEngine.Components;

namespace GameEngine.Systems
{
    /* Author: Alex DS */
    public class BaseSystem
    {
        public BaseSystem(string name)
        {
            systemName = name;
        }

        /* Author: Alex DS */
        public void ProcessEntityMask(List<Entity> listToCheck, List<Entity> listToAdd, ComponentTypes MASK)
        {
            foreach (Entity ent in listToCheck)
            {
                if ((ent.Mask & MASK) == MASK)
                {
                    listToAdd.Add(ent);
                }
            }
        }

        protected string systemName = "UNDEFINED";
        /* Author: Alex DS */
        public string Name
        {
            get { return systemName; }
            set { systemName = value; }
        }

    }
}
