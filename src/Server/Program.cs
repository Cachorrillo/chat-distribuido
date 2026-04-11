using System.Net;
using System.Net.Sockets;
using System.Text;

const int puerto = 5000;

TcpListener servidor = new TcpListener(IPAddress.Any, puerto);
servidor.Start();

Console.WriteLine($"[SERVIDOR] Escuchando en el puerto {puerto}...");

while (true)
{
    TcpClient cliente = servidor.AcceptTcpClient();
    IPEndPoint? endPoint = cliente.Client.RemoteEndPoint as IPEndPoint;

    Console.WriteLine($"[SERVIDOR] Cliente conectado desde {endPoint?.Address}:{endPoint?.Port}");

    _ = Task.Run(() => ManejarCliente(cliente));
}

static void ManejarCliente(TcpClient cliente)
{
    try
    {
        using NetworkStream stream = cliente.GetStream();
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        writer.WriteLine("Conectado al servidor.");

        while (true)
        {
            string? mensaje = reader.ReadLine();

            if (mensaje == null)
            {
                break;
            }

            Console.WriteLine($"[MENSAJE] {mensaje}");
            writer.WriteLine($"Servidor recibió: {mensaje}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] {ex.Message}");
    }
    finally
    {
        Console.WriteLine("[SERVIDOR] Cliente desconectado.");
        cliente.Close();
    }
}