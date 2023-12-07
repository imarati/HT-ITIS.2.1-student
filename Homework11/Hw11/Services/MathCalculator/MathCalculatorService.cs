using System.Linq.Expressions;

namespace Hw11.Services.MathCalculator;
using static Hw11.ErrorMessages.MathErrorMessager;

public class MathCalculatorService : IMathCalculatorService
{
    public async Task<double> CalculateMathExpressionAsync(string? expression)
    {
        MathExpressionValidatorService.ValidateExpression(expression);
        var tree = MathParserService.ParseExpression(expression!);
        var result = await ExpressionVisitorDispatcher.ExpressionVisitorDispatcher.Visit((dynamic)tree);
        if (double.IsFinite(result))
            return result;ы
        throw new DivideByZeroException(DivisionByZero);
    }
}
