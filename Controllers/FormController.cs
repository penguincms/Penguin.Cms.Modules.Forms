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

        public FormController(FormRepository formRepository, FormSubmissionRepository formSubmissionRepository, ISendTemplates? emailTemplateRepository = null)
        {
            EmailTemplateRepository = emailTemplateRepository;
            FormSubmissionRepository = formSubmissionRepository;
            FormRepository = formRepository;
        }

        [EmailHandler("Any Form Submission")]
        public ActionResult Submit(string formData, Guid ownerGuid)
        {
            using (IWriteContext context = FormSubmissionRepository.WriteContext())
            {
                SubmittedForm thisForm = new()
                {
                    FormData = formData,
                    Owner = ownerGuid
                };

                FormSubmissionRepository.AddOrUpdate(thisForm);
            }

            EmailTemplateRepository.TrySendTemplate(new Dictionary<string, object>()
            {
                [nameof(formData)] = formData.PrettifyJson(),
                [nameof(ownerGuid)] = ownerGuid
            });

            return Content("Submitted");
        }

        public ActionResult ViewById(int Id)
        {
            JsonForm form = FormRepository.Find(Id) ?? throw new NullReferenceException($"Form not found with Id {Id}");

            return View("ViewJsonForm", form);
        }

        // GET: Form
        public ActionResult ViewByName(string Name)
        {
            Form thisForm = FormRepository.GetByName(Name);

            if (thisForm is null)
            {
                return Redirect("/Error/NotFound");
            }

            if (thisForm.IsJsonForm)
            {
                return View("ViewJsonForm", thisForm);
            }
            else
            {
                MetaConstructor c = AdminController.Constructor;

                MetaObject mo = new(thisForm, c);
                mo.Hydrate();
                return View("ViewForm", mo);
            }
        }
    }
}