using System.Linq.Expressions;
using Hw10.Dto;

namespace Hw10.Services.MathCalculator;

public class MathCalculatorService : IMathCalculatorService
{
    public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
    {
        try
        {
            MathExpressionValidatorService.ValidateExpression(expression);
            var tree = MathParserService.ParseExpression(expression!);
            var result = Expression.Lambda<Func<double>>(
                    await ParallelEvaluationVisitor.VisitExpression(tree))
                .Compile()
                .Invoke();

            return new CalculationMathExpressionResultDto(result);
        }
        catch (Exception ex)
        {
            return new CalculationMathExpressionResultDto(ex.Message);
        }
    }
}