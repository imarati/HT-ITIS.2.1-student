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
            if (node.Left is ConstantExpression constantLeft)
                return VisitConstant(constantLeft);
            return node.Left;
        });
        var secondTask = new Lazy<Task<Expression>>(async () =>
        {
            await Task.Delay(1000);
            if (node.Right is BinaryExpression binaryLeft)
                return await VisitBinaryAsync(binaryLeft);
            if (node.Right is UnaryExpression unaryRight)
                return await VisitUnaryAsync(unaryRight);
            if (node.Right is ConstantExpression constantRight)
                return VisitConstant(constantRight);
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

    public async Task<Expression> VisitUnaryAsync(UnaryExpression node)
    {
        var operandTask = new Lazy<Task<Expression>>(async () =>
        {
            await Task.Delay(1000);
            if (node.Operand is BinaryExpression binaryOperand)
                return await VisitBinaryAsync(binaryOperand);
            if (node.Operand is UnaryExpression unaryOperand)
                return await VisitUnaryAsync(unaryOperand);
            if (node.Operand is ConstantExpression constantOperand)
                return VisitConstant(constantOperand);
            return node.Operand;
        });

        var result = await Task.WhenAll(operandTask.Value);

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

    public Expression VisitConstant(ConstantExpression node)
    {
        return node;
    }
}