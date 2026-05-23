using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PRN232.Lms.Services.Helpers;

public static class QueryHelper
{
    public static IQueryable<T> ApplyExpansion<T>(IQueryable<T> query, string? expand) where T : class
    {
        if (string.IsNullOrWhiteSpace(expand)) return query;

        var relations = expand.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(r => r.Trim())
                              .ToList();

        foreach (var rel in relations)
        {
            if (string.IsNullOrEmpty(rel)) continue;
            var capitalized = char.ToUpper(rel[0]) + rel.Substring(1);
            try
            {
                query = query.Include(capitalized);
            }
            catch
            {
                // Ignore invalid relations to prevent crash
            }
        }

        return query;
    }

    public static IQueryable<T> ApplySorting<T>(IQueryable<T> query, string? sortString)
    {
        if (string.IsNullOrWhiteSpace(sortString)) return query;

        var sortParts = sortString.Split(',', StringSplitOptions.RemoveEmptyEntries);
        bool isFirst = true;

        foreach (var part in sortParts)
        {
            var trimmed = part.Trim();
            bool descending = trimmed.StartsWith("-");
            var propertyName = descending ? trimmed.Substring(1) : trimmed;

            var property = typeof(T).GetProperty(propertyName, 
                System.Reflection.BindingFlags.IgnoreCase | 
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.Instance);

            if (property == null) continue;

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var keySelector = Expression.Lambda(propertyAccess, parameter);

            string methodName;
            if (isFirst)
            {
                methodName = descending ? "OrderByDescending" : "OrderBy";
                isFirst = false;
            }
            else
            {
                methodName = descending ? "ThenByDescending" : "ThenBy";
            }

            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), property.PropertyType },
                query.Expression,
                Expression.Quote(keySelector)
            );

            query = query.Provider.CreateQuery<T>(resultExpression);
        }

        return query;
    }

    public static object SelectFields<T>(T source, string? fields)
    {
        if (string.IsNullOrWhiteSpace(fields)) return source!;

        var fieldList = fields.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(f => f.Trim().ToLower())
                              .ToList();

        var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
        var properties = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (fieldList.Contains(prop.Name.ToLower()))
            {
                var value = prop.GetValue(source);
                expando[prop.Name] = value!;
            }
        }

        return expando;
    }
}
