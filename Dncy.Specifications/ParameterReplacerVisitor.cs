using System.Linq.Expressions;

namespace Dotnetydd.Specifications
{
    public class ParameterReplacerVisitor : ExpressionVisitor
    {
        private readonly Expression newExpression;

        private readonly ParameterExpression oldParameter;

        private ParameterReplacerVisitor(ParameterExpression oldParameter, Expression newExpression)
        {
            this.oldParameter = oldParameter;
            this.newExpression = newExpression;
        }

        public static Expression Replace(Expression expression, ParameterExpression oldParameter,
            Expression newExpression)
        {
            return new ParameterReplacerVisitor(oldParameter, newExpression).Visit(expression);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (p == oldParameter)
            {
                return newExpression;
            }

            return p;
        }
    }
}

