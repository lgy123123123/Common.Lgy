using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Common
{
    /// <summary>
    /// 模型验证帮助类
    /// System.ComponentModel.DataAnnotations
    /// </summary>
    public static class ModelCheckUtil
    {
        /// <summary>
        /// 验证模型，返回验证结果，自行处理
        /// System.ComponentModel.DataAnnotations中的验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ValidResult CheckModelResult<T>(this T model) where T : class, new()
        {
            ValidResult result = new ValidResult();
            try
            {
                var validationContext = new ValidationContext(model);
                var results = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(model, validationContext, results, true);

                if (!isValid)
                {
                    result.IsVaild = false;
                    foreach (var item in results)
                    {
                        result.ErrorMembers.Add(new ErrorMember()
                        {
                            ErrorMessage = item.ErrorMessage,
                            ErrorMemberName = item.MemberNames.FirstOrDefault()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsVaild = false;
                result.ErrorMembers.Add(new ErrorMember() { ErrorMessage = ex.Message, ErrorMemberName = "" });
            }

            return result;
        }
        /// <summary>
        /// 验证模型，返回是否通过
        /// System.ComponentModel.DataAnnotations中的验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool CheckModel<T>(this T model) where T : class, new()
        {
            return model.CheckModelResult().IsVaild;
        }
        /// <summary>
        /// 验证方法参数
        /// System.ComponentModel.DataAnnotations中的验证
        /// </summary>
        /// <param name="model"></param>
        /// <param name="methodName">方法名，使用nameof(方法)</param>
        /// <param name="paramsObjs">按顺序，将方法的参数放入</param>
        /// <returns></returns>
        public static ValidResult CheckMethodParamsResult<T>(this T model, string methodName, params object[] paramsObjs) where T : class, new()
        {
            ValidResult result = new ValidResult();
            try
            {
                var modelType = model.GetType();
                var methodInfo = modelType.GetMethod(methodName);
                if (methodInfo is null)
                    throw new Exception("验证方法参数的方法名不正确");
                else
                {
                    var paramInfos = methodInfo.GetParameters();
                    for (var i = 0; i < paramsObjs.Length; i++)
                    {
                        var paramInfo = paramInfos[i];
                        var value = paramsObjs[i];
                        if (paramInfo.ParameterType.IsClass)
                        {
                            var results= value.CheckModelResult();
                            if (!results.IsVaild)
                            {
                                result.IsVaild = false;
                                result.ErrorMembers.AddRange(results.ErrorMembers);
                            }
                        }
                        else
                        {
                            var attrValid = paramInfo.GetCustomAttributes<ValidationAttribute>();
                            if (!attrValid.Any())
                                continue;
                            var context = new ValidationContext(paramInfo);
                            var results = new List<ValidationResult>();
                            var isValid = Validator.TryValidateValue(value, context, results, attrValid);
                            if (!isValid)
                            {
                                result.IsVaild = false;
                                foreach (var item in results)
                                {
                                    result.ErrorMembers.Add(new ErrorMember()
                                    {
                                        ErrorMessage = item.ErrorMessage,
                                        ErrorMemberName = item.MemberNames.FirstOrDefault()
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsVaild = false;
                result.ErrorMembers.Add(new ErrorMember() { ErrorMessage = ex.Message, ErrorMemberName = "" }); ;
            }

            return result;
        }

        /// <summary>
        /// 验证方法参数
        /// System.ComponentModel.DataAnnotations中的验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="methodName">方法名，使用nameof(方法)</param>
        /// <param name="paramsObjs">按顺序，将方法的参数放入</param>
        /// <returns></returns>
        public static bool CheckMethodParams<T>(this T model, string methodName, params object[] paramsObjs) where T : class, new()
        {
            return model.CheckMethodParamsResult(methodName, paramsObjs).IsVaild;
        }
    }

    /// <summary>
    /// 验证结果
    /// </summary>
    public class ValidResult
    {
        /// <summary>
        /// 错误结果
        /// </summary>
        public List<ErrorMember> ErrorMembers { get; set; } = new List<ErrorMember>();
        /// <summary>
        /// 验证是否通过
        /// </summary>
        public bool IsVaild { get; set; } = true;
    }
    /// <summary>
    /// 报错信息
    /// </summary>
    public class ErrorMember
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 发生错误的变量名
        /// </summary>
        public string ErrorMemberName { get; set; }
    }
}
