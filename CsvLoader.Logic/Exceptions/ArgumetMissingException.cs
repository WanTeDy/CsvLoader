using CsvLoader.Logic.Common;

namespace CsvLoader.Logic.Exceptions
{
    public class ArgumetMissingException : BaseException
    {
        public ArgumetMissingException(string argumentException)
            : base(Errors.ErrorCode.ArgumentMisssing, argumentException)
        { }
    }
}