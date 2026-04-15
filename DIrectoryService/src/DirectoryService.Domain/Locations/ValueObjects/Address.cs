using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Locations;

public sealed record Address
{
    private Address(string city, string street, int building, string postcode)
    {
        City = city;
        Street = street;
        Building = building;
        Postcode = postcode;
    }
    
    public string City { get;}
    public string Street { get; }
    public int Building { get; }
    public string Postcode { get; }

    public static Result<Address, Errors> Create(string city, string street, int building, string postcode)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return GeneralErrors.ValueIsInvalid("City cannot be empty", "Location.Address.City").ToErrors();
        }
        
        if (string.IsNullOrWhiteSpace(street))
        {
            return GeneralErrors.ValueIsInvalid("Street cannot be empty", "Location.Address.Street").ToErrors();
        }
        
        if (string.IsNullOrWhiteSpace(postcode))
        {
            return GeneralErrors.ValueIsInvalid("Postcode cannot be empty", "Location.Address.Postcode").ToErrors();
        }
        
        if (building <= 0)
        {
            return GeneralErrors.ValueIsInvalid("Building number must be greater than 0", "Location.Address.Building").ToErrors();
        }
        
        return new Address(city, street, building, postcode);
    }
}