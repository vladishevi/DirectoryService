using System.Text;

namespace DirectoryService.Domain.Departments;

public record Path
{
    public Path(string value)
    {
        Value = value;
    }
    
    public Path(Identifier identifier, Department? parentDepartment)
    {
        Value = GetDepartmentPath(identifier, parentDepartment);
    }

    public string Value { get; }

    private string GetDepartmentPath(Identifier identifier, Department? parentDepartment)
    {
        return parentDepartment == null ?
            identifier.Value : 
            $"{parentDepartment.Path.Value}.{identifier.Value}";
    }
}