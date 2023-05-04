using Dapper;
using Npgsql;
using System.Data;

namespace Sinerlog.Lambda.Pdf.Common.Persistency
{
    public static class QueryRepository<T> 
    {
        public static async Task<IReadOnlyList<T>> GetListAsync(string sql)
        {
            try
            {
                using (var connection = DbConnector.CreateConnection())
                {
                    return (await connection.QueryAsync<T>(sql)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public static async Task<T> GetAsync(string sql)
        {
            try
            {
                using (var connection = DbConnector.CreateConnection())
                {
                    return (await connection.QueryFirstAsync<T>(sql));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        
    }
}
