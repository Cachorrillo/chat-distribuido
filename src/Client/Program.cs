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

bool connectionLost = true;
object stateLock = new();

_ = Task.Run(async () =>
{
    while (true)
    {
        bool shouldReconnect;

        lock (stateLock)
        {
            shouldReconnect = connectionLost;
        }

        if (shouldReconnect)
        {
            Console.WriteLine("\nIntentando conectar o reconectar...");

            var result = await ConnectToAvailableServerAsync(servers);

            if (result.Client != null && result.Stream != null)
            {
                lock (stateLock)
                {
                    client = result.Client;
                    stream = result.Stream;
                    connectionLost = false;

                    listenCts?.Cancel();
                    listenCts = new CancellationTokenSource();
                    _ = Task.Run(() => ListenForMessagesAsync(client, stream, listenCts.Token, () =>
                    {
                        lock (stateLock)
                        {
                            connectionLost = true;
                        }
                    }));
                }
            }
            else
            {
                Console.WriteLine("No se pudo conectar a ningún servidor. Reintentando en 3 segundos...");
                await Task.Delay(3000);
            }
        }

        await Task.Delay(500);
    }
});

while (true)
{
    Console.Write("Tú: ");
    string? message = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(message))
        continue;

    TcpClient? currentClient;
    NetworkStream? currentStream;
    bool lost;

    lock (stateLock)
    {
        currentClient = client;
        currentStream = stream;
        lost = connectionLost;
    }

    if (lost || currentClient == null || currentStream == null || !currentClient.Connected)
    {
        Console.WriteLine("No hay conexión activa. Espera la reconexión automática...");
        continue;
    }

    try
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        await currentStream.WriteAsync(data, 0, data.Length);
    }
    catch
    {
        Console.WriteLine("Se perdió la conexión. Esperando reconexión automática...");
        lock (stateLock)
        {
            connectionLost = true;
        }
    }
}

static async Task<(TcpClient? Client, NetworkStream? Stream)> ConnectToAvailableServerAsync(
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
    CancellationToken token,
    Action onConnectionLost)
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
                onConnectionLost();
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
        onConnectionLost();
    }
}