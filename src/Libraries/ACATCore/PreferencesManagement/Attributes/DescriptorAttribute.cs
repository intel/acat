using System;

namespace ACAT.Core.PreferencesManagement.Attributes
{
    public class DescriptorAttribute : Attribute
    {
        public DescriptorAttribute(string description)
        {
            Description = description;
            Category = "";
        }

        public DescriptorAttribute(string description, string category) : this(description)
        {
            Category = category;
        }

        public string Description { get; }
        public string Category { get; }
    }
}
