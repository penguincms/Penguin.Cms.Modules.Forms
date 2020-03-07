using Penguin.Reflection.Extensions;
using Penguin.Templating.Abstractions.Interfaces;
using System.Collections.Generic;

#pragma warning disable CA1716 // Identifiers should not match keywords

namespace Penguin.Cms.Modules.Forms.Macros
{
#pragma warning restore CA1716 // Identifiers should not match keywords

    public class FormMacro : ITemplateProperty
    {
        public List<FormMacro> Children { get; } = new List<FormMacro>();

        public string DisplayName { get; set; } = "Form Macros";

        public string MacroBody => $"@(Model.{this.DisplayName.ToVariableName()})";

        IEnumerable<ITemplateProperty> ITemplateProperty.Children => this.Children;

        public FormMacro()
        {
        }

        public FormMacro(string FieldName)
        {
            this.DisplayName = FieldName;
        }
    }
}