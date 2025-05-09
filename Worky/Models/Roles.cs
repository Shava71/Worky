namespace Worky.Models;

public class MyRoles()
{
    public string Value {get; private set;}

    public MyRoles(string value) : this()
    {
        Value = value;
    }
    
    public static MyRoles SuperAdmin => new("SuperAdmin");
    public static MyRoles Manager => new("Manager");
    public static MyRoles Company => new("Company");
    public static MyRoles Worker => new("Worker");

    public override string ToString()
    {
        return Value;
    }
}