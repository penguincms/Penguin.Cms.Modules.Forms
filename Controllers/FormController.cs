using Microsoft.AspNetCore.Mvc;
using Penguin.Cms.Email.Abstractions.Attributes;
using Penguin.Cms.Forms;
using Penguin.Cms.Forms.Repositories;
using Penguin.Cms.Modules.Admin.Areas.Admin.Controllers;
using Penguin.Cms.Web.Extensions;
using Penguin.Email.Abstractions.Interfaces;
using Penguin.Email.Templating.Abstractions.Extensions;
using Penguin.Email.Templating.Abstractions.Interfaces;
using Penguin.Persistence.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Constructors;
using Penguin.Reflection.Serialization.Objects;
using System;
using System.Collections.Generic;

namespace Penguin.Cms.Modules.Forms.Controllers
{
    public class FormController : Controller, IEmailHandler
    {
        protected ISendTemplates EmailTemplateRepository { get; set; }

        protected FormRepository FormRepository { get; set; }

        protected FormSubmissionRepository FormSubmissionRepository { get; set; }

        public FormController(FormRepository formRepository, FormSubmissionRepository formSubmissionRepository, ISendTemplates emailTemplateRepository = null)
        {
            this.EmailTemplateRepository = emailTemplateRepository;
            this.FormSubmissionRepository = formSubmissionRepository;
            this.FormRepository = formRepository;
        }

        [EmailHandler("Any Form Submission")]
        public ActionResult Submit(string formData, Guid ownerGuid)
        {
            using (IWriteContext context = this.FormSubmissionRepository.WriteContext())
            {
                SubmittedForm thisForm = new SubmittedForm
                {
                    FormData = formData,
                    Owner = ownerGuid
                };

                this.FormSubmissionRepository.AddOrUpdate(thisForm);
            }

            this.EmailTemplateRepository.TrySendTemplate(new Dictionary<string, object>()
            {
                [nameof(formData)] = formData.PrettifyJson(),
                [nameof(ownerGuid)] = ownerGuid
            });

            return this.Content("Submitted");
        }

        public ActionResult ViewById(int Id)
        {
            JsonForm form = this.FormRepository.Find(Id) ?? throw new NullReferenceException($"Form not found with Id {Id}");

            return this.View("ViewJsonForm", form);
        }

        // GET: Form
        public ActionResult ViewByName(string Name)
        {
            Form thisForm = this.FormRepository.GetByName(Name);

            if (thisForm is null)
            {
                return this.Redirect("/Error/NotFound");
            }

            if (thisForm.IsJsonForm)
            {
                return this.View("ViewJsonForm", thisForm);
            }
            else
            {
                MetaConstructor c = AdminController.Constructor;

                MetaObject mo = new MetaObject(thisForm, c);
                mo.Hydrate();
                return this.View("ViewForm", mo);
            }
        }
    }
}