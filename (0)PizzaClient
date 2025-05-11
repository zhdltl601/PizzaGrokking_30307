using System.Net.Sockets;
using System.Text;

namespace _0_SimpleClient
{
    internal class PizzaClient
    {
        private const int BUFFER_SIZE = 1024;   // 데이터 송수신 최대 길이
        private const string SERVER_ADDRESS = "127.0.0.1";
        private const int SERVER_PORT = 12345;

        static void Main()
        {
            try
            {   // 클라이언트 소켓 생성
                using TcpClient client = new TcpClient(SERVER_ADDRESS, SERVER_PORT);
                NetworkStream stream = client.GetStream();

                while (true)
                {
                    Console.Write("How many pizzas do you want? ");
                    string? order = Console.ReadLine();                     // 피자 주문 입력

                    if (string.IsNullOrEmpty(order))
                        break;

                    byte[] dataToSend = Encoding.UTF8.GetBytes(order);      // 바이트 변환
                    stream.Write(dataToSend, 0, dataToSend.Length);         // 데이터 전송

                    byte[] buffer = new byte[BUFFER_SIZE];
                    int bytesRead = stream.Read(buffer, 0, BUFFER_SIZE);    // 데이터 수신
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead).TrimEnd();      // 바이트 변환

                    Console.WriteLine($"Server replied '{response}'");      // 주문 결과 출력
                }

                Console.WriteLine("Client closing");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
            }
        }
    }
}
