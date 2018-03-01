using CsvLoader.Logic.Common;
using CsvLoader.Logic.Exceptions;
using CsvLoader.Logic.Model;
using CsvLoader.Logic.Objects.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CsvLoader.Logic.Helper
{
    public class ImportCSVToDbHelper
    {
        public CSVImportModel Model { get; private set; }
        private HashSet<String> _skuSet { get; set; }
        private String _data { get; set; }
        private Dictionary<int, string> _columns { get; set; }
        public ImportCSVToDbHelper(string data, Dictionary<int, string> columns)
        {
            if (data == null || columns == null || columns.Count == 0)
                throw new ArgumetMissingException("Получены некорректные данные");

            _data = data;
            _columns = columns;
            _skuSet = new HashSet<string>();
        }

        public RespCommonObject PrepareAndStartImport()
        {
            using (var reader = new StreamReader(new MemoryStream(Convert.FromBase64String(_data))))
            {
                String row = reader.ReadLine();
                var columns = row.Split(GlobalConsts.Splitter);
                if (columns.Length == 0 || columns.Length != _columns.Count)
                    throw new ArgumetMissingException("Получены некорректные данные");

                CheckAndFixColumnNames();
                var skuIndex = _columns.FirstOrDefault(x => x.Value == CSVColumns.SKU).Key;
                var dataList = new List<string[]>();
                var rowIndex = 1;
                while (reader.Peek() >= 0)
                {
                    row = reader.ReadLine();
                    columns = row.Split(GlobalConsts.Splitter);
                    if (!_skuSet.Add(columns[skuIndex]))
                        throw new InvalidArgumentException("Обнаружено совпадение для поля SKU: " + columns[skuIndex] + " | Ряд № " + rowIndex);
                    dataList.Add(columns);
                    rowIndex++;
                }
                Model = new CSVImportModel(_columns, dataList);
                Model.StartImportData();
            }
            return new RespCommonObject
            {
                ErrCode = Errors.ErrorCode.Success,
                DebugMessage = "Данные успешно импортированы",
            };
        }

        private void CheckAndFixColumnNames()
        {
            var fixFeature = _columns.Count(x => x.Value == CSVColumns.Feature) > 1;
            var fixProducts = _columns.Count(x => x.Value == CSVColumns.Product) > 1;
            var featureIndex = 1;
            var productsIndex = 1;
            for (int i = _columns.Count - 1; i >= 0; i--)
            {
                var name = _columns[i];
                if (name == CSVColumns.NoMapped || name == CSVColumns.Ignore)
                    _columns.Remove(i);
                else if (fixFeature && name == CSVColumns.Feature)
                    _columns[i] = CSVColumns.Feature + featureIndex++;
                else if (fixProducts && name == CSVColumns.Product)
                    _columns[i] = CSVColumns.Product + productsIndex++;
                else if (name != CSVColumns.Brand && name != CSVColumns.Feature && name != CSVColumns.Price && name != CSVColumns.Product && name != CSVColumns.SKU && name != CSVColumns.Weight)
                    _columns.Remove(i);
            }
            if (_columns.Count(x => x.Value == CSVColumns.SKU) != 1)
                throw new ArgumetMissingException("Обязательное поле SKU не найдено или полей больше чем одно");
            if (_columns.Count(x => x.Value == CSVColumns.Brand) != 1)
                throw new ArgumetMissingException("Обязательное поле Brand не найдено или полей больше чем одно");
            if (_columns.Count(x => x.Value == CSVColumns.Price) != 1)
                throw new ArgumetMissingException("Обязательное поле Price не найдено или полей больше чем одно");
            if (_columns.Count(x => x.Value == CSVColumns.Weight) > 1)
                throw new ArgumetMissingException("Поле Weight должно быть только одно");
        }
    }
}