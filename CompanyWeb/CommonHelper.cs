using MySql.Data.MySqlClient;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CompanyWeb
{
    public class CommonHelper
    {
        /// <summary>
        /// 用数据填充Modles模板，渲染生成html返回
        /// </summary>
        /// <param name="ModlesName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string RenderHtml(string ModlesName, object data)
        {
            VelocityEngine vltEngine = new VelocityEngine();
            vltEngine.SetProperty(RuntimeConstants.RESOURCE_LOADER, "file");
            vltEngine.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH,
                System.Web.Hosting.HostingEnvironment.MapPath("~/Modles"));
            vltEngine.Init();

            VelocityContext vltContext = new VelocityContext();
            vltContext.Put("Data", data);
            Template vltTemplate = vltEngine.GetTemplate(ModlesName);
            System.IO.StringWriter vltWriter = new System.IO.StringWriter();
            vltTemplate.Merge(vltContext, vltWriter);

            string html = vltWriter.GetStringBuilder().ToString();
            return html;
        }
        /// <summary>
        /// 判断是否有上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool HasFile(HttpPostedFile file) {
            if (file == null)
            {
                return false;
            }
            else {
                return file.ContentLength > 0;
            }
        }

        public static DataRowCollection GetProductCategories() {
            return MySqlHelper.ExecuteDataTable(@" select * from 
                    T_ProductCategories ").Rows;
        }

        public static string ReadSetting(string name) {
           DataTable dt= MySqlHelper.ExecuteDataTable("select value from T_Settings where Name=@Name",
                new MySqlParameter("@Name", name));
            if(dt.Rows.Count<=0){
                throw new Exception("找不到Name="+name+"的配置项");
            }else if(dt.Rows.Count>1){
                throw new Exception("找到多条数据");
            }
            return (string)dt.Rows[0]["Value"];

        }

    }
}