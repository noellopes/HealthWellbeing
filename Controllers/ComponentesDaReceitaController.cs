// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using HealthWellbeing.Models;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;

// namespace HealthWellbeing.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class ComponentesDaReceitaController : ControllerBase
//     {
//         private readonly HealthWellbeingContext _db;

//         public ComponentesDaReceitaController(HealthWellbeingContext db)
//         {
//             _db = db;
//         }


//         [HttpGet]
//         [ProducesResponseType(typeof(IEnumerable<ComponentesDaReceita>), 200)]
//         public async Task<IActionResult> Get([FromQuery] int? receitaId)
//         {
//             IQueryable<ComponentesDaReceita> query = _db.Set<ComponentesDaReceita>().AsNoTracking();

//             if (receitaId.HasValue && receitaId.Value > 0)
//             {
//                 query = query.Where(c => c.ReceitaId == receitaId.Value);
//             }

//             var itens = await query
//                 .OrderBy(c => c.Nome)
//                 .ToListAsync();

//             return Ok(itens);
//         }


//         [HttpGet("{id:int}")]
//         [ProducesResponseType(typeof(ComponentesDaReceita), 200)]
//         [ProducesResponseType(404)]
//         public async Task<IActionResult> GetById(int id)
//         {
//             var item = await _db.Set<ComponentesDaReceita>()
//                                 .AsNoTracking()
//                                 .FirstOrDefaultAsync(c => c.ComponenteId == id);

//             if (item == null) return NotFound();

//             return Ok(item);
//         }


//         [HttpPost]
//         [ProducesResponseType(typeof(ComponentesDaReceita), 201)]
//         [ProducesResponseType(400)]
//         public async Task<IActionResult> Create([FromBody] ComponentesDaReceita model)
//         {
//             if (!ModelState.IsValid) return ValidationProblem(ModelState);

//             _db.Add(model);
//             await _db.SaveChangesAsync();

//             return CreatedAtAction(nameof(GetById), new { id = model.ComponenteId }, model);
//         }


//         [HttpPut("{id:int}")]
//         [ProducesResponseType(204)]
//         [ProducesResponseType(400)]
//         [ProducesResponseType(404)]
//         public async Task<IActionResult> Update(int id, [FromBody] ComponentesDaReceita model)
//         {
//             if (id != model.ComponenteId)
//                 return BadRequest("ID do caminho difere do corpo da requisição.");

//             if (!ModelState.IsValid)
//                 return ValidationProblem(ModelState);

//             var exists = await _db.Set<ComponentesDaReceita>()
//                                   .AnyAsync(c => c.ComponenteId == id);
//             if (!exists) return NotFound();

//             _db.Entry(model).State = EntityState.Modified;

//             try
//             {
//                 await _db.SaveChangesAsync();
//             }
//             catch (DbUpdateConcurrencyException)
//             {
//                 var stillExists = await _db.Set<ComponentesDaReceita>()
//                                            .AnyAsync(c => c.ComponenteId == id);
//                 if (!stillExists) return NotFound();
//                 throw;
//             }

//             return NoContent();
//         }


//         [HttpDelete("{id:int}")]
//         [ProducesResponseType(204)]
//         [ProducesResponseType(404)]
//         public async Task<IActionResult> Delete(int id)
//         {
//             var item = await _db.Set<ComponentesDaReceita>()
//                                 .FirstOrDefaultAsync(c => c.ComponenteId == id);
//             if (item == null) return NotFound();

//             _db.Remove(item);
//             await _db.SaveChangesAsync();

//             return NoContent();
//         }
//     }
// }
