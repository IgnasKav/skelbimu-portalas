using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Categories;
using Domain;
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

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            return Ok(await Mediator.Send(new Create.Command{Category = category}));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, Category category)
        {
            category.Id = id;
            return Ok(await Mediator.Send(new Edit.Command {Category = category}));
        }
    }
}