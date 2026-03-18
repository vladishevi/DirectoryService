using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Departments;

public class Department
{
    private Department(Name name, Identifier identifier, Department? parentDepartment, Path path)
    {
        Id = Guid.NewGuid();
        Name = name;
        Identifier = identifier;
        ParentDepartment = parentDepartment;
        Path = path;
    }
    
    public Guid Id { get; private set; }
    public Name Name { get; private set; }
    public Identifier Identifier { get; private set; }
    public Department? ParentDepartment { get; private set; }
    public Path Path { get; private set; }
    public short Depth { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt  { get; private set; }
    public DateTime UpdatedAt  { get; private set; }

    public static Department Create(Name name, Identifier identifier, Department? parentDepartment)
    {
        Path path = new(identifier, parentDepartment);
        return new Department(name, identifier, parentDepartment, path);
    }
}