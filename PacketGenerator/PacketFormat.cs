using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PacketGenerator
{
    class PacketFormat
    {

        //{0} 패킷 이름
        public static string strManagerRegisterFormat =
@"
    m_OnRecv.Add((ushort)PacketID.{0}, MakePacket<{0}>);
    m_Handler.Add((ushort)PacketID.{0}, PacketHandler.{0}Handler);
";

        //{0}패킷 등록
        public static string strManagerFormat =
@"
using System;
using ServerCore;

class PacketManager
{{
    static PacketManager m_Instance = new PacketManager();
    public static PacketManager Instance{{ get{{return m_Instance;}} }}

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> m_OnRecv = new();
    Dictionary<ushort, Action<PacketSession, IPacket>> m_Handler = new();

    private PacketManager()
    {{
        Register();
    }}

    public void Register()
    {{
{0}
    }}

    public void OnRecvPakcet(PacketSession _refSession, ArraySegment<byte> _arrBuffer)
    {{
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(_arrBuffer.Array, _arrBuffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(_arrBuffer.Array, _arrBuffer.Offset + count);
        count += 2;

        Action<PacketSession, ArraySegment<byte>> refAction = null;
        if (m_OnRecv.TryGetValue(id, out refAction))
            refAction.Invoke(_refSession, _arrBuffer);
    }}

    private void MakePacket<T>(PacketSession _refSession, ArraySegment<byte> _arrBuffer) where T : IPacket, new()
    {{
        T pkt = new T();
        pkt.Read(_arrBuffer);

        Action<PacketSession, IPacket> refAction = null;
        if (m_Handler.TryGetValue(pkt.Protocol, out refAction))
            refAction.Invoke(_refSession, pkt);
    }}

}}
";


        //{0} 패킷 이름/번호 목록
        //{1} 패킷 목록
        public static string strFileFormat =
@"using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

public enum PacketID
{{
    {0}
}}

interface IPacket
{{
	ushort Protocol {{ get; }}
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}}


{1}
";
        //{0} 패킷 이름
        //{1} 패킷 번호
        public static string strPacketEnumFormat =
@"{0} = {1},";

        //{0} 패킷 이름
        //{1} 멤버 변수
        //{2} 멤버 변수 Read
        //{3} 멤버 변수 Write
        public static string m_strPacketFormat =
@"
class {0} : IPacket
{{
    {1}

    public ushort Protocol {{ get {{ return (ushort)PacketID.{0}; }} }}
    public void Read(ArraySegment<byte> _arrSeg)
    {{
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(_arrSeg.Array, _arrSeg.Offset, _arrSeg.Count);
        
        count +=sizeof(ushort);
        count +=sizeof(ushort);

        {2}
    }}
    
    public ArraySegment<byte> Write()
    {{
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;
        
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.{0});
        count += sizeof(ushort);
        
        {3}

        success &= BitConverter.TryWriteBytes(s, count);
        if(success == false)
            return null;

        return SendBufferHelper.Close(count);
    }}
 }}
";

        //{0} 변수 형식
        //{1} 변수 이름
        public static string strMemberFormat =
@"public {0} {1};";

        //{0} 리스트 이름, 
        //{1} 리스트 이름(소문자)
        //{2} 멤버 변수들
        //{3} 멤버 변수 Read
        //{4} 멤버 변수 Write
        public static string strMemberListFormat =
@"
public class {0}
{{
    {2}
    
    public void Read(ReadOnlySpan<byte> s, ref ushort count)
    {{
        {3}
    }}

    public bool Write(Span<byte> s, ref ushort count)
    {{
        bool success = true;
        {4}
        return success;
    }}
}}
public List<{0}> {1}s = new List<{0}>();";

        //{0} 뱐수 이름
        //{1} To~ 변수 형식
        //{2} 변수 형식
        public static string strReadFormat =
@"this.{0} = BitConverter.{1}(s.Slice(count, s.Length - count));
count += sizeof({2});";
        

        //{0} 변수 이름
        //{1} 변수 형식
        public static string strReadByteFormat =
@"this.{0} = ({1})segment.Array[segment.Offset + count];
count += sizeof({1});";


        //{0} 변수 이름
        public static string strReadStringFormat =
@"ushort {0}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(s.Slice(count, {0}Len));
count += {0}Len;";

        //{0} 리스트 이름 대문자
        //{1} 리스트 이름 소문자
        public static string strReadListFormat =
@"this.{1}s.Clear();
ushort {1}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
for(int i = 0; i<{1}Len; ++i)
{{
    {0} {1} = new {0}();
    {1}.Read(s, ref count);
    {1}s.Add({1});
}}";

        public static string strWriteFormat =
@"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.{0});
count += sizeof({1});";

        public static string strWriteByteFormat =
@"segment.Array[segment.Offset + count] = ({1})this.{0};
count += sizeof({1});";


        public static string strWriteStringFormat =
@"ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, segment.Array, segment.Offset + count + sizeof(ushort));
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0}Len);
count += sizeof(ushort);
count += {0}Len;";

        //{0} 리스트 이름 대문자
        //{1} 리스트 이름 소문자
        public static string strWriteListFormat =
@"
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.{1}s.Count);
count += sizeof(ushort);
foreach({0} {1} in this.{1}s)
    success &= {1}.Write(s, ref count);";

    }
}
