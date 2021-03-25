using Castle.DynamicProxy;
using Foundation.Dapper.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Foundation.Dapper.Interceptor
{
    /// <summary>
    /// 事务拦截器
    /// </summary>
    public class UnitOfWorkIInterceptor : IInterceptor
    {
        private IDapperExecute _dapperExecute;
        public UnitOfWorkIInterceptor(IDapperExecute dapperExecute) => _dapperExecute = dapperExecute;
        public void Intercept(IInvocation invocation)
        {
            MethodInfo methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
            UnitOfWorkAttribute transaction = methodInfo.GetCustomAttributes<UnitOfWorkAttribute>(true).FirstOrDefault();
            if (transaction != null)
            {
                _dapperExecute.StartNewTransaction();
                try
                {
                    invocation.Proceed();
                    _dapperExecute.CommitTransaction();
                }
                catch (Exception ex)
                {
                    _dapperExecute.RollBackTransaction();
                    throw ex;
                }
            }
            else invocation.Proceed();
        }
    }
}
