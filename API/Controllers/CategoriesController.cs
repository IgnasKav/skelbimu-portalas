using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Categories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CategoriesController: BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            return await Mediator.Send(new List.Query());
        }
    }
}