using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{

    public class PriorityQueue<T> where T : IComparable<T>
    {
        List<T> m_listHeap = new List<T>();
        public int Count() { return m_listHeap.Count; }

        public void Push(T _value)
        {
            m_listHeap.Add(_value);

            int iNow = m_listHeap.Count - 1;

            while(iNow > 0 )
            {
                int iNext = (iNow - 1) / 2;
                if (m_listHeap[iNow].CompareTo(m_listHeap[iNext]) < 0)
                    break;

                //교체
                T temValue = m_listHeap[iNow];
                m_listHeap[iNow] = m_listHeap[iNext];
                m_listHeap[iNext] = temValue;

                iNow = iNext;
            }
        }

        public T Pop()
        {
            
            T retValue = m_listHeap[0];

            int iLastIdx = m_listHeap.Count - 1;
            m_listHeap[0] = m_listHeap[iLastIdx];
            m_listHeap.RemoveAt(iLastIdx);
            --iLastIdx;

            int iNow = 0;

            while(true)
            {
                int iLeft = 2 * iNow + 1;
                int iRight = 2 * iNow + 2;

                int iNext = iNow;

                //왼쪽값이 현재값보다 크면 왼쪽으로 이동
                if (iLeft <= iLastIdx && m_listHeap[iNext].CompareTo(m_listHeap[iLeft]) < 0)
                    iNext = iLeft;

                if (iRight <= iLastIdx && m_listHeap[iNext].CompareTo(m_listHeap[iRight]) < 0)
                    iNext = iRight;

                if (iNext == iNow)
                    break;

                T temValue = m_listHeap[iNow];
                m_listHeap[iNow] = m_listHeap[iNext];
                m_listHeap[iNext] = temValue;

                iNow = iNext;
            }
            return retValue;
        }

        public T Peek()
        {
            if (m_listHeap.Count == 0)
                return default(T);

            return m_listHeap[0];
        }
    }
}
