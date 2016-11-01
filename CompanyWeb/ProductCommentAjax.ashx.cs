using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace CompanyWeb
{
    /// <summary>
    /// ProductCommentAjax 的摘要说明
    /// </summary>
    public class ProductCommentAjax : IHttpHandler
    {
       
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["Action"];
            string status;
            if ("PostComment".Equals(action))
            {
                int productId = Convert.ToInt32(context.Request["ProductId"]);
                string title = context.Request["Title"];
                string msg = context.Request["Msg"];
                if (productId == 0 || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(msg))
                {
                    status = "数据不能为空";
                }
                else
                {
                    int line = MySqlHelper.ExecuteNonQuery(@"insert into T_ProductComments (ProductId
                    ,Title,Msg,CreateDateTime) values (@ProductId,@Title,@Msg,@CreateDateTime)"
                    , new MySqlParameter("@ProductId", productId)
                    , new MySqlParameter("@Title", title)
                     , new MySqlParameter("@Msg", msg)
                       , new MySqlParameter("@CreateDateTime", DateTime.Now.ToLocalTime().ToString()));
                    if (line == 1)
                    {
                        status = "OK";
                    }
                    else
                    {
                        status = "插入数据失败！";
                    }
                }
                context.Response.Write(status);
            }
            else if ("Load".Equals(action))
            {
                int productId = Convert.ToInt32(context.Request["ProductId"]);
                DataTable dt = MySqlHelper.ExecuteDataTable(@"select * from T_ProductComments
                where ProductId=@ProductId", new MySqlParameter("@ProductId", productId));
                //一般不要把DataTable等对象直接通过Json传递给客户端，一般应该只传递基本类型或者
                //POCO（Plain Object C# Object 简单的类，只有属性的类）或是POCO的简单结合

                object[] comments = new object[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    DateTime createDT = (DateTime)row["CreateDateTime"];
                    comments[i] = new
                    {
                        Title = row["Title"],
                        Msg = row["Msg"],
                        CreateDateTime = createDT.ToString()
                    };
                }
                string json = new JavaScriptSerializer().Serialize(comments);
                context.Response.Write(json);

            }
            else
            {

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