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
            public int Id => int.TryParse(this._Id, out int id) ? id : 0;
        }

        public FormController(FormRepository formRepository, FormSubmissionRepository formSubmissionRepository, IServiceProvider serviceProvider, IUserSession userSession) : base(serviceProvider, userSession)
        {
            this.FormSubmissionRepository = formSubmissionRepository;
            this.FormRepository = formRepository;
        }

        public ActionResult BaseEdit(int Id) => base.Edit(Id, typeof(JsonForm).FullName);

        public ActionResult Create()
        {
            JsonForm newForm = new JsonForm();
            FormCreatePageModel model = new FormCreatePageModel
            {
                ExistingForm = newForm,
                Modules = this.ComponentService.GetComponents<ViewModule, Entity>(newForm).ToList()
            };

            return this.View(model);
        }

        public override ActionResult Edit(int? id, string? type)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            JsonForm existingForm = this.FormRepository.Find(id.Value);

            FormCreatePageModel model = new FormCreatePageModel
            {
                ExistingForm = existingForm,
                Modules = this.ComponentService.GetComponents<ViewModule, Entity>(existingForm).ToList()
            };

            return this.View("Create", model);
        }

        public ActionResult SaveForm([FromBody] FormPost formBody)
        {
            if (formBody is null)
            {
                throw new ArgumentNullException(nameof(formBody));
            }

            JsonForm toSave;

            using (IWriteContext context = this.FormRepository.WriteContext())
            {
                if (formBody.Id != 0)
                {
                    toSave = this.FormRepository.Find(formBody.Id) ?? throw new NullReferenceException($"Can not find form with Id {formBody.Id}");
                }
                else
                {
                    toSave = new JsonForm();
                }

                toSave.FormData = formBody.Formhtml;

                this.FormRepository.AddOrUpdate(toSave);
            }

            JsonForm SavedForm = this.FormRepository.Find(toSave.Guid);

            return new JsonResult(new { Id = SavedForm._Id });
        }

        public ActionResult Submissions(string Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                List<FormSubmissionPageModel> model = this.FormRepository.All
                                                                    .ToList()
                                                                    .Select(f =>
                                                                        new FormSubmissionPageModel(this.FormSubmissionRepository.GetByOwner(f.Guid))
                                                                        {
                                                                            Form = f,
                                                                        })
                                                                    .ToList();

                model.AddRange(TypeFactory.GetDerivedTypes(typeof(Form)).Select(t =>
                {
                    if (Activator.CreateInstance(t) is Form formInstance)
                    {
                        return new FormSubmissionPageModel(this.FormSubmissionRepository.GetByOwner(formInstance.Guid))
                        {
                            Form = formInstance
                        };
                    }
                    else
                    {
                        throw new Exception("What the fuck?");
                    }
                }));

                return this.View(model);
            }
            else
            {
                Form thisForm = this.FormRepository.GetByName(Name);

                FormSubmissionPageModel model = new FormSubmissionPageModel(this.FormSubmissionRepository.GetByOwner(thisForm.Guid))
                {
                    Form = thisForm
                };

                return this.View("FormSubmissions", model);
            }
        }
    }
}