using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMEDmgt.Filters
{
    public class MyErrorHandlerAttribute : FilterAttribute, IExceptionFilter
    {
        private BMEDcontext db = new BMEDcontext();
        public void OnException(ExceptionContext filterContext)
        {
            // Save error msg. 
            SystemLog log = new SystemLog();
            log.LogClass = "系統錯誤訊息";
            log.LogTime = DateTime.UtcNow.AddHours(8);
            log.Action = filterContext.Exception.Message;
            db.SystemLogs.Add(log);
            db.SaveChanges();

            filterContext.ExceptionHandled = true;
            filterContext.Result = new JsonResult
            {
                Data = new { success = false, error = filterContext.Exception.Message },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}