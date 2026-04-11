using System.Net;
using System.Net.Sockets;
using System.Text;

const int puerto = 5000;

TcpListener servidor = new TcpListener(IPAddress.Any, puerto);
servidor.Start();

Console.WriteLine($"[SERVIDOR] Escuchando en el puerto {puerto}...");

// Lista de clientes conectados
List<TcpClient> clientes = new List<TcpClient>();

while (true)
{
    TcpClient cliente = servidor.AcceptTcpClient();
    clientes.Add(cliente);

    IPEndPoint? endPoint = cliente.Client.RemoteEndPoint as IPEndPoint;

    Console.WriteLine($"[SERVIDOR] Cliente conectado desde {endPoint?.Address}:{endPoint?.Port}");

    _ = Task.Run(() => ManejarCliente(cliente, clientes));
}

static void ManejarCliente(TcpClient cliente, List<TcpClient> clientes)
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
                break;

            Console.WriteLine($"[MENSAJE] {mensaje}");

            // Enviar a todos los clientes
            lock (clientes)
            {
                foreach (var c in clientes)
                {
                    try
                    {
                        if (c.Connected)
                        {
                            NetworkStream ns = c.GetStream();
                            StreamWriter w = new StreamWriter(ns, Encoding.UTF8) { AutoFlush = true };
                            w.WriteLine(mensaje);
                        }
                    }
                    catch
                    {
                        // Ignorar errores de clientes desconectados
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] {ex.Message}");
    }
    finally
    {
        Console.WriteLine("[SERVIDOR] Cliente desconectado.");
        clientes.Remove(cliente);
        cliente.Close();
    }
}