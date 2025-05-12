using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Server
{
    private const int BUFFER_SIZE = 1024;//촤대 버퍼 사이즈
    // IP주소, 포트번호
    private static readonly IPEndPoint ADDRESS = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);//주소 : 127.0.0.1, 포트 : 12345
    private TcpListener serverSocket;

    public Server()     // 서버 소켓 생성
    {
        try
        {
            Console.WriteLine($"Starting up at: {ADDRESS}");
            serverSocket = new TcpListener(ADDRESS);
            serverSocket.Start();
        }
        catch (SocketException)
        {
            Console.WriteLine("\nServer failed to start.");
            serverSocket?.Stop();
        }
    }

    public TcpClient Accept()   // 클라이언트 서버 접속 대기 및 클라이언트 소켓 반환
    {
        TcpClient client = serverSocket.AcceptTcpClient();                      //TcpListener에서 client accept
        IPEndPoint clientEndPoint = client.Client.RemoteEndPoint as IPEndPoint; // IP가져오기
        Console.WriteLine($"Connected to {clientEndPoint}");
        return client;
    }

    public void Serve(TcpClient client)     // 클라가 보내는 데이터 수신 및 서버의 응답 송신
    {
        NetworkStream stream = client.GetStream();                                      //클라이언트 스트림
        byte[] buffer = new byte[BUFFER_SIZE];

        try
        {
            while (true)//여기서 스레드 멈춰서 새 클라이언트 받지 못함.
            {
                int bytesRead = stream.Read(buffer, 0, BUFFER_SIZE);                    //클라이언트에서 받은 스트림 읽기
                if (bytesRead == 0) break;                                              //데이터 크기 0일때 종료 (근데 0이면 클라가 안보냄)

                //역직렬화
                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);    //데이터를 String으로 변환
                string response;                                                        //클라한테 보낼 메세지

                if (int.TryParse(receivedData, out int order))                          //클라가 보낸 string 데이터를 숫자로 변환
                {
                    response = $"Thank you for ordering {order} pizzas!\n";             //클라한테 보낼 메세지 업데이트
                }
                else
                {
                    response = "Wrong number of pizzas, please try again\n";            //문자열 치환 실패 시 오류 메세지
                }

                //직렬화 후 보내기
                Console.WriteLine($"Sending message to {client.Client.RemoteEndPoint}");
                byte[] responseData = Encoding.UTF8.GetBytes(response);
                stream.Write(responseData, 0, responseData.Length);                     //클라에게 보내기
            }
        }
        finally
        {
            Console.WriteLine($"Connection with {client.Client.RemoteEndPoint} has been closed");
            client.Close();
        }
    }

    public void Start() // 클라 연결 대기를 위한 loop문
    {
        Console.WriteLine("Server listening for incoming connections");

        try
        {
            while (true)
            {
                TcpClient client = Accept();    //클라이언트 받기, 멈춤
                Serve(client);                  //while 문에서 서버처리
            }
        }
        finally
        {
            serverSocket.Stop();
            Console.WriteLine("\nServer stopped.");
        }
    }

    static void Main(string[] args) // Main문 서버 시작
    {
        Server server = new Server();//서버생성
        server.Start();
    }
}
