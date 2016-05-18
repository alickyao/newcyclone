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
    /// 分类树
    /// </summary>
    public class ApiSysCatTreeController : ApiController
    {

        /// <summary>
        /// 检查别名是否存在
        /// </summary>
        /// <param name="alias">别名</param>
        /// <param name="wid">需要排除的ID</param>
        /// <returns>返回结果>0表示已存在</returns>
        [HttpGet]
        public int checkTreeIdIsExist(string alias, string wid = null)
        {
            return SysCatTree.checkAliasIsExist(alias, wid);
        }

        /// <summary>
        /// 添加/编辑分类树
        /// </summary>
        /// <param name="condtion">添加/编辑分类树请求参数</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public BaseResponse<SysCatTree> editCatTree(VMTreeEditCatTreeRequest condtion) {
            BaseResponse<SysCatTree> result = new BaseResponse<SysCatTree>();
            try
            {
                result.result = SysCatTree.edit(condtion);
                result.msg = "保存成功";
            }
            catch (SysException ex)
            {
                result = ex.getresult(result);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }

        /// <summary>
        /// 将树的某个节点标记为删除
        /// </summary>
        /// <param name="Id">节点的ID</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public BaseResponse delCatTree(string Id) {
            BaseResponse result = new BaseResponse();
            try
            {
                SysCatTree tree = new SysCatTree(Id);
                tree.delete();
                result.msg = "删除成功";
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, Id);
            }
            return result;
        }

        /// <summary>
        /// 根据树节点的别名获取树的分类树的信息
        /// </summary>
        /// <param name="alias">别名</param>
        /// <param name="getChild">是否获取子节点</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<SysCatTree> getTreeByalias(string alias, bool getChild = false) {
            BaseResponse<SysCatTree> result = new BaseResponse<SysCatTree>();
            try
            {
                result.result = SysCatTree.getTreeByAlias(alias, getChild);
            }
            catch (SysException e)
            {
                result = e.getresult(result);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, alias);
            }
            return result;
        }

        /// <summary>
        /// 根据功能标示获取分类树集合
        /// </summary>
        /// <param name="fun">功能标示符 已有:webcms</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<List<SysCatTree>> getTreelist(string fun) {
            BaseResponse<List<SysCatTree>> result = new BaseResponse<List<SysCatTree>>();
            try
            {
                result.result = SysCatTree.getTreeList(fun);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, fun);
            }
            return result;
        }
    }
}
