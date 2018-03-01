using CsvLoader.Logic.Common;
using CsvLoader.Logic.Exceptions;
using CsvLoader.Logic.Helper;
using CsvLoader.Logic.Objects.Response;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace CsvLoader.Web.SignalRHubs
{
    public class LoaderHub : Hub
    {
        public static void SendProgress(dynamic caller, double progressPercentage)
        {
            caller.sendProgress(progressPercentage);
        }

        public static void SendMessage(dynamic caller, string message, bool isError = false)
        {
            caller.sendMessage(message, isError);
        }
    }
}