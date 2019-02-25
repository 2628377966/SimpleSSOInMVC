using Common;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSODemoNew1.Controllers
{
    public class SSOController : Controller
    {
        //根据一次性密钥返回token

        [AllowAnonymous]
        public JsonResult Token(string cert)
        {
            var sso = HttpContext.Cache["SSO"] == null ? new List<SSO>() : (List<SSO>)HttpContext.Cache["SSO"];
            sso.RemoveAll(p => p.Time < DateTime.Now);
            var single = sso.SingleOrDefault(p => p.Cert.ToString() == cert);
            sso.Remove(single);
            HttpContext.Cache["SSO"] = sso;
            if (null != single)
                return Json(new { Flag = true, Data = single.Token });
            return Json(new { Flag = false });
        }

        //根据token查询用户信息
        [AllowAnonymous]
        public JsonResult Identity(string token)
        {
            //根据token查询用户信息
            //获取用户信息
            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(CommonConstants.RedisServer))
            {
                IDatabase db = redis.GetDatabase();
                string userStr = db.StringGet(token.ToString());
                if (string.IsNullOrEmpty(userStr))
                    return Json(new { Flag = false });
                return Json(new { Flag = true, Data = userStr });
            }

        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult Logout(string token)
        {
            //清空cookie
            CurrentUser.Set(null);
            //删除用户信息
            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(CommonConstants.RedisServer))
            {
                IDatabase db = redis.GetDatabase();
                 db.KeyDelete(token);
            }
            return Json(new { Flag = true });
        }
    }
}