using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Engine.Enums.Selection;
using Engine.Types.Selection;

namespace Engine
{
    public class SelectionEngine
    {
        private const string PARAMETER_NAME = "x";

        private readonly Dictionary<StringConversionType, string> StringConversionTypeMap = new Dictionary<StringConversionType, string>
        {
            { StringConversionType.Lowercase, nameof(string.ToLower) },
            { StringConversionType.Uppercase, nameof(string.ToUpper) }
        };

        public TOut[] Select<TIn, TOut>(IEnumerable<TIn> data, IEnumerable<SelectionMapping> mappings)
        {
            var parameterExpression = CreateParameter(typeof(TIn), PARAMETER_NAME);

            var newExpression = Expression.New(typeof(TOut));

            var bindings = mappings.Select(x => CreateMemberAssignment<TOut>(parameterExpression, x));

            var memberInitExpression = Expression.MemberInit(newExpression, bindings);

            var lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, parameterExpression);

            var select = lambda.Compile();

            return data.Select(select).ToArray();
        }

        private ParameterExpression CreateParameter(Type type, string parameterName)
        {
            return Expression.Parameter(type, parameterName);
        }

        private MemberAssignment CreateMemberAssignment<T>(ParameterExpression parameterExpression, SelectionMapping mapping)
        {
            var memberInfo = typeof(T).GetProperty(mapping.OutputProperty);

            var propertyExpression = Expression.Property(parameterExpression, mapping.InputProperty);

            var assignmentExpression = (Expression)propertyExpression;

            if (mapping.StringConversionType.HasValue)
            {
                assignmentExpression = CreateStringConversionExpression(propertyExpression, mapping.StringConversionType.Value);
            }

            return Expression.Bind(memberInfo, assignmentExpression);
        }

        private Expression CreateStringConversionExpression(MemberExpression propertyExpression, StringConversionType stringConversionType)
        {
            var methodName = StringConversionTypeMap[stringConversionType];

            return Expression.Call(propertyExpression, typeof(string).GetMethod(methodName, Type.EmptyTypes));
        }
    }
}
