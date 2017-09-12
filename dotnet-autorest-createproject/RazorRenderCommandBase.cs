using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace ESW.autorest.createProject
{
    public abstract class RazorRenderCommandBase<T> where T:class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IRazorViewEngine _viewEngine;

        public RazorRenderCommandBase(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public abstract void Render(T model, string outputFile);
        
        public void RenderViewToString(string outputFile, T viewModel, string viewPath)
        {
            var actionContext = GetActionContext();

            var view = FindView(actionContext, viewPath);

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary<T>(
                        new EmptyModelMetadataProvider(),
                        new ModelStateDictionary())
                    {
                        Model = viewModel
                    },
                    new TempDataDictionary(
                        actionContext.HttpContext,
                        _tempDataProvider),
                    output,
                    new HtmlHelperOptions());

                view.RenderAsync(viewContext).GetAwaiter().GetResult();
                Directory.CreateDirectory(Directory.GetParent(outputFile).FullName);
                File.WriteAllText(outputFile, output.ToString());
            }

            ActionContext GetActionContext()
            {
                var httpContext = new DefaultHttpContext
                {
                    RequestServices = _serviceProvider
                };

                return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            }
        }

        private IView FindView(ActionContext actionContext, string viewName)

        {
            var getViewResult = _viewEngine.GetView(null, viewName, true);

            if (getViewResult.Success)
                return getViewResult.View;

            var findViewResult = _viewEngine.FindView(actionContext, viewName, true);

            if (findViewResult.Success)
                return findViewResult.View;

            var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);

            var errorMessage = string.Join(
                Environment.NewLine,
                new[] {$"Unable to find view '{viewName}'. The following locations were searched:"}.Concat(
                    searchedLocations));

            throw new InvalidOperationException(errorMessage);
        }
    }
}