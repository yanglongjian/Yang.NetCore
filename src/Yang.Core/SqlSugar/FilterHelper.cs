using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Yang.Core
{
    /// <summary>
    /// 查询表达式辅助操作类
    /// </summary>
    public static class FilterHelper
    {
        #region 字段

        private static readonly Dictionary<FilterOperate, Func<Expression, Expression, Expression>> ExpressionDict =
            new()
            {
                {
                    FilterOperate.Equal,
                    Expression.Equal
                },
                {
                    FilterOperate.NotEqual,
                    Expression.NotEqual
                },
                {
                    FilterOperate.Less,
                    Expression.LessThan
                },
                {
                    FilterOperate.Greater,
                    Expression.GreaterThan
                },
                {
                    FilterOperate.LessOrEqual,
                    Expression.LessThanOrEqual
                },
                {
                    FilterOperate.GreaterOrEqual,
                    Expression.GreaterThanOrEqual
                },
                {
                    FilterOperate.StartsWith,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string))
                        {
                            throw new NotSupportedException("“StartsWith”比较方式只支持字符串类型的数据");
                        }
                        return Expression.Call(left,
                            typeof(string).GetMethod("StartsWith", new[] { typeof(string) })
                            ?? throw new InvalidOperationException($"名称为“StartsWith”的方法不存在"),
                            right);
                    }
                },
                {
                    FilterOperate.EndsWith,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string))
                        {
                            throw new NotSupportedException("“EndsWith”比较方式只支持字符串类型的数据");
                        }
                        return Expression.Call(left,
                            typeof(string).GetMethod("EndsWith", new[] { typeof(string) })
                            ?? throw new InvalidOperationException($"名称为“EndsWith”的方法不存在"),
                            right);
                    }
                },
                {
                    FilterOperate.Contains,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string))
                        {
                            throw new NotSupportedException("“Contains”比较方式只支持字符串类型的数据");
                        }
                        return Expression.Call(left,
                            typeof(string).GetMethod("Contains", new[] { typeof(string) })
                            ?? throw new InvalidOperationException($"名称为“Contains”的方法不存在"),
                            right);
                    }
                },
                {
                    FilterOperate.NotContains,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string))
                        {
                            throw new NotSupportedException("“NotContains”比较方式只支持字符串类型的数据");
                        }
                        return Expression.Not(Expression.Call(left,
                            typeof(string).GetMethod("Contains", new[] { typeof(string) })
                            ?? throw new InvalidOperationException($"名称为“Contains”的方法不存在"),
                            right));
                    }
                }
            };

        #endregion

        /// <summary>
        /// 获取指定查询条件组的查询表达式
        /// </summary>
        /// <typeparam name="T">表达式实体类型</typeparam>
        /// <param name="group">查询条件组，如果为null，则直接返回 true 表达式</param>
        /// <returns>查询表达式</returns>
        public static Expression<Func<T, bool>> GetExpression<T>(FilterRule[] rules)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), "m");
            Expression body = GetExpressionBody(param, rules);
            Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(body, param);
            return expression;
        }

        /// <summary>
        /// 获取排序条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sortConditions"></param>
        /// <returns></returns>
        public static string GetSortCondition<T>(SortCondition[] sortConditions, string defaultColumn = "", OrderByType orderByType = OrderByType.Asc)
        {
            if (sortConditions == null || sortConditions.Length == 0)
            {
                if (defaultColumn.IsNotEmpty())
                {
                    string orderType = orderByType == OrderByType.Asc ? "asc" : "desc";
                    return $"{defaultColumn} {orderType}";
                }

                if (typeof(IEntity<int>).IsAssignableFrom(typeof(T)))
                {
                    return "Id desc";
                }
                if (typeof(ICreatedTime).IsAssignableFrom(typeof(T)))
                {
                    return "CreatedTime desc";
                }
                else
                {
                    throw new ArgumentException($"类型“{typeof(T)}”未添加默认排序方式");
                }
            }
            else
            {
                StringBuilder sb = new();
                foreach (SortCondition sortCondition in sortConditions)
                {
                    var orderType = sortCondition.OrderByType == OrderByType.Asc ? "ASC" : "DESC";
                    sb.Append($",{sortCondition.SortField} {orderType}");
                }
                return sb.ToString()[1..];
            }
        }


        #region 私有方法

        private static Expression GetExpressionBody(ParameterExpression param, FilterRule[] rules)
        {
            //如果无条件或条件为空，直接返回 true表达式
            if (rules == null || rules.Length == 0)
            {
                return Expression.Constant(true);
            }
            List<Expression> bodies = new();

            bodies.AddRange(rules.Select(rule => GetExpressionBody(param, rule)));

            ///bodies.AddRange(rules.Select(subGroup => GetExpressionBody(param, subGroup)));

            return bodies.Aggregate(Expression.AndAlso);
        }

        private static Expression GetExpressionBody(ParameterExpression param, FilterRule rule)
        {
            if (rule == null)
            {
                return Expression.Constant(true);
            }
            LambdaExpression expression = GetPropertyLambdaExpression(param, rule);
            if (expression == null)
            {
                return Expression.Constant(true);
            }
            Expression constant = ChangeTypeToExpression(rule, expression.Body.Type);
            return ExpressionDict[rule.Operate](expression.Body, constant);
        }

        private static LambdaExpression GetPropertyLambdaExpression(ParameterExpression param, FilterRule rule)
        {
            string[] propertyNames = rule.Field.Split('.');
            Expression propertyAccess = param;
            Type type = param.Type;
            for (var index = 0; index < propertyNames.Length; index++)
            {
                string propertyName = propertyNames[index];
                PropertyInfo property = type.GetProperty(propertyName);
                if (property == null)
                {
                    throw new InvalidOperationException($"{rule.Field} 属性未找到");
                }

                type = property.PropertyType;
                //验证最后一个属性与属性值是否匹配
                if (index == propertyNames.Length - 1)
                {
                    bool flag = CheckFilterRule(type, rule);
                    if (!flag)
                    {
                        return null;
                    }
                }

                propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
            }
            return Expression.Lambda(propertyAccess, param);
        }

        /// <summary>
        /// 验证最后一个属性与属性值是否匹配 
        /// </summary>
        /// <param name="type">最后一个属性</param>
        /// <param name="rule">条件信息</param>
        /// <returns></returns>
        private static bool CheckFilterRule(Type type, FilterRule rule)
        {
            if (rule.Value == null || rule.Value.ToString() == string.Empty)
            {
                rule.Value = null;
            }

            if (rule.Value == null && (type == typeof(string) || type.IsNullableType()))
            {
                return rule.Operate == FilterOperate.Equal || rule.Operate == FilterOperate.NotEqual;
            }

            if (rule.Value == null)
            {
                return !type.IsValueType;
            }
            return true;
        }

        private static Expression ChangeTypeToExpression(FilterRule rule, Type conversionType)
        {
            object value = rule.Value.CastTo(conversionType);
            return Expression.Constant(value, conversionType);
        }

        /// <summary>
        /// 把对象类型转换为指定类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static object CastTo(this object value, Type conversionType)
        {
            if (value == null)
            {
                return null;
            }
            if (conversionType.IsNullableType())
            {
                conversionType = conversionType.GetUnNullableType();
            }
            if (conversionType.IsEnum)
            {
                return Enum.Parse(conversionType, value.ToString());
            }
            if (conversionType == typeof(Guid))
            {
                return Guid.Parse(value.ToString());
            }
            return Convert.ChangeType(value.ToString(), conversionType);
        }

        /// <summary>
        /// 通过类型转换器获取Nullable类型的基础类型
        /// </summary>
        /// <param name="type"> 要处理的类型对象 </param>
        /// <returns> </returns>
        public static Type GetUnNullableType(this Type type)
        {
            if (IsNullableType(type))
            {
                NullableConverter nullableConverter = new(type);
                return nullableConverter.UnderlyingType;
            }
            return type;
        }

        /// <summary>
        /// 判断是否可空类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableType(this Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        #endregion
    }
}



