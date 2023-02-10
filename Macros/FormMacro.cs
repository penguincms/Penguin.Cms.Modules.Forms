using Penguin.Reflection.Extensions;
using Penguin.Templating.Abstractions.Interfaces;
using System.Collections.Generic;

namespace Penguin.Cms.Modules.Forms.Macros
{
    public class FormMacro : ITemplateProperty
    {
        public List<FormMacro> Children { get; } = new List<FormMacro>();

        public string DisplayName { get; set; } = "Form Macros";

        public string MacroBody => $"@(Model.{DisplayName.ToVariableName()})";

        IEnumerable<ITemplateProperty> ITemplateProperty.Children => Children;

        public FormMacro()
        {
        }

        public FormMacro(string FieldName)
        {
            DisplayName = FieldName;
        }
    }
}