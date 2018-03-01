using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvLoader.Logic.Common
{
    public class Errors : Dictionary<Errors.ErrorCode, string>
    {
        public enum ErrorCode
        {
            Success = 1,
            InvalidArgument,
            ArgumentMisssing,
            InternalError,
        }

        private static Errors _inst;

        private Errors()
        {
            Add(ErrorCode.Success, "Файл импортирован успешно");
            Add(ErrorCode.InvalidArgument, "Некорректные входные данные");
            Add(ErrorCode.ArgumentMisssing, "Пустой аргумент");
            Add(ErrorCode.InternalError, "Ошибка сервера");
        }

        public static Errors Inst
        {
            get { return _inst ?? (_inst = new Errors()); }
        }
    }
}
