using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Map
    {
        //맵 시작 위치 (좌하단), 맵 크기, 유닛 사이즈
        private Vector2 m_vOrigin;
        private Vector2 m_vSize;
        private float m_fCellSize;

        public byte[] Grid { get; private set; }

        public void Init(string _strMapPath)
        {
            string[] strLine = File.ReadAllLines(_strMapPath);

            if (strLine.Length < 3)
                throw new Exception("Invalid map file (header too short)");

            ParseOrigin(strLine[0]);

            m_fCellSize = ParseSingleValue(strLine[1], "cell");

            ParseSize(strLine[2]);

            Grid = new byte[(int)m_vSize.Y * (int)m_vSize.X];

            // row lines: 3 .. 3+Height-1 (z = 0..Height-1)
            for (int z = 0; z < (int)m_vSize.Y; z++)
            {
                string row = strLine[3 + z];

                // 공백/탭 섞였을 수도 있으니 제거 (없으면 이 줄 삭제해도 됨)
                row = row.Trim();

                if (row.Length < (int)m_vSize.X)
                    throw new Exception("Invalid map row length at z=" + z);

                int baseIndex = z * (int)m_vSize.X;
                for (int x = 0; x < (int)m_vSize.X; x++)
                {
                    char c = row[x];
                    Grid[baseIndex + x] = (byte)(c == '1' ? 1 : 0);
                }
            }
        }

        private void ParseOrigin(string strLine)
        {
            // "origin -50.000 -20.000"
            string[] strValue = strLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
          
            float x = float.Parse(strValue[1], CultureInfo.InvariantCulture);
            float z = float.Parse(strValue[2], CultureInfo.InvariantCulture);
            m_vOrigin = new Vector2(x, z);
        }

        private float ParseSingleValue(string strLine, string key)
        {
            // "cell 0.250"
            string[] strValue = strLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            return float.Parse(strValue[1], CultureInfo.InvariantCulture);
        }

        private void ParseSize(string strLine)
        {
            // "size 480 320"
            string[] strValue = strLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);


            float w = int.Parse(strValue[1], CultureInfo.InvariantCulture);
            float h = int.Parse(strValue[2], CultureInfo.InvariantCulture);
            m_vSize = new Vector2(w, h);
        }

        public bool CanGo(float _x, float _z)
        {
            int gridX = (int)Math.Floor((_x - m_vOrigin.X) / m_fCellSize);
            int gridZ = (int)Math.Floor((_z - m_vOrigin.Y) / m_fCellSize);

            if (gridX < 0 || gridX <= _x || gridZ < 0 || gridZ <= _z)
                return false;

            return Grid[gridZ * (int)m_vSize.X + gridX] == 1;
        }


        public Vector2 ClamToLastVaild(float _x, float _z, float _gx, float _gy, MoveDir _refMove)
        {
            Vector2 vRetValue = new Vector2(_x, _z);

            //내가 왔던 방향의 반대 방향으로 체크
            while(CanGo(_x,_z) == false)
            {
                _x += (m_fCellSize * -_refMove.DirX);
                _z += (m_fCellSize * -_refMove.DirZ);

                vRetValue = new Vector2(_x,_z);
            }

            return vRetValue;
        }
    }
}
