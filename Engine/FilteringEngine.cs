using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Engine.Enums;
using Engine.Types;

namespace Engine
{
    public class FilteringEngine
    {
        private const string PARAMETER_NAME = "x";
        private const string METHOD_NAME = "Where";

        private readonly Dictionary<ComparisonType, ExpressionType> ComparisonTypeMap = new Dictionary<ComparisonType, ExpressionType>
        {
            { ComparisonType.Equal, ExpressionType.Equal },
            { ComparisonType.GreaterThan, ExpressionType.GreaterThan },
            { ComparisonType.GreaterThanOrEqual, ExpressionType.GreaterThanOrEqual },
            { ComparisonType.LessThan, ExpressionType.LessThan },
            { ComparisonType.LessThanOrEqual, ExpressionType.LessThanOrEqual }
        };

        private readonly Dictionary<ConditionGroupType, Func<Expression, Expression, BinaryExpression>> ConditionGroupTypeMap = new Dictionary<ConditionGroupType, Func<Expression, Expression, BinaryExpression>>
        {
            { ConditionGroupType.Or, Expression.OrElse },
            { ConditionGroupType.And, Expression.AndAlso }
        };

        public T[] Filter<T>(IEnumerable<T> data, ConditionGroup group)
        {
            var queryable = data.AsQueryable();

            var parameterExpression = CreateParameter(typeof(T), PARAMETER_NAME);

            var expression = BuildExpression(parameterExpression, group);

            var where = Expression.Call(
                typeof(Queryable),
                METHOD_NAME,
                new Type[] { queryable.ElementType },
                queryable.Expression,
                Expression.Lambda<Func<T, bool>>(expression, parameterExpression)
            );

            var query = queryable.Provider.CreateQuery<T>(where);

            return query.ToArray();
        }

        private ParameterExpression CreateParameter(Type type, string parameterName)
        {
            return Expression.Parameter(type, parameterName);
        }

        private ConstantExpression CreateConstant(Condition condition)
        {
            return Expression.Constant(condition.Value);
        }

        private MemberExpression CreateProperty(ParameterExpression parameterExpression, Condition condition)
        {
            return Expression.Property(parameterExpression, condition.Property);
        }

        private BinaryExpression CreateConditionExpression(ParameterExpression parameterExpression, Condition condition)
        {
            var propertyExpression = CreateProperty(parameterExpression, condition);

            var constantExpression = CreateConstant(condition);

            return Expression.MakeBinary(ComparisonTypeMap[condition.ComparisonType], propertyExpression, constantExpression);
        }

        private BinaryExpression BuildExpression(ParameterExpression parameterExpression, ConditionGroup group)
        {
            if (group.Condition != null)
            {
                return CreateConditionExpression(parameterExpression, group.Condition);
            }

            var expression = BuildExpression(parameterExpression, group.Conditions.ElementAt(0));

            var merge = ConditionGroupTypeMap[group.ConditionType.Value];

            for (int i = 1; i < group.Conditions.Count(); i++)
            {
                expression = merge(expression, BuildExpression(parameterExpression, group.Conditions.ElementAt(i)));
            }

            return expression;
        }
    }
}
