﻿using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context
            )
        {
            try
            {
                var categories = await context.Categories.ToListAsync();

                return Ok(new ResultViewModel<List<Category>>(categories));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("E05X01 - Falha interna no servidor"));
            }
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context
    )
        {
            try
            {
                var category = await context
                    .Categories
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                    return NotFound(new ResultViewModel<Category>("Conteúdo não encontrado"));

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Category>("E05X02 - Falha interna no servidor"));
            }
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync(
            [FromBody] EditorCategoryViewModel model,
            [FromServices] BlogDataContext context
    )
        {

            if(!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));    

            try
            {
                var category = new Category
                {
                    Id = 0,
                    Name = model.Name,
                    Slug = model.Slug.ToLower(),
                };

                await context.Categories.AddAsync(category);

                await context.SaveChangesAsync();

                return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("E05EX01 - Falha ao criar categoria"));
            }
            catch 
            {
                return StatusCode(500, new ResultViewModel<Category>("E05X03 - Falha interna no servidor"));
            }
        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync(
            [FromRoute] int id,
            [FromBody] EditorCategoryViewModel model,
            [FromServices] BlogDataContext context
)
        {
           // if (!ModelState.IsValid)
             //   return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

            try
            {
                var category = await context
                .Categories
                .FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                    return NotFound(new ResultViewModel<Category>("Conteúdo não encontrado"));

                category.Name = model.Name;
                category.Slug = model.Slug.ToLower();

                context.Categories.Update(category);

                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("E05EX02 - Falha ao alterar categoria"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Category>("E05X04 - Falha interna no servidor"));
            }
        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context
)
        {
            try
            {
                var category = await context
                    .Categories
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                    return NotFound(new ResultViewModel<Category>("Conteúdo não encontrado"));

                context.Categories.Remove(category);

                await context.SaveChangesAsync();

                return Ok(category);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("E05EX03 - Falha ao deletar categoria"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Category>("E05X05 - Falha interna no servidor"));
            }
        }
    }
}
