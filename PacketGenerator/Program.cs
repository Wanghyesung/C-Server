
using System;
using System.Xml;

namespace PacketGenerator
{
    class Programm
    {
        
        static string m_strGenPackets;
        static ushort m_iPacketID; 
        static string m_strPacketEnum;


        static string m_strClientRegister;
        static string m_strServerRegister;
        static void Main(string[] args)
        {
            string strPDLPath = "../PDL.xml";

            XmlReaderSettings refSetting = new XmlReaderSettings();
            //주석 스페이스 무시
            refSetting.IgnoreComments = true;
            refSetting.IgnoreWhitespace = true;

            if(args.Length>=1)
            {
                strPDLPath = args[0];
            }

            using (XmlReader xmlReader =  XmlReader.Create(strPDLPath, refSetting))
            {
                xmlReader.MoveToContent(); //본문으로 바로 이동

                while(xmlReader.Read())//한줄씩
                {
                    //깊이가 1이고 패킷의 정보 시작이라면
                    if (xmlReader.Depth == 1 && xmlReader.NodeType == XmlNodeType.Element)
                        ParsePacket(xmlReader);

                    //해당 타입과 name을 뽑아온다
                   // Console.WriteLine(xmlReader.Name + " " + xmlReader["name"]);
                }

                string strFileText = string.Format(PacketFormat.strFileFormat, m_strPacketEnum, m_strGenPackets);
                File.WriteAllText("GenPackets.cs", strFileText);
                string strClientMmanagerText = string.Format(PacketFormat.strManagerFormat, m_strClientRegister);
                File.WriteAllText("ClientPacketManager.cs", strClientMmanagerText);

                string strServerMmanagerText = string.Format(PacketFormat.strManagerFormat, m_strServerRegister);
                File.WriteAllText("ServerPacketManager.cs", strServerMmanagerText);

            }
        }

        public static void ParsePacket(XmlReader _reader)
        {
            if (_reader.NodeType == XmlNodeType.EndElement)
                return;

            if (_reader.Name.ToLower() != "packet")
                return;

            string strPacketName = _reader["name"];
            if(string.IsNullOrEmpty(strPacketName))
            {
                Console.WriteLine("packet without name");
                return;
            }

            Tuple<string, string, string> tMembers = ParseMembers(_reader);
            m_strGenPackets += String.Format(PacketFormat.strPacketFormat,
                strPacketName, tMembers.Item1, tMembers.Item2, tMembers.Item3);
            m_strPacketEnum += string.Format(PacketFormat.strPacketEnumFormat, strPacketName, ++m_iPacketID) + Environment.NewLine + "\t";

            //서버에서 클라로
            if(strPacketName.StartsWith("S_") || strPacketName.StartsWith("s_"))
                m_strClientRegister += string.Format(PacketFormat.strManagerRegisterFormat, strPacketName) + Environment.NewLine;
            else
                m_strServerRegister += string.Format(PacketFormat.strManagerRegisterFormat, strPacketName) + Environment.NewLine;
        }
        //1 : 멤버 변수들
        //2 : 멤버 변수 read
        //3 : 멤버 변수 write
        public static Tuple<string, string, string> ParseMembers(XmlReader _reader)
        {
            string strPacketName = _reader["name"];

            string strMemberCode = "";
            string strReadCode = "";
            string strWriteCode = "";

            int iDepth = _reader.Depth + 1; //패킷시작의 다음부터(속성) 파싱
            
            while (_reader.Read())
            {
                if (_reader.Depth != iDepth)
                    break;

                string strMemberName = _reader["name"];
                if (string.IsNullOrEmpty(strMemberName))
                {
                    Console.WriteLine("Member without name");
                    return null;
                }

                if (string.IsNullOrEmpty(strMemberCode) == false)
                    strMemberCode += Environment.NewLine;
                if (string.IsNullOrEmpty(strReadCode) == false)
                    strReadCode += Environment.NewLine;
                if (string.IsNullOrEmpty(strWriteCode) == false)
                    strWriteCode += Environment.NewLine;
               
                string strMemberType = _reader.Name.ToLower();
                switch(strMemberType)
                {
                    case "byte":
                    case "sbyte":
                        strMemberCode += string.Format(PacketFormat.strMemberFormat, strMemberType, strMemberName);
                        strReadCode += string.Format(PacketFormat.strReadByteFormat, strMemberName, strMemberType);
                        strWriteCode += string.Format(PacketFormat.strWriteByteFormat, strMemberName, strMemberType);
                        break;
                    case "bool":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                        strMemberCode += string.Format(PacketFormat.strMemberFormat, strMemberType, strMemberName);
                        strReadCode += string.Format(PacketFormat.strReadFormat, strMemberName, ToMemeberType(strMemberType), strMemberType);
                        strWriteCode += string.Format(PacketFormat.strWriteFormat, strMemberName, strMemberType);
                        break;
                    case "string":
                        strMemberCode += string.Format(PacketFormat.strMemberFormat, strMemberType, strMemberName);
                        strReadCode += string.Format(PacketFormat.strReadStringFormat, strMemberName);
                        strWriteCode += string.Format(PacketFormat.strWriteStringFormat, strMemberName);
                        break;
                    case "list":
                        Tuple<string, string, string> tTu = ParseList(_reader);
                        strMemberCode += tTu.Item1;
                        strReadCode += tTu.Item2;
                        strWriteCode += tTu.Item3;
                        break;
                    default:
                        break;
                }
            }
            strMemberCode = strMemberCode.Replace("\n", "\n\t");
            strReadCode = strReadCode.Replace("\n", "\n\t\t");
            strWriteCode = strWriteCode.Replace("\n", "\n\t\t");
            return new Tuple<string, string, string>(strMemberCode, strReadCode, strWriteCode);

        }

        public static string ToMemeberType(string strMemberType)
        {
            switch (strMemberType)
            {
                case "bool":
                    return "ToBoolean";
                case "short":
                    return "ToInt16";
                case "ushort":
                    return "ToUInt16";
                case "int":
                    return "ToInt32";
                case "long":
                    return "ToInt64";
                case "float":
                    return "ToSingle";
                case "double":
                    return "ToDouble";
                default:
                    return "";
            }
        }

        public static Tuple<string, string, string> ParseList(XmlReader _reader)
        {
            string strListName = _reader["name"];
            if(string.IsNullOrEmpty(strListName))
            {
                Console.WriteLine("List without name");
                return null;
            }

            Tuple<string,string,string> tMem = ParseMembers(_reader);
            string strMemCode = string.Format(PacketFormat.strMemberListFormat,
                FirstCharToUpper(strListName), FirstCharToLower(strListName),
                tMem.Item1,tMem.Item2,tMem.Item3);

            string strReadCode = string.Format(PacketFormat.strReadListFormat,
                FirstCharToUpper(strListName), FirstCharToLower(strListName));

            string strWriteCode = string.Format(PacketFormat.strWriteListFormat,
              FirstCharToUpper(strListName), FirstCharToLower(strListName));

            return new Tuple<string, string, string>(strMemCode, strReadCode, strWriteCode);
        }

        public static string FirstCharToUpper(string strInput)
        {
            if (string.IsNullOrEmpty(strInput))
                return "";

            return strInput[0].ToString().ToUpper() + strInput.Substring(1);
        }

        public static string FirstCharToLower(string strInput)
        {
            if (string.IsNullOrEmpty(strInput))
                return "";

            return strInput[0].ToString().ToLower() + strInput.Substring(1);
        }
    }

}