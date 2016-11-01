using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CompanyWeb
{
    /// <summary>
    /// ProductView 的摘要说明
    /// </summary>
    public class ProductView : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            //todo：检验是否传递了Id，已经Id的合法性
            long id = Convert.ToInt64(context.Request["Id"]);
            DataTable dt = MySqlHelper.ExecuteDataTable("select * from T_Product where Id=@Id", new MySqlParameter("@Id", id));
            if (dt.Rows.Count <= 0)
            {
                context.Response.Write("找不到");
            }
            else if (dt.Rows.Count > 1)
            {
                context.Response.Write("找到多条数据");
            }
            else
            {
                DataRow product = dt.Rows[0];
                var data = new { Product = product};
                string html = CommonHelper.RenderHtml("Front/ProductView.html", data);
                context.Response.Write(html);
            }       
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