using _4_Pizza_Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4_Pizza_Event
{
    internal class Program
    {
        static void Main()
        {
            EventLoop eventLoop = new EventLoop();                  //이벤트 루프 생성
            EventServer server = new EventServer(eventLoop);        //서버 생성
            server.Start();                                         //서버 시작
            eventLoop.RunForever();                                 //이벤트 loop while 문 처리
        }
    }
}
