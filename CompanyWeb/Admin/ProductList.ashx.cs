using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CompanyWeb.Admin
{
    /// <summary>
    /// ProductList 的摘要说明
    /// </summary>
    public class ProductList : IHttpHandler
    {
        private int pageNum = 3;
        private int startNum = 0;//表示从第1行开始
        private int endNum = 6;//在mysql中，第二个数字代表取几个数字
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string num = context.Request["pageNum"];
            if (!string.IsNullOrEmpty(num))//判断是否有传值
            {
                if (int.TryParse(num, out pageNum) == true)//判断传值是否是数字
                {
                    pageNum = Convert.ToInt32(num);//页面设为传过来的值
                    startNum = setStartNum(pageNum);
                }
            }
            //            DataTable products = MySqlHelper.ExecuteDataTable(@"select p.Id as Id,p.Name as Name,c.Name as CategoryName from T_Product p 
            //            left join T_ProductCategories c on p.CategoriyId=c.Id");
            DataTable products = MySqlHelper.ExecuteDataTable(@"select p.Id as Id,p.Name as Name,
            c.Name as CategoryName from T_Product p left join T_ProductCategories c on 
            p.CategoriyId=c.Id order by Id limit @StartNum,@EndNum",
            new MySqlParameter("@StartNum", startNum), new MySqlParameter("@EndNum", endNum));

            //不能一次把表中的数据都取出来，否则性能很差
            //也不能一次性取出来，用c#过滤

            //总条数
            long totalCount = (long)MySqlHelper.ExecuteScalar("select count(*) from T_Product");
            //总页数
            int pageCount = (int)Math.Ceiling(totalCount / endNum * 1.0);
            object[] pageData = new object[pageCount];
            for (int i = 0; i < pageCount; i++)
            {
                pageData[i] = new { Href = "ProductList.ashx?pageNum=" + (i + 1), 
                    Title = "第" + (i + 1) + "页" };
            }
            var data = new { Title = "产品列表", Products = products.Rows, PageData = pageData };
            string html = CommonHelper.RenderHtml("Admin/ProductList.html", data);
            context.Response.Write(html);

        }

        public int setStartNum(int pageNum)
        {
            int temp = (pageNum - 1) * endNum;
            return temp;
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