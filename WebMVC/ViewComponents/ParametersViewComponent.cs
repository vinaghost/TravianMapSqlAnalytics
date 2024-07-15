using Microsoft.AspNetCore.Mvc;

namespace WebMVC.ViewComponents
{
    [ViewComponent]
    public abstract class ParametersViewComponent<TParameter>
        : ViewComponent
    {
        public virtual Task<IViewComponentResult> InvokeAsync(TParameter parameter)
        {
            return Task.FromResult<IViewComponentResult>(View(parameter));
        }
    }
}