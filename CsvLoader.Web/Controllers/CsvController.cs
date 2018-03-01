using CsvLoader.Logic.Common;
using CsvLoader.Logic.Exceptions;
using CsvLoader.Logic.Helper;
using CsvLoader.Logic.Objects.Response;
using CsvLoader.Web.SignalRHubs;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace CsvLoader.Web.Controllers
{
    public class CsvController : Controller
    {
        private Timer _timer { get; set; }
        public ActionResult Load()
        {
            return View();
        }
        
        [HttpPost]
        public JsonResult Load(string connectionId, string base64File, string[] fields)
        {
            RespCommonObject result;
            var context = GlobalHost.ConnectionManager.GetHubContext<LoaderHub>();
            try
            {
                var index = 0;
                var importHelper = new ImportCSVToDbHelper(base64File, fields.ToDictionary(x => index++));
                _timer = new Timer((x) =>
                {
                    if (importHelper.Model != null)
                        LoaderHub.SendProgress(context.Clients.Client(connectionId), importHelper.Model.ProgressPercentage);
                }, null, 1000, 200);
                result = importHelper.PrepareAndStartImport();
            }
            catch (BaseException ex)
            {
                result = new RespCommonObject
                {
                    DebugMessage = (ex.ExMessage != null)
                        ? ex.ExMessage
                        : ex.Message,
                    ErrCode = ex.ErrorCode,
                };
            }
            catch (Exception ex)
            {
                result = new RespCommonObject
                {
                    DebugMessage = (ex.InnerException != null)
                        ? ex.InnerException.Message
                        : ex.Message,
                    ErrCode = Errors.ErrorCode.InternalError
                };
            }
            if (_timer != null)
                _timer.Change(Timeout.Infinite, Timeout.Infinite);

            LoaderHub.SendMessage(context.Clients.Client(connectionId), result.DebugMessage, result.ErrCode != Errors.ErrorCode.Success);
            return Json(result);
        }
    }
}