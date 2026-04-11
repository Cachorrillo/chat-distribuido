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

    Task tareaLectura = Task.Run(() =>
    {
        try
        {
            while (true)
            {
                string? mensaje = reader.ReadLine();

                if (mensaje == null)
                    break;

                Console.WriteLine($"\n[CHAT] {mensaje}");
                Console.Write("Mensaje: ");
            }
        }
        catch
        {
            Console.WriteLine("\n[CLIENTE] Conexión cerrada.");
        }
    });

    while (true)
    {
        Console.Write("Mensaje: ");
        string? mensaje = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(mensaje))
            continue;

        writer.WriteLine(mensaje);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[ERROR] {ex.Message}");
}