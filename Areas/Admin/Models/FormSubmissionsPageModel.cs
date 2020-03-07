using Penguin.Cms.Forms;
using System.Collections.Generic;

namespace Penguin.Cms.Modules.Forms.Areas.Admin.Models
{
    public class FormSubmissionPageModel
    {
        public Form? Form { get; set; }

        public List<SubmittedForm> Submissions { get; }

        public FormSubmissionPageModel(List<SubmittedForm> submissions)
        {
            this.Submissions = submissions;
        }
    }
}