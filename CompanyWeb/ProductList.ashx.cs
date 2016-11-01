using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CompanyWeb
{
    /// <summary>
    /// ProductList 的摘要说明
    /// </summary>
    public class ProductList : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            int pageNum = 1;
            if (context.Request["PageNum"] != null)
            {
                pageNum = Convert.ToInt32(context.Request["PageNum"]);
            }

            DataTable products;

            //如果指定了CategoryId参数，则只显示特定类型的产品
            //如果没有指定，则显示所有产品
            bool hasCategoryId = !string.IsNullOrEmpty(context.Request["CategoryId"]);
            if (hasCategoryId)
            {
                long categoryId = Convert.ToInt64(context.Request["CategoryId"]);
                products = MySqlHelper.ExecuteDataTable(@"select * from T_Product order by Id limit 
                @Start, @End where CategoriyId=@CategoriyId", new MySqlParameter("@CategoryId", categoryId),
                    new MySqlParameter("@Start", (pageNum - 1) * 9 + 1)
                , new MySqlParameter("@End", pageNum * 9));
            }
            else
            {
                products = MySqlHelper.ExecuteDataTable(@"select * from T_Product order by Id limit @Start, @End",
                    new MySqlParameter("@Start", (pageNum - 1) * 9 + 1)
                , new MySqlParameter("@End", pageNum * 9));
            }

            long totalCount;
            if (hasCategoryId)
            {
                int categoryId = Convert.ToInt32(context.Request["CategoryId"]);
                totalCount = (long)MySqlHelper.ExecuteScalar("select count(*) from T_Product where CategoryId=@CategoryId", new MySqlParameter("@CategoryId", categoryId));
            }
            else
            {
                totalCount = (long)MySqlHelper.ExecuteScalar("select count(*) from T_Product");
            }
            int pageCount = (int)Math.Ceiling(totalCount / 9.0);
            object[] pageData = new object[pageCount];
            for (int i = 0; i < pageCount; i++)
            {
                pageData[i] = new { Href = "ProductList.ashx?PageNum=" + (i + 1), Title = i + 1 };
            }
            var data = new { Products = products.Rows, PageData = pageData, TotalCount = totalCount, PageNum = pageNum, PageCount = pageCount };
            string html = CommonHelper.RenderHtml("Front/ProductList.html", data);
            context.Response.Write(html);
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