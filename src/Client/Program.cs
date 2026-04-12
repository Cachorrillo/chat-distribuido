using System.Net.Sockets;
using System.Text;
using System.Linq;

var servers = new List<(string Host, int Port, string Name)>
{
    ("server-main", 5000, "MAIN"),
    ("server-backup", 5001, "BACKUP")
};

TcpClient? client = null;
NetworkStream? stream = null;
CancellationTokenSource? listenCts = null;

bool connectionLost = true;
DateTime lastPongTime = DateTime.MinValue;
DateTime? disconnectionTime = null;
string? currentServerName = null;
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
                    currentServerName = result.Client.Client.RemoteEndPoint?.ToString()?.Contains(":5001") == true
                        ? "BACKUP"
                        : "MAIN";

                    if (disconnectionTime.HasValue)
                    {
                        TimeSpan reconnectTime = DateTime.Now - disconnectionTime.Value;
                        Console.WriteLine($"Tiempo de reconexión: {reconnectTime.TotalSeconds:F2} segundos");
                        disconnectionTime = null;
                    }

                    connectionLost = false;
                    lastPongTime = DateTime.Now;
                    HeartbeatState.LastPong = DateTime.Now;

                    listenCts?.Cancel();
                    listenCts = new CancellationTokenSource();

                    _ = Task.Run(() => ListenForMessagesAsync(client, stream, listenCts.Token, () =>
                    {
                        lock (stateLock)
                        {
                            if (!connectionLost)
                            {
                                disconnectionTime = DateTime.Now;
                            }

                            connectionLost = true;
                        }
                    }));

                    _ = Task.Run(() => HeartbeatLoopAsync(
                        () =>
                        {
                            lock (stateLock)
                            {
                                return (client, stream, connectionLost, lastPongTime);
                            }
                        },
                        () =>
                        {
                            lock (stateLock)
                            {
                                if (!connectionLost)
                                {
                                    disconnectionTime = DateTime.Now;
                                }

                                connectionLost = true;
                            }
                        },
                        listenCts.Token));

                    _ = Task.Run(() => FailbackLoopAsync(
                        servers,
                        () =>
                        {
                            lock (stateLock)
                            {
                                return (client, stream, connectionLost, currentServerName);
                            }
                        },
                        (newClient, newStream) =>
                        {
                            lock (stateLock)
                            {
                                Console.WriteLine("\nMAIN disponible nuevamente. Volviendo al servidor principal...");

                                client?.Close();

                                client = newClient;
                                stream = newStream;
                                currentServerName = "MAIN";
                                connectionLost = false;
                                lastPongTime = DateTime.Now;
                                HeartbeatState.LastPong = DateTime.Now;

                                listenCts?.Cancel();
                                listenCts = new CancellationTokenSource();

                                _ = Task.Run(() => ListenForMessagesAsync(client, stream, listenCts.Token, () =>
                                {
                                    lock (stateLock)
                                    {
                                        if (!connectionLost)
                                        {
                                            disconnectionTime = DateTime.Now;
                                        }

                                        connectionLost = true;
                                    }
                                }));

                                _ = Task.Run(() => HeartbeatLoopAsync(
                                    () =>
                                    {
                                        lock (stateLock)
                                        {
                                            return (client, stream, connectionLost, lastPongTime);
                                        }
                                    },
                                    () =>
                                    {
                                        lock (stateLock)
                                        {
                                            if (!connectionLost)
                                            {
                                                disconnectionTime = DateTime.Now;
                                            }

                                            connectionLost = true;
                                        }
                                    },
                                    listenCts.Token));
                            }
                        },
                        listenCts.Token));
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
            if (!connectionLost)
            {
                disconnectionTime = DateTime.Now;
            }

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
            Console.Write("Tú: ");
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
                Console.Write("Tú: ");
                onConnectionLost();
                break;
            }

            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

            if (message == "PONG")
            {
                HeartbeatState.LastPong = DateTime.Now;
                continue;
            }

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
        Console.Write("Tú: ");
        onConnectionLost();
    }
}

static async Task HeartbeatLoopAsync(
    Func<(TcpClient? Client, NetworkStream? Stream, bool Lost, DateTime LastPong)> getState,
    Action markConnectionLost,
    CancellationToken token)
{
    while (!token.IsCancellationRequested)
    {
        try
        {
            var (client, stream, lost, _) = getState();

            if (!lost && client != null && stream != null && client.Connected)
            {
                byte[] ping = Encoding.UTF8.GetBytes("PING");
                await stream.WriteAsync(ping, 0, ping.Length);

                await Task.Delay(3000, token);

                TimeSpan elapsed = DateTime.Now - HeartbeatState.LastPong;
                if (elapsed.TotalSeconds > 6)
                {
                    Console.WriteLine("\nHeartbeat perdido. Marcando conexión como caída...");
                    Console.Write("Tú: ");
                    markConnectionLost();
                    break;
                }
            }
            else
            {
                break;
            }
        }
        catch (OperationCanceledException)
        {
            break;
        }
        catch
        {
            Console.WriteLine("\nError en heartbeat. Marcando conexión como caída...");
            Console.Write("Tú: ");
            markConnectionLost();
            break;
        }
    }
}

static async Task FailbackLoopAsync(
    List<(string Host, int Port, string Name)> servers,
    Func<(TcpClient? Client, NetworkStream? Stream, bool Lost, string? CurrentServer)> getState,
    Action<TcpClient, NetworkStream> switchToMain,
    CancellationToken token)
{
    while (!token.IsCancellationRequested)
    {
        try
        {
            var (_, _, lost, currentServer) = getState();

            if (!lost && currentServer == "BACKUP")
            {
                var main = servers.First(s => s.Name == "MAIN");

                try
                {
                    var testClient = new TcpClient();
                    await testClient.ConnectAsync(main.Host, main.Port);

                    var testStream = testClient.GetStream();
                    switchToMain(testClient, testStream);
                    break;
                }
                catch
                {
                    // MAIN sigue caído
                }
            }

            await Task.Delay(5000, token);
        }
        catch (OperationCanceledException)
        {
            break;
        }
        catch
        {
            await Task.Delay(5000, token);
        }
    }
}

static class HeartbeatState
{
    public static DateTime LastPong = DateTime.MinValue;
}