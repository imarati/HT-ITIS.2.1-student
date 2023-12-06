using System.Linq.Expressions;
using Hw11.ErrorMessages;

namespace Hw11.Services.MathCalculator;

public class ParallelEvaluationVisitor
{
    public async Task<Expression> VisitBinaryAsync(BinaryExpression node)
    {
        var firstTask = new Lazy<Task<Expression>>(async () =>
        {
            await Task.Delay(1000);
            if (node.Left is BinaryExpression binaryLeft)
                return await VisitBinaryAsync(binaryLeft);
            if (node.Left is UnaryExpression unaryLeft)
                return await VisitUnaryAsync(unaryLeft);
            return node.Left;
        });
        var secondTask = new Lazy<Task<Expression>>(async () =>
        {
            await Task.Delay(1000);
            if (node.Right is BinaryExpression binaryLeft)
                return await VisitBinaryAsync(binaryLeft);
            if (node.Right is UnaryExpression unaryRight)
                return await VisitUnaryAsync(unaryRight);
            return node.Right;
        });

        var result = await Task.WhenAll(firstTask.Value, secondTask.Value);

        if (node.NodeType == ExpressionType.Divide)
        {
            if (Expression.Lambda<Func<double>>(result[1]).Compile().Invoke() == 0)
                throw new DivideByZeroException(MathErrorMessager.DivisionByZero);
        }

        return node.NodeType switch
        {
            ExpressionType.Add => Expression.Add(result[0], result[1]),
            ExpressionType.Subtract => Expression.Subtract(result[0], result[1]),
            ExpressionType.Multiply => Expression.Multiply(result[0], result[1]),
            _ => Expression.Divide(result[0], result[1])
        };
    }

    public async Task<UnaryExpression> VisitUnaryAsync(UnaryExpression node)
    {
        return node;
    }

    public Expression VisitConstant(ConstantExpression node)
    {
        return node;
    }
}