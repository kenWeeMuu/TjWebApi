﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpDb
{


    class MyDbConfig : DbConfiguration
    {
        public MyDbConfig()
        {
          
            //SetDatabaseLogFormatter(
            //    (context, writeAction) => new OneLineFormatter(context, writeAction));
        }
    }
    public class OneLineFormatter : DatabaseLogFormatter
    {
        public OneLineFormatter(DbContext context, Action<string> writeAction)
            : base(context, writeAction) {
        }

        public override void LogCommand<TResult>(
            DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext) {
            Write(string.Format(
                "Context '{0}' is executing command '{1}'{2}",
                Context.GetType().Name,
                command.CommandText.Replace(Environment.NewLine, ""),
                Environment.NewLine));
        }

        public override void LogResult<TResult>(
            DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext) {
        }
    }
}
