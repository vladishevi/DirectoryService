namespace DirectoryService.Domain.Departments;

public class Department
{
    private Department(Name name, Identifier identifier, Department parentDepartment)
    {
        Id = Guid.NewGuid();
        Name = name;
        Identifier = identifier;
        ParentDepartment = parentDepartment;
    }
    
    public Guid Id { get; private set; }
    public Name Name { get; private set; }
    public Identifier Identifier { get; private set; }
    public Department? ParentDepartment { get; private set; }
    public string Path { get; private set; }
    public short Depth { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt  { get; private set; }
    public DateTime UpdatedAt  { get; private set; }

    public static Department Create(Name name, Identifier identifier, Department parentDepartment)
    {
        return new Department(name, identifier, parentDepartment);
    }
}