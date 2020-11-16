using BMEDmgt.Areas.MedEngMgt.Controllers;
using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace BMEDmgt.Filters
{
    public class MyErrorHandlerAttribute : FilterAttribute, IExceptionFilter
    {
        private BMEDcontext db = new BMEDcontext();
        public void OnException(ExceptionContext filterContext)
        {
            // Save error msg. 
            string logClass = "系統錯誤訊息";
            string logAction = filterContext.Exception.Message;
            var result = new SystemLogsController().SaveLog(logClass, logAction);

            filterContext.ExceptionHandled = true;
            filterContext.Result = new JsonResult
            {
                Data = new { success = false, error = filterContext.Exception.Message },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}