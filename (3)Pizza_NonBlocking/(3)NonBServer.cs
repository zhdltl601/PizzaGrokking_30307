using System.Net;
using System.Net.Sockets;
using System.Text;

namespace _3_Pizza_NonB
{
    internal class NonBServer
    {
        private const int BUFFER_SIZE = 1024;
        private const int PORT = 12345;
        private Socket serverSocket;
        private readonly List<Socket> clients = new();

        public NonBServer()
        {
            try
            {
                Console.WriteLine($"Starting up at: 127.0.0.1:{PORT}");

                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(IPAddress.Loopback, PORT));
                serverSocket.Listen(1000);      // 클라이언트 소켓 대기열 최대 1000 
                serverSocket.Blocking = false;  // 서버 소켓 : 블로킹 off!
            }
            catch (SocketException)
            {
                serverSocket?.Close();
                Console.WriteLine("\nServer failed to start.");
            }
        }

        private void Accept()
        {
            try
            {
                Socket clientSocket = serverSocket.Accept();    //클라이언트 받기
                clientSocket.Blocking = false;                  // 클라이언트소켓 : 블로킹 off로 지정
                clients.Add(clientSocket);                      //콜렉션에 추가
                Console.WriteLine($"Connected to {clientSocket.RemoteEndPoint}");
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.WouldBlock)  //블록할때 무시하기
            {
                // no client waiting to be accepted — just continue
            }
        }

        private void Serve(Socket client)
        {
            byte[] buffer = new byte[BUFFER_SIZE];

            try
            {
                int bytesRead = client.Receive(buffer);                 //client 읽기
                if (bytesRead == 0)                                     //메세지 없을경우 지우기
                {
                    clients.Remove(client);
                    client.Close();
                    return;
                }

                string received = Encoding.UTF8.GetString(buffer, 0, bytesRead);    //받은 메세지 deserialize
                string response;

                if (int.TryParse(received, out int order))                          //받은 메세지 숫자로 치환 시도
                {
                    response = $"Thank you for ordering {order} pizzas!\n";         //보낼 메세지 업데이트
                }
                else
                {
                    response = "Wrong number of pizzas, please try again\n";        //받은 메세지 숫자 치환 실패시 보낼 메세지 업데이트
                }

                Console.WriteLine($"Sending message to {client.RemoteEndPoint}");
                byte[] responseData = Encoding.UTF8.GetBytes(response);             //보낼 메세지 serialize
                client.Send(responseData);                                          //클라이언트한테 보내기
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.WouldBlock)  //블록할때 무시하기
            {
                // no client waiting to be accepted — just continue
            }
        }
        public void Start()
        {
            Console.WriteLine("Server listening for incoming connections");

            try
            {
                while (true)            // 폴링 반복문
                {                       // 입출력 연산(accept(), send(), read() 이 성공할 때까지 연산 반복
                    Accept();           // 만약 소켓에 데이터가 없으면 대기하지 않고 넘어감

                    foreach (Socket client in new List<Socket>(clients))//클라이언 리스트의 모든 원소에 서버 처리 로직하기
                    {
                        Serve(client);      // 만약 소켓에 데이터가 없으면 대기하지 않고 넘어감
                    }

                    Thread.Sleep(1);        // avoid 100% CPU usage
                }
            }
            finally
            {
                serverSocket.Close();
                Console.WriteLine("\nServer stopped.");
            }
        }
        static void Main()
        {
            NonBServer server = new NonBServer();       //서버 생성
            server.Start();                             //서버 시작
        }
    }
}
