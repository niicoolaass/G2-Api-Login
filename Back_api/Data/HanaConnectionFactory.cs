using System;
using System.Data.Odbc;
using Microsoft.Extensions.Configuration;

namespace Back_api.Data
{
    public class HanaConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public HanaConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public OdbcConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString("HanaConnection");

            return new OdbcConnection(connectionString);
        }
    }
}
