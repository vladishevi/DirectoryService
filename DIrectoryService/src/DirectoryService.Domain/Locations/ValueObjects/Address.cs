using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Locations;

public record Address
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
            return new Errors(Error.Validation("City cannot be empty", invalidField: "Location.Address.City"));
        }
        
        if (string.IsNullOrWhiteSpace(street))
        {
            return new Errors(Error.Validation("Street cannot be empty", invalidField: "Location.Address.Street"));
        }
        
        if (string.IsNullOrWhiteSpace(postcode))
        {
            return new Errors(Error.Validation("Postcode cannot be empty", invalidField: "Location.Address.Postcode"));
        }
        
        if (building <= 0)
        {
            return new Errors(Error.Validation("Building number must be greater than 0", invalidField: "Location.Address.Building"));
        }
        
        return new Address(city, street, building, postcode);
    }
}