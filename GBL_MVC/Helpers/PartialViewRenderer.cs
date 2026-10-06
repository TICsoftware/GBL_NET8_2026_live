
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.IO;
using Microsoft.AspNetCore.Mvc.Razor;

public class PartialViewRenderer
{
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public PartialViewRenderer(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider)
    {
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> RenderPartialToStringAsync<TModel>(
        Controller controller,
        string viewName,
        TModel model)
    {
        controller.ViewData.Model = model;

        var viewResult = _viewEngine.FindView(
            controller.ControllerContext,
            viewName,
            false);

        if (!viewResult.Success)
        {
            throw new InvalidOperationException(
                $"Partial view '{viewName}' was not found.");
        }

        await using var writer = new StringWriter();

        var viewData = new ViewDataDictionary<TModel>(
            new EmptyModelMetadataProvider(),
            controller.ModelState)
        {
            Model = model
        };

        foreach (var item in controller.ViewData)
        {
            viewData[item.Key] = item.Value;
        }

        var viewContext = new ViewContext(
            controller.ControllerContext,
            viewResult.View,
            viewData,
            new TempDataDictionary(
                controller.HttpContext,
                _tempDataProvider),
            writer,
            new HtmlHelperOptions());

        await viewResult.View.RenderAsync(viewContext);

        return writer.ToString();
    }
}
