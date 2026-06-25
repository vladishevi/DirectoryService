using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation.Results;
using Shared;
using Shared.Errors;

namespace DirectoryService.Application.Validation;

public static class ValidationExtensions
{
    public static Errors ToErrors(this ValidationResult validationResult)
    {
        var errorsMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
        var errors = errorsMessages
            .Select(errorsMessage => JsonSerializer.Deserialize<List<Error>>(errorsMessage,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })).Select(error => error.First())
            .ToList();

        return new Errors(errors);
    }
}