using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    struct tJobTimerElem : IComparable<tJobTimerElem>
    {
        public int iExecTick;
        public Action refAction;
        public int CompareTo(tJobTimerElem other)
        {
            //틱이 작을수록 먼저
            return other.iExecTick - iExecTick;
        }
    }
    class JobTimer
    {
        private PriorityQueue<tJobTimerElem> m_pqTimer = new PriorityQueue<tJobTimerElem>();
        private object m_lock = new object();

        public static JobTimer m_Instance { get; } = new JobTimer();


        public void Push(Action _refAction, int _iTick = 0)
        {
            tJobTimerElem tJob;
            tJob.iExecTick = System.Environment.TickCount + _iTick; //다음 실행 시간
            tJob.refAction = _refAction;

            lock(m_lock)
            {
                m_pqTimer.Push(tJob);
            }
        }

        public void Flush()
        {
            while(true)
            {
                int iNow = System.Environment.TickCount;
                tJobTimerElem tJob;

                lock(m_lock)
                {
                    if (m_pqTimer.Count() == 0)
                        break;

                    tJob = m_pqTimer.Peek();
                    if (tJob.iExecTick > iNow)
                        break;

                    m_pqTimer.Pop();
                }
                tJob.refAction.Invoke();
            }
        }
    }
}
