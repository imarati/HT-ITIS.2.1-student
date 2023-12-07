using System.Linq.Expressions;
using Hw11.Services.MathCalculator;

namespace Hw11.Services.ExpressionVisitorDispatcher;

public class ExpressionVisitorDispatcher
{
    public static async Task<double> Visit(BinaryExpression expression)
    {
        var exp = await new ParallelEvaluationVisitor().VisitBinaryAsync(expression);
        return Expression.Lambda<Func<double>>(exp).Compile().Invoke();
    }
    
    public static async Task<double> Visit(UnaryExpression expression)
    {
        var exp = await new ParallelEvaluationVisitor().VisitUnaryAsync(expression);
        return Expression.Lambda<Func<double>>(exp).Compile().Invoke();
    }

    public static async Task<double> Visit(ConstantExpression expression)
    {
        var value = (double)expression.Value!;
        return await Task.Run(() => value);
    }

    public static async Task<double> Visit(Expression expression)
    {
        return Visit((dynamic)expression);
    }
}