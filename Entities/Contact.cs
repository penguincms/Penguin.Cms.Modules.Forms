using Penguin.Cms.Forms;
using Penguin.Persistence.Abstractions.Attributes.Rendering;

namespace Penguin.Cms.Modules.Forms.Entities
{
    [Display(Name = "Contact Us")]
    public class Contact : Form
    {
        [HtmlRender(HtmlRenderAttribute.RenderingType.email)]
        public string ContactEmail { get; set; } = string.Empty;

        [Display(Name = "Your Name")]
        [HtmlRender(HtmlRenderAttribute.RenderingType.text)]
        public string ContactName { get; set; } = string.Empty;

        [Display(Name = "Message")]
        [HtmlRender(HtmlRenderAttribute.RenderingType.textarea)]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Subject")]
        [HtmlRender(HtmlRenderAttribute.RenderingType.text)]
        public string Subject { get; set; } = string.Empty;
    }
}