using System.ComponentModel.DataAnnotations;

namespace WebSqliteApp.Models;

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public class StudentDto
{
    [Required, MaxLength(100)] public string Nombre { get; set; } = string.Empty;   
    [Required, MaxLength(100)] public string Apellido {  get; set; } = string.Empty;
    [Required, MaxLength(100), EmailAddress] public string Email { get; set; } = string.Empty;
    public DateTime? FechaNacimiento { get; set; }
    [Required, MaxLength(100)] public string NroTelefono { get; set; } = string.Empty;

}

public class CourseDto
{
    [Required, MaxLength(200)] public string Nombre { get; set; } = string.Empty;
    [MaxLength(500)] public string? Descripcion { get; set; }
}

public class EnrollmentDto
{
    [Required, Range(1, int.MaxValue)] public int StudentId { get; set; }
    [Required, Range(1, int.MaxValue)] public int CourseId { get; set; }
    public DateTime FechaInscripcion { get; set; }
}

public class AssessmentDto
{
    [Required, Range(1, int.MaxValue)] public int Id { get; set; }

    [Required, Range(1, int.MaxValue)] public int CourseId { get; set; }
    [Required] public DateTime FechaEvaluacion { get; set; }
    [MaxLength(500), Required] public string Nombre { get; set; } = string.Empty;
    [MaxLength(500), Required] public string NombreCurso { get; set; } = string.Empty;
    [MaxLength(500), Required] public string Descripcion { get; set; } = string.Empty;
    [Required, Range(1, 100)] public int Puntaje { get; set; }

}

public class StudentScoreDto
{
    [ Range(1, int.MaxValue)] public int Id { get; set; }
    [Required, Range(1, int.MaxValue)] public int AssessmentId { get; set; }
    [MaxLength(500),] public string NombreAssessment { get; set; } = string.Empty;

    [Required, Range(1, int.MaxValue)] public int EnrollmentId { get; set; }
    [MaxLength(500)] public string NombreStudent { get; set; } = string.Empty;
    [MaxLength(500)] public string NombreCurso { get; set; } = string.Empty;
    [Required] public DateTime FechaEvaluacion { get; set; }
    [Required, Range(1, 100)] public int PuntajeObtenido { get; set; }
    [MaxLength(500), Required] public string Observacinoes { get; set; } = string.Empty;

}