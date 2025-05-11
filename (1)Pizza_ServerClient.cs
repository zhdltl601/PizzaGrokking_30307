    internal class Server
    {
        private const int BUFFER_SIZE = 1024;
        // IP주소, 포트번호
        private static readonly IPEndPoint ADDRESS = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
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
            TcpClient client = serverSocket.AcceptTcpClient();
            IPEndPoint clientEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            Console.WriteLine($"Connected to {clientEndPoint}");
            return client;
        }

        public void Serve(TcpClient client)     // 클라가 보내는 데이터 수신 및 서버의 응답 송신
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[BUFFER_SIZE];

            try
            {
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, BUFFER_SIZE);
                    if (bytesRead == 0) break;

                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string response;

                    if (int.TryParse(receivedData, out int order))
                    {
                        response = $"Thank you for ordering {order} pizzas!\n";
                    }
                    else
                    {
                        response = "Wrong number of pizzas, please try again\n";
                    }

                    Console.WriteLine($"Sending message to {client.Client.RemoteEndPoint}");
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseData, 0, responseData.Length);
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
                    TcpClient client = Accept();
                    Serve(client);
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
            Server server = new Server();
            server.Start();
        }
    }
