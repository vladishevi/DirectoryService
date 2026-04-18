using System.Collections;
using System.Text.Json.Serialization;

namespace Shared;

public record Errors : IEnumerable<Error>
{
    private readonly IEnumerable<Error> _errors;

    public Errors(IEnumerable<Error> errors)
    {
        _errors = [.. errors];
    }

    public Errors(Error error)
    {
        _errors = [error];
    }

    public IEnumerator<Error> GetEnumerator() => _errors.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}