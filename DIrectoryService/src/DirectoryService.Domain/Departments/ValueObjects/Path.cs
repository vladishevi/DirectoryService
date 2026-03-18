using System.Text;
using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Departments;

public record Path
{
    private Path(string value)
    {
        Value = value;
    }
    
    public string Value { get; private set; }
    
    public static Result<Path, string> Create(string identifier, Department parentDepartment)
    {
        StringBuilder pathBuilder = new StringBuilder();
        pathBuilder.Insert(0, identifier);

        Department? parent = parentDepartment;
        while (parent != null)
        {
            pathBuilder.Insert(0, parent.Identifier);
            parent = parent.ParentDepartment;
        }
        
        return new Path(pathBuilder.ToString());   
    }
}