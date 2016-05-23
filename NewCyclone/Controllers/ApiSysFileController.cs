using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NewCyclone.Models;

namespace NewCyclone.Controllers
{
    /// <summary>
    /// 文件服务
    /// </summary>
    public class ApiSysFileController : ApiController
    {
        /// <summary>
        /// 列出upload文件夹下的目录与文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpGet]
        public BaseResponse<VMDiskFileQueryResponse> queryDiskFiles(string path) {
            BaseResponse<VMDiskFileQueryResponse> result = new BaseResponse<VMDiskFileQueryResponse>();
            try
            {
                result.result = SysFile.listFiles(path);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, path);
            }
            return result;
        }

        /// <summary>
        /// 删除upload文件夹中的文件
        /// </summary>
        /// <param name="condtion">文件路径集合</param>
        /// <returns></returns>
        [Authorize(Roles ="admin")]
        [HttpPost]
        public BaseResponse delFiles(VMEditListRequest<string> condtion) {
            BaseResponse result = new BaseResponse();
            try
            {
                SysFile.deleteFile(condtion.rows);
                result.msg = "文件删除成功";
                string urls = String.Join(",", condtion.rows);
                SysUserLog.saveLog("直接删除文件:" + condtion.rows.Count + "个，" + urls, SysUserLogType.删除);
            }
            catch (SysException e)
            {
                result = e.getresult(result);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }

        /// <summary>
        /// 删除upload文件夹中的目录
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public BaseResponse delDirs(VMEditListRequest<string> condtion) {
            BaseResponse result = new BaseResponse();
            try
            {
                foreach (string dir in condtion.rows) {
                    SysFile.deleteDirs(dir);
                }
                result.msg = "文件夹删除成功";
                string urls = String.Join(",", condtion.rows);
                SysUserLog.saveLog("直接删除文件夹:" + condtion.rows.Count + "个，" + urls, SysUserLogType.删除);
            }
            catch (SysException e)
            {
                result = e.getresult(result);
            }
            catch (Exception e)
            {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }

        /// <summary>
        /// 调整文件排序
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public BaseResponse<List<SysFileSort>> editFilesSort(VMEditListRequest<VMEditFileSortRequest> condtion) {
            BaseResponse<List<SysFileSort>> result = new BaseResponse<List<SysFileSort>>();
            try
            {
                result.result = SysFileSort.editSort(condtion);
                result.msg = "文件排序调整成功";
            }
            catch (SysException e)
            {
                result = e.getresult(result);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }

        /// <summary>
        /// 编辑带描述的文件信息与排序
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public BaseResponse<List<SysFileInfo>> editFilesInfo(VMEditListRequest<VMEditFileInfoRequest> condtion) {
            BaseResponse<List<SysFileInfo>> result = new BaseResponse<List<SysFileInfo>>();
            try
            {
                result.result = SysFileInfo.editInfo(condtion);
                result.msg = "文件信息调整成功";
            }
            catch (SysException e)
            {
                result = e.getresult(result);
            }
            catch (Exception e)
            {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }

        /// <summary>
        /// 根据原图像文件的地址获取他的缩略图
        /// </summary>
        /// <param name="width">宽（单位：像素）</param>
        /// <param name="height">高（单位：像素）</param>
        /// <param name="url">图片地址  /path/file.ext </param>
        /// <returns>返回缩略图的地址</returns>
        [HttpGet]
        public BaseResponse<string> getImgThumbnail(int width, int height, string url) {
            BaseResponse<string> result = new BaseResponse<string>();
            try
            {
                SysFile file = new SysFile(new VMCreateFileRequest() {
                    url = url
                });
                result.result = file.getThumbnail(width, height);
            }
            catch (SysException e)
            {
                result = e.getresult(result, true);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, string.Format("width:{0},height:{1},url:{2}", width, height, url));
            }
            return result;
        }
    }
}
