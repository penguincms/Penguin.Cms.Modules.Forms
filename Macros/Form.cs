using Penguin.Cms.Forms;
using Penguin.Cms.Forms.Repositories;
using Penguin.Cms.Web.Extensions;
using Penguin.Email.Templating.Abstractions.Extensions;
using Penguin.Email.Templating.Abstractions.Interfaces;
using Penguin.Messaging.Abstractions.Interfaces;
using Penguin.Messaging.Persistence.Messages;
using Penguin.Reflection.Extensions;
using Penguin.Templating.Abstractions;
using Penguin.Templating.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Penguin.Cms.Modules.Forms.Macros
{
    public class Form : IProvideTemplates, IMessageHandler<Penguin.Messaging.Application.Messages.Startup>, IMessageHandler<Creating<JsonForm>>, IMessageHandler<Updating<JsonForm>>, IMessageHandler<Creating<SubmittedForm>>
    {
        private const string FORM_BODY_MACRO = "Form Json";
        private static List<ITemplateDefinition>? Cache;
        protected ISendTemplates EmailTemplateRepository { get; set; }

        protected FormRepository FormRepository { get; set; }

        public Form(FormRepository formRepository, ISendTemplates emailTemplateRepository = null)
        {
            this.EmailTemplateRepository = emailTemplateRepository;
            this.FormRepository = formRepository;
        }

        public void AcceptMessage(Penguin.Messaging.Application.Messages.Startup startup)
        {
            this.RefreshHandlers();
        }

        public void AcceptMessage(Creating<JsonForm> startup)
        {
            this.RefreshHandlers();
        }

        public void AcceptMessage(Updating<JsonForm> startup)
        {
            this.RefreshHandlers();
        }

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

            this.EmailTemplateRepository.TrySendTemplate(Parameters, null, submittedForm.Target.Owner.ToString());
        }

        public IEnumerable<ITemplateDefinition> GetTemplateDefinitions()
        {
            return Cache ?? throw new NullReferenceException("The Cache was not populated before the request");
        }

        private void RefreshHandlers()
        {
            List<ITemplateDefinition> toReturn = new List<ITemplateDefinition>();

            foreach (JsonForm thisForm in this.FormRepository.All.ToList())
            {
                TemplateDefinition toAdd = new TemplateDefinition(thisForm.Name, this.GetType(), thisForm.Guid.ToString());

                toAdd.Children.Add(new FormMacro(FORM_BODY_MACRO));

                foreach (JsonFormField thisField in thisForm.Fields)
                {
                    if (thisField.Name != null)
                    {
                        toAdd.Children.Add(new FormMacro(thisField.Name));
                    }
                }

                toReturn.Add(toAdd);
            }

            Cache = toReturn;
        }
    }
}