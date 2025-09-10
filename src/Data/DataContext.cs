using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace perla_metro_route_service.src.Data
{
    public class DataContext : IDisposable
    {
        private readonly IDriver _driver;
        public DataContext(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }
        
        public void Dispose()
        {
            _driver?.Dispose();
        }
        

        
    }
    
}