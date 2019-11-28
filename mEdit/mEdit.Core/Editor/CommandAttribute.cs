using System;

namespace mEdit.Core.Editor
{
    
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string target)
        {
            Target = target;
        }

        public string Target { get; set; }
    }
}