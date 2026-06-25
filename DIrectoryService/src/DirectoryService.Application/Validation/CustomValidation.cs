using System.Text.Json;
using CSharpFunctionalExtensions;
using FluentValidation;
using Shared;
using Shared.Errors;

namespace DirectoryService.Application.Validation;

public static class CustomValidation
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement, TValueObject>(this
        IRuleBuilder<T, TElement> ruleBuilder, Func<TElement, Result<TValueObject, Errors>> factoryMethod)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            Result<TValueObject, Errors> result = factoryMethod.Invoke(value);
            if (result.IsSuccess)
                return;

            context.AddFailure(JsonSerializer.Serialize(result.Error));
        });
    }

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> ruleBuilder, Error error)
    {
        return ruleBuilder.WithMessage(JsonSerializer.Serialize(GeneralErrors.ValueIsInvalid(error.InvalidField).ToErrors()));
    }
}