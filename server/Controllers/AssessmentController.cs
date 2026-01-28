using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebSqliteApp.Models;

namespace WebSqliteApp.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]

public class AssessmentsController : ControllerBase
{
    private readonly AppDb _db;
    public AssessmentsController(AppDb db) { _db = db; }

    // GET: Enrollments
    //consultar un objeto por ID
    [HttpGet("{id:int}")]  // localhost:8080/api/v1/enrollments/1234
    [ProducesResponseType(typeof(Enrollment), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetById(int id) 
    {
        try
        {
            var enrollment = _db.Assessments.SingleOrDefault(x => x.Id == id);
            return Ok(enrollment);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }

    }

    //Obtener listado de objetos
    [HttpGet]
    [ProducesResponseType(typeof(object), 200)] //localhost:8080/api/v1/enrollments?filtro=Juan&page=0&pageSize=0
    [ProducesResponseType(400)]
    public IActionResult GetAll([FromQuery] string? filtro, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page > 0 ? page : 1;
            pageSize = pageSize > 0 ? pageSize : 10;

            var query = _db.Assessments.AsQueryable();
            var lista = new List<AssessmentDto>();


            if (!string.IsNullOrEmpty(filtro))
            {
                var like = $"%{filtro}%";
                var like2 = "%" + like + "%";

                query = query.Where(s =>
                EF.Functions.Like(s.CourseId.ToString(), like));
            }
            lista = query.Select(a => new AssessmentDto
             {
                 Id = a.Id,
                 CourseId = a.CourseId,
                 NombreCurso = a.Course.Nombre,
                 Nombre = a.Nombre,
                 Puntaje = a.Puntaje,
                 FechaEvaluacion = a.FechaEvaluacion,
                 Descripcion = a.Descripcion
             }).ToList();

            var total = lista.Count();
            var items = lista.OrderBy(s => s.Nombre).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(new { total, page, pageSize, items });

        }
        catch
        {
            return BadRequest();
        }
    }

    //Crear un objeto
    [HttpPost]
    [ProducesResponseType(typeof(Enrollment), 200)]
    [ProducesResponseType(400)]
    public IActionResult Create([FromBody] AssessmentDto dto)
    {
        var s = new Assessment
        {
            CourseId = dto.CourseId,
            Nombre = dto.Nombre,
            Puntaje = dto.Puntaje,
            FechaEvaluacion = dto.FechaEvaluacion,
            Descripcion = dto.Descripcion
        };
        _db.Assessments.Add(s);
        try
        {
            _db.SaveChanges();
            return Ok(s);
        }
        catch
        {
            return BadRequest("Hubo un error al guardar los datos.");
        }
    }

    //Editar un objeto
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Enrollment), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult Update(int id, [FromBody] AssessmentDto dto)
    {
        var s = _db.Assessments.Find(id);
        if (s is null)
        {
            return NotFound();
        }

        s.CourseId = dto.CourseId;
        s.Nombre = dto.Nombre;
        s.Puntaje = dto.Puntaje;
        s.FechaEvaluacion = dto.FechaEvaluacion;
        s.Descripcion = dto.Descripcion;

        try
        {
            _db.SaveChanges();
            return Ok(s);
        }
        catch
        {
            return BadRequest("Hubo un error al intentar actualizar los datos");
        }
    }

    //Eliminar un objeto
    [HttpDelete("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult Delete(int id)
    {
        var s = _db.Assessments.Find(id);
        if (s is null)
        {
            return NotFound();
        }

        try
        {
            _db.Remove(s);
            _db.SaveChanges();
            return Ok(new { ok = true });
        }
        catch
        {
            return BadRequest("Ocurrio algun error");
        }
    }

}