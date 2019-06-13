using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using NLog;

namespace ErpDb
{
    public class NLogCommandInterceptor : IDbCommandInterceptor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void NonQueryExecuting(
            DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            //Logger.Info($"here is NonQueryExecuting {command}");
            //LogIfNonAsync(command, interceptionContext);
        }

        public void NonQueryExecuted(
            DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            //Logger.Info($"here is NonQueryExecuted {command}");
            //LogIfError(command, interceptionContext);
        }

        public void ReaderExecuting(
            DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
            //Logger.Info($"here is ReaderExecuting {command}");
           LogIfNonAsync(command, interceptionContext);
        }

        public void ReaderExecuted(
            DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
            //Logger.Info($"here is ReaderExecuted {command}");
          LogIfError(command, interceptionContext);
        }

        public void ScalarExecuting(
            DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
          //  Logger.Info($"here is ScalarExecuting {command}");
    //        LogIfNonAsync(command, interceptionContext);
        }

        public void ScalarExecuted(
            DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
         //   Logger.Info($"here is ScalarExecuted ");
       //     LogIfError(command, interceptionContext);
        }

        private void LogIfNonAsync<TResult>(
            DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
          
            if (!interceptionContext.IsAsync)
            {
                Logger.Warn("Non-async command used: \r\n  {0}", command.CommandText);
            }
            else
            {
                Logger.Info("async command used: \r\n {0}", command.CommandText);
            }
        }

        private void LogIfError<TResult>(
            DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
       
            if (interceptionContext.Exception != null)
            {
                Logger.Error(
                    interceptionContext.Exception,
                    $"Command failed.\r\n {command.CommandText} ");
            }
        }
    }
}