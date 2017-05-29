using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameEngine.Components;

namespace GameEngine.Objects
{
    public class Entity
    {
        public bool dispose = false;
        public bool Dispose
        {
            get { return dispose; }
            set { dispose = value; }
        }
        public string name;
        List<IComponent> componentList = new List<IComponent>();
        ComponentTypes mask;

        public Entity(string name)
        {
            this.name = name;
        }

        /// <summary>Adds a single component</summary>
        public void AddComponent(IComponent component)
        {
            Debug.Assert(component != null, "Component cannot be null");

            componentList.Add(component);
            mask |= component.ComponentType;
        }
        public Entity getEntity
        {
            get
            {
                return this;
            }
        }
        public String Name
        {
            get { return name; }
        }

        public ComponentTypes Mask
        {
            get { return mask; }
        }

        // Author: Alex DS
        public bool HasComponent(ComponentTypes componentType)
        {
            return (mask & componentType) == componentType;
        }

        public List<IComponent> Components
        {
            get { return componentList; }
        }

        public IComponent GetComponent(ComponentTypes type)
        {
            IComponent foundComponent = componentList.Find(delegate(IComponent component)
            {
                return component.ComponentType == type;
            });
            return foundComponent;
        }
    }
}
