using Npgsql;
using System.Data;

namespace Sinerlog.Lambda.Pdf.Common.Persistency
{
    public static class DbConnector
    {
        public static IDbConnection CreateConnection()
        {
            string connectionString = Environment.GetEnvironmentVariable("SHM_DB");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("Missing ShmDB Enviroment Variable!");
            }

            return new NpgsqlConnection(connectionString);
        }
    }
}
