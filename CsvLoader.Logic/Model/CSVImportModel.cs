using CsvLoader.Logic.Common;
using CsvLoader.Logic.DbLayer;
using CsvLoader.Logic.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvLoader.Logic.Model
{
    public class CSVImportModel
    {
        const string pool = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public double ProgressPercentage { get; private set; }
        private Dictionary<int, string> _columns { get; set; }
        private List<string[]> _dataList { get; set; }
        private DbWorker _db { get; set; }

        public CSVImportModel(Dictionary<int, string> columns, List<string[]> dataList)
        {
            if (columns == null || columns.Count == 0 || dataList == null || dataList.Count == 0)
                throw new ArgumetMissingException("Получены некорректные данные");

            _columns = columns;
            _dataList = dataList;
            _db = new DbWorker();
        }

        public void StartImportData()
        {
            var isError = false;
            var tries = 5;
            do
            {
                isError = false;
                var dbName = GetRandomDBName();
                try
                {
                    CreateNewTable(dbName);
                    ImportData(dbName);
                }
                catch
                {
                    isError = true;
                    tries--;
                    try
                    {
                        var command = new SqlCommand();
                        command.CommandText = "DROP TABLE " + dbName;
                        _db.ExecuteCommand(command);
                    }
                    catch { }
                }
            } while (isError && tries > 0);
            if (isError)
                throw new InvalidArgumentException("Произошла ошибка при импортировании данных");
        }

        private string GetRandomDBName()
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            var length = rnd.Next(5, 15);
            var chars = Enumerable.Range(0, length)
                .Select(x => pool[rnd.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }

        private void CreateNewTable(string dbName)
        {
            var command = new SqlCommand();
            var builder = new StringBuilder("CREATE TABLE ");
            builder.Append(dbName);
            builder.Append(" (");
            foreach (var column in _columns)
            {
                var columnName = column.Value.Replace(" ", String.Empty);
                if (column.Value == CSVColumns.Price)
                    builder.Append(columnName + " decimal(18,2)");
                else
                {
                    builder.Append(columnName + " varchar(255)");
                    if (column.Value == CSVColumns.SKU)
                        builder.Append(" NOT NULL PRIMARY KEY");
                }
                builder.Append(",");
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append(")");
            command.CommandText = builder.ToString();
            _db.ExecuteCommand(command);
        }

        private void ImportData(string dbName)
        {
            var builder = new StringBuilder("INSERT INTO ");
            builder.Append(dbName);
            builder.Append(" (");
            foreach (var column in _columns)
            {
                var columnName = column.Value.Replace(" ", String.Empty);
                builder.Append(columnName);
                builder.Append(",");
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append(") VALUES (");
            var startingQuery = builder.ToString();
            for (int i = 0; i < _dataList.Count; i++)
            {
                ProgressPercentage = Math.Round(((i + 1) / (double)_dataList.Count) * 100, 2, MidpointRounding.AwayFromZero);
                var command = new SqlCommand();
                builder = new StringBuilder(startingQuery);
                for (int j = 0; j < _dataList[i].Length; j++)
                {
                    string columnName = "";
                    if (!_columns.TryGetValue(j, out columnName))
                        continue;

                    var param = "@val" + j;
                    builder.Append(param);
                    builder.Append(",");
                    if (columnName == CSVColumns.Price)
                    {
                        decimal value = 0;
                        if (!Decimal.TryParse(_dataList[i][j].Replace('.', ','), out value))
                            throw new ArgumetMissingException("Получены некорректные данные поля Price");

                        command.Parameters.Add(new SqlParameter(param, value));
                    }
                    else
                        command.Parameters.Add(new SqlParameter(param, _dataList[i][j]));
                }
                builder.Remove(builder.Length - 1, 1);
                builder.Append(")");
                command.CommandText = builder.ToString();
                _db.ExecuteCommand(command);
            }
        }
    }
}
