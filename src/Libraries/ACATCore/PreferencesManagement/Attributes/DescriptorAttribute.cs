using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACAT.Core.PreferencesManagement
{
    public class DescriptorAttribute : Attribute
    {
        public DescriptorAttribute(String description)
        {
            Description = description;
            Category = "";
        }

        public DescriptorAttribute(String description, String category) : this(description)
        {
            Category = category;
        }

        public string Description { get; }
        public string Category { get; }
    }
}
