using TweetBook.Data;
using TweetBook.HealthChecks;

namespace TweetBook.Installers
{
    public class HealthChecksInstaller : IInstaller
    {
        public void InstallServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddHealthChecks()
                    .AddDbContextCheck<DataContext>()
                    .AddCheck<RedisHealthCheck>("Redis");
        }
    }
}
