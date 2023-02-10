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

        public Form(FormRepository formRepository, ISendTemplates? emailTemplateRepository = null)
        {
            EmailTemplateRepository = emailTemplateRepository;
            FormRepository = formRepository;
        }

        public void AcceptMessage(Penguin.Messaging.Application.Messages.Startup message)
        {
            RefreshHandlers();
        }

        public void AcceptMessage(Creating<JsonForm> message)
        {
            RefreshHandlers();
        }

        public void AcceptMessage(Updating<JsonForm> message)
        {
            RefreshHandlers();
        }

        public void AcceptMessage(Creating<SubmittedForm> message)
        {
            if (message is null)
            {
                throw new System.ArgumentNullException(nameof(message));
            }

            Dictionary<string, object> Parameters = new();

            foreach (string thisField in message.Target.GetKeys())
            {
                Parameters.Add(thisField.ToVariableName(), message.Target.GetValue(thisField));
            }

            Parameters.Add(FORM_BODY_MACRO.ToVariableName(), message.Target.FormData.PrettifyJson());

            EmailTemplateRepository.TrySendTemplate(Parameters, null, message.Target.Owner.ToString());
        }

        public IEnumerable<ITemplateDefinition> GetTemplateDefinitions()
        {
            return Cache ?? throw new NullReferenceException("The Cache was not populated before the request");
        }

        private void RefreshHandlers()
        {
            List<ITemplateDefinition> toReturn = new();

            foreach (JsonForm thisForm in FormRepository.All.ToList())
            {
                TemplateDefinition toAdd = new(thisForm.Name, GetType(), thisForm.Guid.ToString());

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