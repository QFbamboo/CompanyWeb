using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CompanyWeb.Admin
{
    /// <summary>
    /// Settings 的摘要说明
    /// </summary>
    public class Settings : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            bool isSave = !string.IsNullOrEmpty(context.Request["Save"]);
            if (isSave)
            {

            }
            context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}