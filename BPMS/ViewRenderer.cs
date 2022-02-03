
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BPMS
{
    public static class ViewRenderer
    {  
        public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool partial = false) 
        {
            controller.ViewData.Model = model;

            #pragma warning disable CS8602, CS8600
            IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
            ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);
            #pragma warning restore CS8602, CS8600

            if (!viewResult.Success)
            {
                throw new Exception($"View with name: {viewName} could not be found.");
            }

            // using pro zavolani metody Dispose() - uvolneni zdroju; nad objektem StringWriter na konci tohoto bloku
            using (var writer = new StringWriter())
            {
                // sestaveni view kontextu pro jeho nasledne renderovani, vysledek renderovani se uklada do 'writer'
                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                // asynchroni renderovani view
                await viewResult.View.RenderAsync(viewContext);

                // prevod vyrenderovaneho view na string
                return writer.GetStringBuilder().ToString();
            }
        }
    }
}