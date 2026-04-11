using System.Net.Sockets;
using System.Text;

string servidor = "127.0.0.1";
int puerto = 5000;

try
{
    TcpClient cliente = new TcpClient();
    cliente.Connect(servidor, puerto);

    Console.WriteLine("[CLIENTE] Conectado al servidor.");

    using NetworkStream stream = cliente.GetStream();
    using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
    using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

    // mensaje inicial del servidor
    string? bienvenida = reader.ReadLine();
    Console.WriteLine($"[SERVIDOR] {bienvenida}");

    while (true)
    {
        Console.Write("Mensaje: ");
        string? mensaje = Console.ReadLine();

        if (string.IsNullOrEmpty(mensaje))
            continue;

        writer.WriteLine(mensaje);

        string? respuesta = reader.ReadLine();
        Console.WriteLine($"[SERVIDOR] {respuesta}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[ERROR] {ex.Message}");
}