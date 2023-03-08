using HW.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace BedAppManage.Core
{
    public class Upload : baseHandlerAPI
    {
        #region 上传图片
        public bool UploadImg(HttpRequest request, out string img ,string classNmae)
        {
            bool result = true;
            try
            {
                HttpPostedFile _upfile = request.Files[0];

                //var filepath = @"C:/bedManage/types/";
                var filepath = System.Web.HttpContext.Current.Server.MapPath("~/upload/"+ classNmae + "/");

                if (!System.IO.Directory.Exists(filepath))
                {
                    System.IO.Directory.CreateDirectory(filepath);
                }
                string extension = Path.GetExtension(_upfile.FileName);
                string savePath = DateTime.Now.ToString("yyyyMMddHHmmss") + extension;

                _upfile.SaveAs(filepath + savePath);

                string virtualPath = "/upload/"+ classNmae + "/" + savePath;
                img = virtualPath;

                if (string.IsNullOrEmpty(savePath)) {
                    img = "";
                    result = false;
                }

            }
            catch {
                img = "";
                result = false;
            }
            return result;

        }
        #endregion

        #region 上传图片
        public bool UploadImgByImgSrc(string up_image_src, out string img, string classNmae)
        {
            bool result = true;
            try
            {
                string imgBase = up_image_src;//传递过来的base64编码
                string imgFomate = "png";//传递过来的图片格式
                string imgReadyBase = imgBase.Substring(imgBase.IndexOf("4") + 2);//截取base64编码无用开头
                byte[] bytes = System.Convert.FromBase64String(imgReadyBase);//base64转为byte数组
                MemoryStream ms = new MemoryStream(bytes);//创建内存流，将图片编码导入
                Image Img = Image.FromStream(ms);//将流中的图片转换为Image图片对象
                                                 //利用时间种子解决伪随机数短时间重复问题
                Random ran = new Random((int)DateTime.Now.Ticks);
                //文件保存位置及命名，精确到毫秒并附带一组随机数，防止文件重名，数据库保存路径为此变量
                string s = ran.Next().ToString();
                string serverPath = System.Web.HttpContext.Current.Server.MapPath("~/upload/" + classNmae + "/");

                if (!System.IO.Directory.Exists(serverPath))
                {
                    System.IO.Directory.CreateDirectory(serverPath);
                }
                //路径映射为绝对路径
                string savePath = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + imgFomate;
                ImageFormat imgfor = ImageFormat.Jpeg;//设置图片格式
                if (imgFomate == "png") imgfor = ImageFormat.Png;
                try
                {
                    Img.Save(serverPath+ savePath, imgfor);//图片保存
                }
                catch
                {
                }
                string virtualPath = "/upload/" + classNmae + "/" + savePath;
                img = virtualPath;

                if (string.IsNullOrEmpty(savePath))
                {
                    img = "";
                    result = false;
                }

            }
            catch
            {
                img = "";
                result = false;
            }
            return result;

        }
        #endregion
    }
}