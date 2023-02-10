using Microsoft.AspNetCore.Mvc;
using Penguin.Cms.Entities;
using Penguin.Cms.Forms;
using Penguin.Cms.Forms.Repositories;
using Penguin.Cms.Modules.Dynamic.Areas.Admin.Controllers;
using Penguin.Cms.Modules.Forms.Areas.Admin.Models;
using Penguin.Cms.Web.Modules;
using Penguin.Persistence.Abstractions.Interfaces;
using Penguin.Reflection;
using Penguin.Security.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Penguin.Cms.Modules.Forms.Areas.Admin.Controllers
{
    public class FormController : ObjectManagementController<JsonForm>
    {
        protected FormRepository FormRepository { get; set; }

        protected FormSubmissionRepository FormSubmissionRepository { get; set; }

        public class FormPost
        {
            [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
            public string? _Id { get; set; }

            public string? Formhtml { get; set; }
            public int Id => int.TryParse(_Id, out int id) ? id : 0;
        }

        public FormController(FormRepository formRepository, FormSubmissionRepository formSubmissionRepository, IServiceProvider serviceProvider, IUserSession userSession) : base(serviceProvider, userSession)
        {
            FormSubmissionRepository = formSubmissionRepository;
            FormRepository = formRepository;
        }

        public ActionResult BaseEdit(int Id)
        {
            return base.Edit(Id, typeof(JsonForm).FullName);
        }

        public ActionResult Create()
        {
            JsonForm newForm = new();
            FormCreatePageModel model = new()
            {
                ExistingForm = newForm,
                Modules = ComponentService.GetComponents<ViewModule, Entity>(newForm).ToList()
            };

            return View(model);
        }

        public override ActionResult Edit(int? id, string? type)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            JsonForm existingForm = FormRepository.Find(id.Value);

            FormCreatePageModel model = new()
            {
                ExistingForm = existingForm,
                Modules = ComponentService.GetComponents<ViewModule, Entity>(existingForm).ToList()
            };

            return View("Create", model);
        }

        public ActionResult SaveForm([FromBody] FormPost formBody)
        {
            if (formBody is null)
            {
                throw new ArgumentNullException(nameof(formBody));
            }

            JsonForm toSave;

            using (IWriteContext context = FormRepository.WriteContext())
            {
                toSave = formBody.Id != 0
                    ? FormRepository.Find(formBody.Id) ?? throw new NullReferenceException($"Can not find form with Id {formBody.Id}")
                    : new JsonForm();

                toSave.FormData = formBody.Formhtml;

                FormRepository.AddOrUpdate(toSave);
            }

            JsonForm SavedForm = FormRepository.Find(toSave.Guid);

            return new JsonResult(new { Id = SavedForm._Id });
        }

        public ActionResult Submissions(string Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                List<FormSubmissionPageModel> model = FormRepository.All
                                                                    .ToList()
                                                                    .Select(f =>
                                                                        new FormSubmissionPageModel(FormSubmissionRepository.GetByOwner(f.Guid))
                                                                        {
                                                                            Form = f,
                                                                        })
                                                                    .ToList();

                model.AddRange(TypeFactory.GetDerivedTypes(typeof(Form)).Select(t =>
                {
                    return Activator.CreateInstance(t) is Form formInstance
                        ? new FormSubmissionPageModel(FormSubmissionRepository.GetByOwner(formInstance.Guid))
                        {
                            Form = formInstance
                        }
                        : throw new Exception("What the fuck?");
                }));

                return View(model);
            }
            else
            {
                Form thisForm = FormRepository.GetByName(Name);

                FormSubmissionPageModel model = new(FormSubmissionRepository.GetByOwner(thisForm.Guid))
                {
                    Form = thisForm
                };

                return View("FormSubmissions", model);
            }
        }
    }
}