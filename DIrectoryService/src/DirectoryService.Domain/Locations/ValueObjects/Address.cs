using CSharpFunctionalExtensions;

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

    public static Result<Address, string> Create(string city, string street, int building, string postcode)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return "City cannot be empty";
        }
        
        if (string.IsNullOrWhiteSpace(street))
        {
            return "Street cannot be empty";
        }
        
        if (string.IsNullOrWhiteSpace(postcode))
        {
            return "Postcode cannot be empty";
        }
        
        if (building <= 0)
        {
            return "Building number must be greater than 0";
        }
        
        return new Address(city, street, building, postcode);
    }
}