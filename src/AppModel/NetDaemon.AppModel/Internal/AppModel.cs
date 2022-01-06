using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
namespace NetDaemon.AppModel.Internal;

internal class AppModelImpl : IAppModel
{
    private readonly IServiceProvider _provider;

    public AppModelImpl(
        IServiceProvider provider
    )
    {
        _provider = provider;
    }

    internal IAppModelContext? CurrentContext { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (CurrentContext is not null)
            await CurrentContext.DisposeAsync().ConfigureAwait(false);
    }

    public Task<IAppModelContext> InitializeAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(
            _provider.GetRequiredService<IAppModelContext>()
            );
    }
}