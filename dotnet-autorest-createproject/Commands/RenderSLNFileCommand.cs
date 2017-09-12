using System;
using System.IO;
using ESW.autorest.createProject.Models;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ESW.autorest.createProject.Commands
{
    // ReSharper disable once InconsistentNaming
    public class RenderSLNFileCommand : RazorRenderCommandBase<SLNFileViewModel>
    {
        public RenderSLNFileCommand(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider):base(viewEngine, tempDataProvider, serviceProvider)
        {
        }

        public override void Render(SLNFileViewModel model, string outputFile)
        {
            RenderViewToString(outputFile, model, "Views\\SLNFile.cshtml");
        }
    }
}
