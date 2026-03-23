using System.Text;

namespace DirectoryService.Domain.Departments;

public record Path
{
    public Path(Identifier identifier, Department? parentDepartment)
    {
        BuildDepartmentPath(identifier, parentDepartment);
    }

    private string Value { get; set; }

    private void BuildDepartmentPath(Identifier identifier, Department? parentDepartment)
    {
        Value = parentDepartment == null ?
            identifier.Value : 
            $"{parentDepartment.Path.Value}.{identifier.Value}";
    }
}