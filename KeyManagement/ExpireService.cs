using Serilog;

namespace KeyManagement;

public class ExpireService(LiteContext context) : IHostedService
{
    private Task? _task;
    private CancellationTokenSource _cts = null!;
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _task = Worker();
        
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _cts.CancelAsync();

            if (_task != null)
            {
                await Task.WhenAny(_task, Task.Delay(Timeout.Infinite, cancellationToken));
            }

            context.Db.Commit();
        }
        finally
        {
            _cts?.Dispose();
            _task?.Dispose();
        }
    }

    private async Task Worker()
    {
        await Task.Delay(1000, _cts.Token);
        while (!_cts.Token.IsCancellationRequested)
        {
            try
            {
                var keys = context.Keys.FindAll().ToList();
                var count = 0;
                foreach (var userKey in keys.Where(userKey => userKey.Expires <= DateTime.Now))
                {
                    context.Keys.Delete(userKey.Id);
                    count++;
                }

                if (count > 0)
                {
                    context.Db.Commit();
                }
            }
            catch (Exception e)
            {
                Log.ForContext<ExpireService>().Error(e, e.Message);
            }
            finally
            {
                await Task.Delay(1000, _cts.Token);
            }
        }
    }
}