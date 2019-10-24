using Penguin.Cms.Forms;
using Penguin.Cms.Forms.Repositories;
using Penguin.Cms.Web.Extensions;
using Penguin.Email.Templating.Abstractions.Extensions;
using Penguin.Email.Abstractions.Interfaces;
using Penguin.Email.Templating.Abstractions.Interfaces;
using Penguin.Messaging.Abstractions.Interfaces;
using Penguin.Messaging.Persistence.Messages;
using Penguin.Reflection.Extensions;
using Penguin.Templating.Abstractions;
using Penguin.Templating.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Penguin.Cms.Modules.Forms.Macros
{
    public class Form : IProvideTemplates, IMessageHandler<Penguin.Messaging.Application.Messages.Startup>, IMessageHandler<Creating<JsonForm>>, IMessageHandler<Updating<JsonForm>>, IMessageHandler<Creating<SubmittedForm>>
    {
        protected ISendTemplates EmailTemplateRepository { get; set; }

        protected FormRepository FormRepository { get; set; }

        private const string FORM_BODY_MACRO = "Form Json";

        private static List<ITemplateDefinition>? Cache;

        public Form(FormRepository formRepository, ISendTemplates emailTemplateRepository = null)
        {
            EmailTemplateRepository = emailTemplateRepository;
            FormRepository = formRepository;
        }

        public void AcceptMessage(Penguin.Messaging.Application.Messages.Startup startup) => this.RefreshHandlers();

        public void AcceptMessage(Creating<JsonForm> startup) => this.RefreshHandlers();

        public void AcceptMessage(Updating<JsonForm> startup) => this.RefreshHandlers();

        public void AcceptMessage(Creating<SubmittedForm> submittedForm)
        {
            if (submittedForm is null)
            {
                throw new System.ArgumentNullException(nameof(submittedForm));
            }

            Dictionary<string, object> Parameters = new Dictionary<string, object>();

            foreach (string thisField in submittedForm.Target.GetKeys())
            {
                Parameters.Add(thisField.ToVariableName(), submittedForm.Target.GetValue(thisField));
            }

            Parameters.Add(FORM_BODY_MACRO.ToVariableName(), submittedForm.Target.FormData.PrettifyJson());

            EmailTemplateRepository.TrySendTemplate(Parameters, null, submittedForm.Target.Owner.ToString());
        }

        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
        public IEnumerable<ITemplateDefinition> GetTemplateDefinitions() => Cache ?? throw new NullReferenceException("The Cache was not populated before the request");

        private void RefreshHandlers()
        {
            List<ITemplateDefinition> toReturn = new List<ITemplateDefinition>();

            foreach (JsonForm thisForm in FormRepository.All.ToList())
            {
                FormMacro thisMacro = new FormMacro(thisForm.Name);

                TemplateDefinition toAdd = new TemplateDefinition(thisForm.Name, this.GetType(), thisForm.Guid.ToString());

                toAdd.Children.Add(new FormMacro(FORM_BODY_MACRO));

                foreach (JsonFormField thisField in thisForm.Fields)
                {
                    if (thisField.name != null)
                    {
                        toAdd.Children.Add(new FormMacro(thisField.name));
                    }
                }

                toReturn.Add(toAdd);
            }

            Cache = toReturn;
        }
    }
}