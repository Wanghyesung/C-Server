using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void C_ChatHandler(PacketSession _refSession, IPacket _iPacket)
    {
        C_Chat chat = _iPacket as C_Chat;
        ClientSession clientSesion = _refSession as ClientSession;

        if (clientSesion == null)
            return;

        clientSesion.Room.Broadcast(clientSesion, chat.chat);
    }
}

