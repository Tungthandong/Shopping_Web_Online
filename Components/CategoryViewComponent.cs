using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models;
using Shopping_Web.Services;

namespace Shopping_Web.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        ICategoryService _service;
        public CategoryViewComponent(ICategoryService service)
        {
            _service = service;
        }
        public async Task<IViewComponentResult> InvokeAsync() => View(_service.getCategories());
    }
}