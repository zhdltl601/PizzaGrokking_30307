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
            EventLoop eventLoop = new EventLoop();                  //�̺�Ʈ ���� ����
            EventServer server = new EventServer(eventLoop);        //���� ����
            server.Start();                                         //���� ����
            eventLoop.RunForever();                                 //�̺�Ʈ loop while �� ó��
        }
    }
}
