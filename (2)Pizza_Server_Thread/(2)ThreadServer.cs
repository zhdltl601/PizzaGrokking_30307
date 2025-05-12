using System.Net.Sockets;
using System.Net;
using System.Text;

namespace _2_Pizza_Thread
{
    internal class Handler
    {
        private const int BUFFER_SIZE = 1024;
        private readonly TcpClient client;

        public Handler(TcpClient client)    // 각 스레드마다 클라이언트 소켓을 부여받는다.
        {
            this.client = client;
        }

        public void Run()   // 스레드마다 서버의 역할 수행
        {
            IPEndPoint clientEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            Console.WriteLine($"Connected to {clientEndPoint}");

            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[BUFFER_SIZE];

            try
            {
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, BUFFER_SIZE);                //클라이언트에서 받은 스트림 읽기
                    if (bytesRead == 0) break;                                          //데이터 크기 0일때 종료 (근데 0이면 클라가 안보냄)

                    string received = Encoding.UTF8.GetString(buffer, 0, bytesRead);    //데이터를 String으로 변환
                    string response;                                                    //클라한테 보낼 메세지

                    if (int.TryParse(received, out int order))                          //클라가 보낸 string 데이터를 숫자로 변환
                    {
                        response = $"Thank you for ordering {order} pizzas!\n";         //클라한테 보낼 메세지 업데이트
                    }
                    else
                    {
                        response = "Wrong number of pizzas, please try again\n";        //문자열 치환 실패 시 오류 메세지
                    }

                    //직렬화 후 보내기
                    Console.WriteLine($"Sending message to {clientEndPoint}");
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseData, 0, responseData.Length);                 //클라에게 보내기
                }
            }
            finally
            {
                Console.WriteLine($"Connection with {clientEndPoint} has been closed");
                client.Close();
            }
        }
    }

    internal class ThreadServer
    {
        private const int PORT = 12345;
        private readonly TcpListener server;

        public ThreadServer()   // 서버 소켓 세팅
        {
            try
            {
                IPEndPoint localAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), PORT);   //주소 : 127.0.0.1, 포트 : 12345
                Console.WriteLine($"Starting up at: {localAddress}");
                server = new TcpListener(localAddress);                                         //listener 설정
                server.Start();
            }
            catch (SocketException)
            {
                server?.Stop();
                Console.WriteLine("\nServer stopped.");
            }
        }

        public void Start()
        {
            Console.WriteLine("Server listening for incoming connections");

            try
            {
                while (true)//while 문에서 클라이언트 입력만 받기, 전송은 다른 스레드에서. 그러므로 메인스레드에서 입력 안막힘
                {
                    TcpClient client = server.AcceptTcpClient();                                            //받을 때 까지 대기
                    Console.WriteLine($"Client connection request from {client.Client.RemoteEndPoint}");

                    Handler handler = new Handler(client);                                                  //Handler 생성
                    Thread thread = new Thread(new ThreadStart(handler.Run));                               //새 스레드를 생성하여 그 스레드에서 전송 하기.
                    thread.Start();
                } // 클라이언트 요청이 들어올 때마다 새로운 스레드를 생성한다.
            }
            finally
            {
                server.Stop();
                Console.WriteLine("\nServer stopped.");
            }
        }
        static void Main()
        {
            ThreadServer server = new ThreadServer();   //서버 생성
            server.Start();                             //서버 시작
        }
    }
}
