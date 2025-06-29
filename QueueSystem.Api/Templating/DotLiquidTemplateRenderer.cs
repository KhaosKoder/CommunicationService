using DotLiquid;
using Microsoft.Extensions.Options;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using QueueSystem.Api.Configuration;

namespace QueueSystem.Api.Templating
{
    public interface ITemplateRenderer
    {
        Task<string> RenderAsync(string templateName, object model);
    }

    public class DotLiquidTemplateRenderer : ITemplateRenderer
    {
        private readonly string _templateFolder;
        public DotLiquidTemplateRenderer(IOptions<DotLiquidSettings> options)
        {
            _templateFolder = options.Value.TemplateFolder;
        }

        public async Task<string> RenderAsync(string templateName, object model)
        {
            var filePath = Path.Combine(_templateFolder, templateName + ".liquid");
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Template '{templateName}' not found.");
            var templateText = await File.ReadAllTextAsync(filePath);
            var template = Template.Parse(templateText);
            var hash = Hash.FromAnonymousObject(model);
            return template.Render(hash);
        }
    }
}
