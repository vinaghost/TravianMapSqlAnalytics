using FluentValidation;
using System.Text;

namespace Features.Parameters
{
    public interface IPaginationParameters
    {
        int PageNumber { get; }
        int PageSize { get; }
    }

    public static class PaginationParametersExtensions
    {
        public static void PaginationKey(this IPaginationParameters parameters, StringBuilder sb)
        {
            const char SEPARATOR = '_';

            sb.Append(parameters.PageNumber);
            sb.Append(SEPARATOR);
            sb.Append(parameters.PageSize);
        }
    }

    public class PaginationParametersValidator : AbstractValidator<IPaginationParameters>
    {
        public PaginationParametersValidator()
        {
            RuleFor(x => x.PageSize)
                .NotEmpty()
                .GreaterThanOrEqualTo(10);

            RuleFor(x => x.PageNumber)
                .NotEmpty()
                .GreaterThanOrEqualTo(1);
        }
    }
}