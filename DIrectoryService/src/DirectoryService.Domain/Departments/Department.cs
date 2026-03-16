namespace DirectoryService.Domain.Departments;

public class Department
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Identifier { get; private set; }
    public Guid? ParentId { get; private set; }
    public string Path { get; private set; }
    public short Depth { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt  { get; private set; }
    public DateTime UpdatedAt  { get; private set; }
}