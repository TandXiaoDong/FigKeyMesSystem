using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.Model
{

    public enum LoginResult
    {
        /// <summary>
        /// 用户名错误
        /// </summary>
        USER_NAME_ERR,
        /// <summary>
        /// 用户密码错误
        /// </summary>
        USER_PWD_ERR,
        /// <summary>
        /// 用户名或密码错误
        /// </summary>
        USER_NAME_PWD_ERR,
        /// <summary>
        /// 其他异常
        /// </summary>
        FAIL_EXCEP,
        /// <summary>
        /// 成功
        /// </summary>
        SUCCESS
    }

    public enum QueryResult
    {
        /// <summary>
        /// 数据不为空，查询成功
        /// </summary>
        EXIST_DATA,
        /// <summary>
        /// 查询失败，数据位空
        /// </summary>
        NONE_DATE,
        /// <summary>
        /// 查询错误
        /// </summary>
        EXCEPT_ERR
    }

    public enum RegisterResult
    {
        /// <summary>
        /// 注册成功
        /// </summary>
        REGISTER_SUCCESS,
        /// <summary>
        /// 用户已存在
        /// </summary>
        REGISTER_EXIST_USER,
        /// <summary>
        /// 执行失败，可能是SQL语法错误
        /// </summary>
        REGISTER_FAIL_SQL,
        /// <summary>
        /// 执行错误
        /// </summary>
        REGISTER_ERR
    }

    public enum LoginUser
    {
        /// <summary>
        /// 普通用户
        /// </summary>
        ORDINARY_USER,
        /// <summary>
        /// 管理员
        /// </summary>
        ADMIN_USER,
        /// <summary>
        /// 游客
        /// </summary>
        TOURIST_USER
    }

    public class LoginInfo
    {
        
    }
}
