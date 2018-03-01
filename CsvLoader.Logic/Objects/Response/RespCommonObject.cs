using CsvLoader.Logic.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvLoader.Logic.Objects.Response
{
    public class RespCommonObject
    {

        public RespCommonObject()
        {
            ErrCode = Errors.ErrorCode.Success;
        }
        /// <summary>
        ///     Error code
        /// </summary>
        [JsonIgnore]
        public Errors.ErrorCode ErrCode { get; set; }


        /// <summary>
        ///     Error code
        /// </summary>
        public int ErrorCode
        {
            get { return (int)ErrCode; }
        }

        /// <summary>
        ///     Debug message
        /// </summary>
        public string DebugMessage { get; set; }
    }
}
