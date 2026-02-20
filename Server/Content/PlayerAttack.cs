using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Content
{
    class PlayerAttack : GameObject
    {
        static int CREATEID = -1;
        override public void Init(ObjectInfo _refObjectInfo)
        {
            CREATEID = _refObjectInfo.CreateId;
            m_eObjectType = ObjectType.Playerattack;
        }

        override public void Update()
        {
            //컴포넌트로 할지      
        }



        public override int GetCreateID() { return CREATEID; }


    }
}
