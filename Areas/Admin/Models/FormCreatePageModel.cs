using Penguin.Cms.Forms;
using Penguin.Cms.Web.Modules;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Penguin.Cms.Modules.Forms.Areas.Admin.Models
{
    public class FormCreatePageModel
    {
        public JsonForm? ExistingForm { get; set; }

        [SuppressMessage("Usage", "CA2227:Collection properties should be read only")]
        public ICollection<ViewModule> Modules { get; set; } = new List<ViewModule>();
    }
}