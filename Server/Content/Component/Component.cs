using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public enum ComponentType
    {
        Transform = 0,
        Collider = 1,
        End = 2,
    }

    public abstract class Component
    {
        protected GameObject m_refOwner = null;
        public GameObject Owner {  get { return m_refOwner; } }
        public abstract void Update();
        public abstract void LateUpdate();

        public virtual void Init(GameObject _refOwner){m_refOwner = _refOwner; }

        private ComponentType m_eComponentType = ComponentType.End;
        public ComponentType ComponentType
        {
            get { return m_eComponentType; }
        }


    }
}
