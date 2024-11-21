using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Supertext.Base.Configuration;

namespace Supertext.Base.Dal.SqlServer.Utils;

public static class DalExtensions
{
    public static SqlConnectionStringBuilder CreateConnectionStringBuilder<TConfiguration>(this TConfiguration configuration,
                                                                                           Expression<Func<TConfiguration, object>> selector)
        where TConfiguration : IConfiguration
    {
        var property = GetPropertyInfo(selector);
        var connectionString = property.GetValue(configuration) as string;

        return new SqlConnectionStringBuilder(connectionString);
    }

    private static PropertyInfo GetPropertyInfo<TType, TReturn>(
        this Expression<Func<TType, TReturn>> property
    )
    {
        LambdaExpression lambda = property;
        var memberExpression = lambda.Body is UnaryExpression expression
                                   ? (MemberExpression)expression.Operand
                                   : (MemberExpression)lambda.Body;

        return (PropertyInfo)memberExpression.Member;
    }
}