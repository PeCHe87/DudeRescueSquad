using System;
#if UNITY_EDITOR
#endif

namespace DudeRescueSquad.Tools
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class HiddenPropertiesAttribute : Attribute
    {
        public string[] PropertiesNames;

        public HiddenPropertiesAttribute(params string[] propertiesNames)
        {
            PropertiesNames = propertiesNames;
        }
    }
}