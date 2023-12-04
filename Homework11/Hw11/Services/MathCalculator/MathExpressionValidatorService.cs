using Hw11.ErrorMessages;
using Hw11.Exceptions;

namespace Hw11.Services.MathCalculator;

public static class MathExpressionValidatorService
{
    private static readonly char[] Operations = { '+', '-', '*', '/' };
    public static void ValidateExpression(string? expression)
    {
        if (string.IsNullOrEmpty(expression))
            throw new Exception(MathErrorMessager.EmptyString);
        if (!expression.StartsWith('-') && Operations.Contains(expression[0]))
            throw new Exception(MathErrorMessager.StartingWithOperation);
        if (Operations.Contains(expression[^1]))
            throw new Exception(MathErrorMessager.EndingWithOperation);
        if (!CheckParenthesis(expression))
            throw new Exception(MathErrorMessager.IncorrectBracketsNumber);

        var symbols = expression.Split(" ");
        var prev = string.Empty;

        foreach (var s in symbols)
        {
            if (s.StartsWith('(') 
                && Operations.Contains(s[1]) 
                && !s[1].Equals('-'))
                throw new InvalidSyntaxException(MathErrorMessager.InvalidOperatorAfterParenthesisMessage(s[1].ToString()));

            if (s.EndsWith(')')
                && Operations.Contains(s[^2]))
                throw new InvalidSyntaxException(MathErrorMessager.OperationBeforeParenthesisMessage(s[^2].ToString()));

            var pure = s.Replace("(", "").Replace(")", "");

            if (!(pure.Length == 1 && Operations.Contains(pure[0]))
                && !double.TryParse(pure, out var num))
            {
                foreach (var c in pure.Where(c => !char.IsDigit(c)
                                               && !c.Equals('.')
                                               && !c.Equals('(')
                                               && !c.Equals(')')
                                               && !Operations.Contains(c)))
                    throw new InvalidSymbolException(MathErrorMessager.UnknownCharacterMessage(c));

                throw new InvalidNumberException(MathErrorMessager.NotNumberMessage(s));
            }

            if (string.IsNullOrEmpty(prev))
            {
                prev = s;
                continue;
            }

            if (prev.Length == 1 && Operations.Contains(prev[0]) && pure.Length == 1 && Operations.Contains(pure[0]))
                throw new InvalidSyntaxException(MathErrorMessager.TwoOperationInRowMessage(prev, pure));

            prev = s;
        }
    }

    private static bool CheckParenthesis(string expression)
    {
        var count = 0;
        foreach (var c in expression)
        {
            if (c.Equals('(')) count++;
            if (c.Equals(')')) count--;
            if (count < 0) return false;
        }

        return count == 0;
    }
}