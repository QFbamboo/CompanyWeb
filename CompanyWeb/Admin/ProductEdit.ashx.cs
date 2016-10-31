using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace CompanyWeb.Admin
{
    /// <summary>
    /// ProductEdit 的摘要说明
    /// </summary>
    public class ProductEdit : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            bool isPostBack = !string.IsNullOrEmpty((context.Request["IsPostBack"]));
            string action = context.Request["Action"];

            if (isPostBack)
            {
                //新增数据操作
                if ("AddNew".Equals(action))
                {
                    //数据的合法性检查（服务端，客户端都要做）数据格式合法性，是否为空，

                    string name = context.Request["Name"];
                    int categoryId = Convert.ToInt32(context.Request["CategoryId"]);
                    //获得浏览器上传的文件信息
                    HttpPostedFile productImg = context.Request.Files["ProductImage"];
                    //图片要保存到项目的文件夹（或子文件夹），才可以通过web来访问图片
                    //productImg.SaveAs("c://");
                    //MapPath可以把一个相对于网站根目录的文件或者文件夹路径转换为在服务器上的物理全路径
                    // DateTime.Now.ToString("yyyyMMddHHmmssfffffff");//得到精确到毫秒的当前时间格式
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfffffff")
                        + Path.GetExtension(productImg.FileName);
                    productImg.SaveAs(context.Server.MapPath("~/uploadfile/" + fileName));

                    // context.Server.MapPath("~/uploadfile/" + fileName);

                    //用guid来做文件名一定不会重复 Guid.NewGuid().ToString();
                    //用文件内容的md5值也几乎不会重复

                    string msg = context.Request["Msg"];
                    MySqlHelper.ExecuteDataTable("insert into T_Product(Name,CategoriyId,ImagePath,Msg) values(@Name,@CategoriyId,@ImagePath,@Msg)",
                        new MySqlParameter("@Name", name), new MySqlParameter("@CategoriyId", categoryId),
                        new MySqlParameter("@ImagePath", "/uploadfile/" + fileName),
                        new MySqlParameter("@Msg", msg));//有bug的，同一毫秒内多个人上传文件
                    context.Response.Redirect("ProductList.ashx");
                }
                //编辑数据操作
                else if ("Edit".Equals(action))
                {
                    int id = Convert.ToInt32(context.Request["Id"]);
                    string name = context.Request["Name"];
                    int categoryId = Convert.ToInt32(context.Request["CategoryId"]);
                    HttpPostedFile productImg = context.Request.Files["ProductImage"];
                    string msg = context.Request["Msg"];
                    if (CommonHelper.HasFile(productImg))
                    {
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfffffff")
                     + Path.GetExtension(productImg.FileName);
                        productImg.SaveAs(context.Server.MapPath("~/uploadfile/" + fileName));
                        MySqlHelper.ExecuteNonQuery("update T_Product set " +
                        "Name=@Name,CategoriyId=@CategoriyId,Msg=@Msg,ImagePath=@ImagePath where Id=@Id",
                        new MySqlParameter("@Name", name), new MySqlParameter("@CategoriyId", categoryId),
                        new MySqlParameter("@Msg", msg), new MySqlParameter("@ImagePath", "/uploadfile/" + fileName)
                        , new MySqlParameter("@Id", id));
                        context.Response.Redirect("ProductList.ashx");
                    }
                    else
                    {//用户没有选择新图片，则还用之前的产品图片

                        MySqlHelper.ExecuteNonQuery("update T_Product set " +
                       "Name=@Name,CategoriyId=@CategoriyId,Msg=@Msg where Id=@Id",
                       new MySqlParameter("@Name", name), new MySqlParameter("@CategoriyId", categoryId),
                       new MySqlParameter("@Msg", msg), new MySqlParameter("@Id", id));
                        context.Response.Redirect("ProductList.ashx");
                    }

                }
                else
                {
                    context.Response.Write("Action错误：" + action);
                }
            }
            else
            {
                DataTable categroies = MySqlHelper.ExecuteDataTable("select * from T_ProductCategories ");
                //新增展示
                if ("AddNew".Equals(action))
                {
                    var data = new
                    {
                        Title = "新增产品",
                        Action = action,
                        Product = new
                        {
                            Id = 0,
                            Name = "",
                            CategoryId = 0,
                            Msg = ""
                        },
                        Categories = categroies.Rows
                    };
                    string html = CommonHelper.RenderHtml("Admin/ProductEdit.html", data);
                    context.Response.Write(html);

                }
                else if ("Edit".Equals(action)) //编辑展示
                {
                    int id = Convert.ToInt32(context.Request["Id"]);
                    DataTable products = MySqlHelper.ExecuteDataTable("select * from T_Product where Id=@Id",
                        new MySqlParameter("@Id", id));

                    if (products.Rows.Count <= 0)
                    {
                        context.Response.Write("找不到Id=" + id + "的产品");
                    }
                    else if (products.Rows.Count > 1)
                    {
                        context.Response.Write("找到多个Id=" + id + "的产品");
                    }
                    else
                    {
                        DataRow row = products.Rows[0];
                        var data = new
                        {
                            Title = "编辑产品",
                            Action = action,
                            Product = row,
                            Categories = categroies.Rows
                        };
                        string html = CommonHelper.RenderHtml("Admin/ProductEdit.html", data);
                        context.Response.Write(html);
                    }
                }
                else
                {
                    context.Response.Write("Action错误" + action);
                }
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