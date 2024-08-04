﻿using FluentValidation;
using System.Text;

namespace Features.Shared.Parameters
{
    public interface ISearchTermParameters
    {
        string? SearchTerm { get; }
    }

    public static class SearchTermParametersExtensions
    {
        public static void SearchTermKey(this ISearchTermParameters parameters, StringBuilder sb)
        {
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                sb.Append(parameters.SearchTerm);
            }
        }
    }

    public class SearchTermParametersValidator : AbstractValidator<ISearchTermParameters>
    {
        public SearchTermParametersValidator()
        {
            RuleFor(x => x.SearchTerm)
                .NotEmpty();
        }
    }
}