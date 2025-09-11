using Neo4j.Driver;

namespace perla_metro_route_service.src.service
{
    public class Neo4jService : IDisposable
    {
        private readonly IDriver _driver;

        public Neo4jService(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public IAsyncSession GetSession() => _driver.AsyncSession();

        public void Dispose() => _driver?.Dispose();
    }
}
