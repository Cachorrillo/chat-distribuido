using System.Net.Sockets;
using System.Text;

var servers = new List<(string Host, int Port, string Name)>
{
    ("127.0.0.1", 5000, "MAIN"),
    ("127.0.0.1", 5001, "BACKUP")
};

TcpClient? client = null;
NetworkStream? stream = null;
CancellationTokenSource? listenCts = null;

while (true)
{
    if (client == null || !client.Connected)
    {
        (client, stream) = await ConnectToAvailableServerAsync(servers);

        if (client == null || stream == null)
        {
            Console.WriteLine("No se pudo conectar a ningún servidor. Reintentando en 3 segundos...");
            await Task.Delay(3000);
            continue;
        }

        listenCts = new CancellationTokenSource();
        _ = Task.Run(() => ListenForMessagesAsync(client, stream, listenCts.Token));
    }

    Console.Write("Tú: ");
    string? message = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(message))
        continue;

    try
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        await stream.WriteAsync(data, 0, data.Length);
    }
    catch
    {
        Console.WriteLine("Se perdió la conexión. Intentando reconectar...");
        listenCts?.Cancel();
        client.Close();
        client = null;
        stream = null;
    }
}

static async Task<(TcpClient?, NetworkStream?)> ConnectToAvailableServerAsync(
    List<(string Host, int Port, string Name)> servers)
{
    foreach (var server in servers)
    {
        try
        {
            var client = new TcpClient();
            await client.ConnectAsync(server.Host, server.Port);

            Console.WriteLine($"Conectado a {server.Name} ({server.Host}:{server.Port})");
            return (client, client.GetStream());
        }
        catch
        {
            Console.WriteLine($"No disponible: {server.Name} ({server.Host}:{server.Port})");
        }
    }

    return (null, null);
}

static async Task ListenForMessagesAsync(
    TcpClient client,
    NetworkStream stream,
    CancellationToken token)
{
    byte[] buffer = new byte[1024];

    try
    {
        while (!token.IsCancellationRequested)
        {
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);

            if (bytesRead == 0)
            {
                Console.WriteLine("\nConexión cerrada por el servidor.");
                break;
            }

            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"\n{message}");
            Console.Write("Tú: ");
        }
    }
    catch (OperationCanceledException)
    {
    }
    catch
    {
        Console.WriteLine("\nSe perdió la conexión con el servidor.");
    }
}