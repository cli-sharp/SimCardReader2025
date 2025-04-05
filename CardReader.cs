namespace SimCardReader2025;

using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class CardReader(ILogger<CardReader> logger) : BackgroundService
{
    private readonly UdpClient udpClient = new(10189);

    public override void Dispose()
    {
        udpClient.Dispose();
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        while (stoppingToken.IsCancellationRequested is false)
        {
            try
            {
                var received = await udpClient.ReceiveAsync(stoppingToken);
                logger.LogDebug(
                    "Received {data} from {end_point}",
                    BitConverter.ToString(received.Buffer),
                    received.RemoteEndPoint);
                var response = await Process(received.Buffer);
                if (response.Length > 0)
                {
                    logger.LogDebug(
                        "Sending {data} to {end_point}",
                        BitConverter.ToString(response),
                        received.RemoteEndPoint);
                    await udpClient.SendAsync(response, received.RemoteEndPoint, stoppingToken);
                }
            }
            catch
            {
            }
        }
    }

    private async Task<byte[]> Process(byte[] buffer)
    {
        throw new NotImplementedException();
    }
}