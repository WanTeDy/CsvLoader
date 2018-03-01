using CsvLoader.Logic.Common;
using CsvLoader.Logic.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvLoader.Logic.DbLayer
{
    public class DbWorker : IDisposable
    {
        private SqlConnection _connection { get; set; }
        public DbWorker()
        {
            _connection = new SqlConnection(GlobalConsts.DbConnection);
            _connection.Open();
        }

        public int ExecuteCommand(SqlCommand command)
        {
            if (command == null)
                throw new ArgumetMissingException("Command null");
            command.Connection = _connection;
            return command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
