using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void S_BoradcastEnterGameHandler(PacketSession _refSession, IPacket _iPacket)
    {
        S_BoradcastEnterGame pkt = _iPacket as S_BoradcastEnterGame;
        ServerSession serverSession = _refSession as ServerSession;
    }

    public static void S_BroadcastLeveGameHandler(PacketSession _refSession, IPacket _iPacket)
    {
        S_BroadcastLeveGame pkt = _iPacket as S_BroadcastLeveGame;
        ServerSession serverSession = _refSession as ServerSession;
    }

    public static void S_PlayerListHandler(PacketSession _refSession, IPacket _iPacket)
    {
        S_PlayerList pkt = _iPacket as S_PlayerList;
        ServerSession serverSession = _refSession as ServerSession;
    }

    public static void S_BroadcastMoveHandler(PacketSession _refSession, IPacket _iPacket)
    {
        S_BroadcastMove pkt = _iPacket as S_BroadcastMove;
        ServerSession serverSession = _refSession as ServerSession;
    }
}

