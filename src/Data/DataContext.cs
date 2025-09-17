using Neo4j.Driver;

namespace perla_metro_route_service.src.data
{
    /// <summary>
    /// Data context for managing Neo4j database connections and queries.
    /// </summary>
    public class DataContext : IDisposable
    {
        /// <summary>
        /// The Neo4j driver instance.
        /// </summary>
        private readonly IDriver _driver;
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContext"/> class.
        /// </summary>
        /// <param name="uri">The URI of the Neo4j database.</param>
        /// <param name="user">The username for database authentication.</param>
        /// <param name="password">The password for database authentication.</param>
        public DataContext(string uri, string user, string password)
        {
            // Initialize the Neo4j driver with the provided credentials
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }
        /// <summary>
        /// Gets an asynchronous session for database operations.
        /// </summary>
        /// <returns>An asynchronous session.</returns>
        public IAsyncSession GetSession() => _driver.AsyncSession();
        /// <summary>
        /// Disposes the Neo4j driver and releases resources.
        /// </summary>
        public void Dispose() => _driver?.Dispose();
    }
}
