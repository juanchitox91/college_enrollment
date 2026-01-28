using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSqliteApp.Models;

namespace WebSqliteApp.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]

public class StudentScoresController : ControllerBase
{
    private readonly AppDb _db;
    public StudentScoresController(AppDb db) { _db = db; }

    // GET: Enrollments
    //consultar un objeto por ID
    [HttpGet("{id:int}")]  // localhost:8080/api/v1/enrollments/1234
    [ProducesResponseType(typeof(Enrollment), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetById(int id) 
    {
        try
        {
            var enrollment = _db.StudentScores.SingleOrDefault(x => x.Id == id);
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

            var query = _db.StudentScores.AsQueryable();
            var lista = new List<StudentScoreDto>();

            if (!string.IsNullOrEmpty(filtro))
            {
                var like = $"%{filtro}%";
                var like2 = "%" + like + "%";

                query = query.Where(s =>
                EF.Functions.Like(s.AssessmentId.ToString(), like) ||
                EF.Functions.Like(s.EnrollmentId.ToString(), like));
            }

            lista = query.Select(a => new StudentScoreDto
            {
                Id = a.Id,
                EnrollmentId = a.EnrollmentId,
                NombreCurso = a.Enrollment.Course.Nombre,
                NombreAssessment = a.Assessment.Nombre,
                NombreStudent = a.Enrollment.Student.Nombre,
                PuntajeObtenido = a.PuntajeObtenido,
                Observacinoes = a.Observaciones,
                FechaEvaluacion = a.Assessment.FechaEvaluacion
            }).ToList();

            var total = lista.Count();
            var items = lista.OrderBy(s => s.NombreAssessment).Skip((page - 1) * pageSize).Take(pageSize).ToList();
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
    public IActionResult Create([FromBody] StudentScoreDto dto)
    {
        var s = new StudentScore
        {
            AssessmentId = dto.AssessmentId,
            EnrollmentId  = dto.EnrollmentId,
            PuntajeObtenido = dto.PuntajeObtenido,
            Observaciones = dto.Observacinoes
        };
        _db.StudentScores.Add(s);
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
    public IActionResult Update(int id, [FromBody] StudentScoreDto dto)
    {
        var s = _db.StudentScores.Find(id);
        if (s is null)
        {
            return NotFound();
        }

        s.PuntajeObtenido = dto.PuntajeObtenido;
        s.Observaciones = dto.Observacinoes;


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
        var s = _db.StudentScores.Find(id);
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