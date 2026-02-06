using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void C_LeaveGameHandler(PacketSession _refSession, IPacket _iPacket)
    {
        ClientSession clientSession = _refSession as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Leave(clientSession);
    }

    public static void C_MoveHandler(PacketSession _refSession, IPacket _iPacket)
    {
        C_Move movePacket = _iPacket as C_Move;
        ClientSession clientSession = _refSession as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Move(clientSession, movePacket);
    }
}

