using CsvLoader.Logic.Common;

namespace CsvLoader.Logic.Exceptions
{
    public class InvalidArgumentException : BaseException
    {
        public InvalidArgumentException(string argumentException)
            : base(Errors.ErrorCode.InvalidArgument, argumentException)
        { }
    }
}