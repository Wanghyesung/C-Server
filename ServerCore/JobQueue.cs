using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public interface IJobQeueu
    {
        void Push(Action _refJob);
    }
    public class JobQueue : IJobQeueu
    {
        private Queue<Action> m_qeueuJob = new Queue<Action>();
        private object m_lock = new object();
        bool m_bFlush = false;

        //먼저 들어온 스레드가 담당해서 계속 실행 하나의 스레드만 계속 당당
        public void Push(Action _refJob)
        {
            bool bFlush = false;
            lock (m_lock)
            {
                m_qeueuJob.Enqueue(_refJob);
                if(m_bFlush == false)
                {
                    m_bFlush=true;  bFlush = true;
                }
            }

            if (bFlush)
                Flush();
        }

        void Flush()
        {
            while(true)
            {
                Action refAction = Pop();
                if (refAction == null)
                    return;

                refAction.Invoke();
            }
        }

        public Action Pop()
        {
            lock(m_lock)
            {
                if (m_qeueuJob.Count == 0)
                {
                    m_bFlush = false;
                    return null;
                }

                return m_qeueuJob.Dequeue();
            }
        }
    }
}
