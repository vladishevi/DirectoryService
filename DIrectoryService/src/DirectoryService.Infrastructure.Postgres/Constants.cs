namespace DirectoryService.Infrastructure.Postgres;

public static class Constants
{
    public const string DATABASE = "DirectoryServiceDb";

    public static class Indexes
    {
        public const string LOCATION_ADDRESS = "ix_locations_address";
        public const string LOCATION_NAME = "ix_locations_name";

        public const string DEPARTMENT_NAME = "ix_departments_name";
        public const string DEPARTMENT_IDENTIFIER = "ix_departments_identifier";
    }
}