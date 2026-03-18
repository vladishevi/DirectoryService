namespace DirectoryService.Domain.Departments;

public class Department
{
    private Department(Name name, 
        Identifier identifier,
        Department? parentDepartment,
        Path path,
        short depth)
    {
        Id = Guid.NewGuid();
        Name = name;
        Identifier = identifier;
        ParentDepartment = parentDepartment;
        Path = path;
        Depth = depth;
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

    public static Department Create(Name name, 
        Identifier identifier,
        Department? parentDepartment)
    {
        Path path = new(identifier, parentDepartment);
        short depth = CalcDepth(parentDepartment);
        return new Department(name, identifier, parentDepartment, path, depth);
    }

    private static short CalcDepth(Department? parentDepartment)
    {
        short depth = 1;

        Department? parent = parentDepartment;
        while (parent != null)
        {
            depth++;
            parent = parent.ParentDepartment;
        }

        return depth;
    }
}