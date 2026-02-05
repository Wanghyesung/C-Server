
using System;
using ServerCore;

class PacketManager
{
    static PacketManager m_Instance = new PacketManager();
    public static PacketManager Instance{ get{return m_Instance;} }

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> m_OnRecv = new();
    Dictionary<ushort, Action<PacketSession, IPacket>> m_Handler = new();

    private PacketManager()
    {
        Register();
    }

    public void Register()
    {

    m_OnRecv.Add((ushort)PacketID.C_Chat, MakePacket<C_Chat>);
    m_Handler.Add((ushort)PacketID.C_Chat, PacketHandler.C_ChatHandler);


    }

    public void OnRecvPakcet(PacketSession _refSession, ArraySegment<byte> _arrBuffer)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(_arrBuffer.Array, _arrBuffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(_arrBuffer.Array, _arrBuffer.Offset + count);
        count += 2;

        Action<PacketSession, ArraySegment<byte>> refAction = null;
        if (m_OnRecv.TryGetValue(id, out refAction))
            refAction.Invoke(_refSession, _arrBuffer);
    }

    private void MakePacket<T>(PacketSession _refSession, ArraySegment<byte> _arrBuffer) where T : IPacket, new()
    {
        T pkt = new T();
        pkt.Read(_arrBuffer);

        Action<PacketSession, IPacket> refAction = null;
        if (m_Handler.TryGetValue(pkt.Protocol, out refAction))
            refAction.Invoke(_refSession, pkt);
    }

}
