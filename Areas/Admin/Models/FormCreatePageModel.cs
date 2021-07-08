using Penguin.Cms.Forms;
using Penguin.Cms.Web.Modules;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Penguin.Cms.Modules.Forms.Areas.Admin.Models
{
    public class FormCreatePageModel
    {
        public JsonForm? ExistingForm { get; set; }

        public ICollection<ViewModule> Modules { get; set; } = new List<ViewModule>();
    }
}