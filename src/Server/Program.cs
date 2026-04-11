using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;

string role = args.Length > 0 ? args[0].ToUpper() : "MAIN";
int port = role == "BACKUP" ? 5001 : 5000;

var listener = new TcpListener(IPAddress.Any, port);
listener.Start();

Console.WriteLine($"[{role}] Servidor iniciado en puerto {port}");

var clients = new ConcurrentDictionary<int, TcpClient>();
int clientIdCounter = 0;

while (true)
{
    TcpClient client = await listener.AcceptTcpClientAsync();
    int clientId = Interlocked.Increment(ref clientIdCounter);

    clients[clientId] = client;
    Console.WriteLine($"[{role}] Cliente {clientId} conectado.");

    _ = Task.Run(() => HandleClientAsync(clientId, client, clients, role));
}

static async Task HandleClientAsync(
    int clientId,
    TcpClient client,
    ConcurrentDictionary<int, TcpClient> clients,
    string role)
{
    try
    {
        using NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        while (true)
        {
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

            if (bytesRead == 0)
                break;

            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            string finalMessage = $"[{role}] Cliente {clientId}: {message}";

            Console.WriteLine(finalMessage.Trim());

            await BroadcastAsync(finalMessage, clients, clientId);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[{role}] Error con cliente {clientId}: {ex.Message}");
    }
    finally
    {
        if (clients.TryRemove(clientId, out _))
        {
            Console.WriteLine($"[{role}] Cliente {clientId} desconectado.");
        }

        client.Close();
    }
}

static async Task BroadcastAsync(
    string message,
    ConcurrentDictionary<int, TcpClient> clients,
    int senderId)
{
    byte[] data = Encoding.UTF8.GetBytes(message);

    foreach (var kvp in clients)
    {
        int clientId = kvp.Key;
        TcpClient client = kvp.Value;

        if (clientId == senderId)
            continue;

        try
        {
            if (client.Connected)
            {
                NetworkStream stream = client.GetStream();
                await stream.WriteAsync(data, 0, data.Length);
            }
        }
        catch
        {
        }
    }
}