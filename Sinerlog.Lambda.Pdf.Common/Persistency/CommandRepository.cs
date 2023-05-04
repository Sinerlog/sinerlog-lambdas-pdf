using Dapper;

namespace Sinerlog.Lambda.Pdf.Common.Persistency
{
    public static class CommandRepository
    {
        public static async Task<bool> ExecuteAsync(string sql, object parameters = null)
        {
            try
            {
                using (var connection = DbConnector.CreateConnection())
                {
                    if (parameters is not null)
                        return (await connection.ExecuteAsync(sql, parameters)) > 0;
                    else
                        return (await connection.ExecuteAsync(sql)) > 0;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
