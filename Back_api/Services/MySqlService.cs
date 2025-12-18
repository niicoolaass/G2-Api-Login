using MySqlConnector;
using Microsoft.Extensions.Configuration;

namespace Back_api.Services
{
    public class MySqlService
    {
        private readonly string _connectionString;

        public MySqlService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MySqlConnection")
                ?? throw new InvalidOperationException("Conexão MySQL não encontrada.");
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
