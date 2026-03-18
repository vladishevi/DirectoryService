using System.Text;

namespace DirectoryService.Domain.Departments;

public record Path
{
    public Path(Identifier identifier, Department? parentDepartment)
    {
        StringBuilder pathBuilder = new();
        pathBuilder.Insert(0, identifier.Value);

        Department? parent = parentDepartment;
        while (parent != null)
        {
            pathBuilder.Insert(0, $"{parent.Path.Value}.");
            parent = parent.ParentDepartment;
        }

        Value = pathBuilder.ToString();
    }

    private string Value { get; set; }
}