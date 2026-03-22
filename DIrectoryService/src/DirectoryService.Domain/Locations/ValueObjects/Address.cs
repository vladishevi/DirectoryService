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
    
    public string City { get; private set; }
    public string Street { get; private set; }
    public int Building { get; private set; }
    public string Postcode { get; private set; }

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
        
        return new Address(city, street, building, postcode);
    }
}