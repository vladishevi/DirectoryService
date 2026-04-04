namespace DirectoryService.Contracts.Locations;

public record CreateLocationDto(string Name, string City, string Street, int Building, string Postcode, string Timezone);
