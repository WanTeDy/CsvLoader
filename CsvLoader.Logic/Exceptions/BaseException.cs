using System;
using CsvLoader.Logic.Common;

namespace CsvLoader.Logic.Exceptions
{
    /// <summary>
    ///     Base exception
    /// </summary>
    public class BaseException : Exception
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="errorCode">Enum error code</param>
        /// <param name="argument">Exception argument</param>
        public BaseException(Errors.ErrorCode errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
            ExMessage = message;
        }

        public BaseException(Errors.ErrorCode errorCode)
        {
            ErrorCode = errorCode;
            ExMessage = Common.Errors.Inst[errorCode];
        }

        /// <summary>
        ///     Error code
        /// </summary>
        public Errors.ErrorCode ErrorCode { get; set; }
                
        /// <summary>
        ///     Argument
        /// </summary>
        public string ExMessage { get; set; }        
    }
}