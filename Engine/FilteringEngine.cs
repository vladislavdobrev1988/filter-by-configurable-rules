using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Engine.Enums.Filtering;
using Engine.Types.Filtering;

namespace Engine
{
    public class FilteringEngine
    {
        private const string PARAMETER_NAME = "x";

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
            var parameterExpression = CreateParameter(typeof(T), PARAMETER_NAME);

            var expression = BuildExpression(parameterExpression, group);

            var lambda = Expression.Lambda<Func<T, bool>>(expression, parameterExpression);

            var where = lambda.Compile();

            return data.Where(where).ToArray();
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

            var convertExpression = Expression.ConvertChecked(constantExpression, propertyExpression.Type);

            return Expression.MakeBinary(ComparisonTypeMap[condition.ComparisonType], propertyExpression, convertExpression);
        }

        private BinaryExpression BuildExpression(ParameterExpression parameterExpression, ConditionGroup group)
        {
            if (group.Condition != null)
            {
                return CreateConditionExpression(parameterExpression, group.Condition);
            }

            var expression = BuildExpression(parameterExpression, group.Conditions[0]);

            var merge = ConditionGroupTypeMap[group.ConditionType.Value];

            for (int i = 1; i < group.Conditions.Length; i++)
            {
                expression = merge(expression, BuildExpression(parameterExpression, group.Conditions[i]));
            }

            return expression;
        }
    }
}
