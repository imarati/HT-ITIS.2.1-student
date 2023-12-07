using System.Linq.Expressions;

namespace Hw11.Services.MathCalculator;

public class MathCalculatorService : IMathCalculatorService
{
    public async Task<double> CalculateMathExpressionAsync(string? expression)
    {
        MathExpressionValidatorService.ValidateExpression(expression);
        var tree = MathParserService.ParseExpression(expression!);
        return await ExpressionVisitorDispatcher.ExpressionVisitorDispatcher.Visit((dynamic)tree);
    }
}
