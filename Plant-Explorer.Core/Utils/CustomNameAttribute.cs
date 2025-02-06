namespace Plant_Explorer.Core.Utils
{
    [AttributeUsage(AttributeTargets.All)]
    public class CustomNameAttribute : Attribute
    {
        public string Name { get; }

        public CustomNameAttribute(string name)
        {
            Name = name;
        }
    }
}
