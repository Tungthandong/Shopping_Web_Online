using Microsoft.AspNetCore.Mvc;
using Shopping_Web.Services;

namespace Shopping_Web.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly ICategoryService _service;

        public CategoryViewComponent(ICategoryService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync() => View(_service.getCategories());
    }
}
