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
        var exp = new ParallelEvaluationVisitor().VisitConstant(expression);
        return Expression.Lambda<Func<double>>(exp).Compile().Invoke();
    }

    public static Task<double> Visit(Expression expression)
    {
        dynamic exp = (dynamic)expression;
        return Visit(exp);
    }
}