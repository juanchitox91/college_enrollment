using System.Diagnostics.Contracts;

namespace WebSqliteApp.Models
{
    public class StudentScore
    {
        public int Id { get; set; }
        public int AssessmentId { get; set; }
        public Assessment Assessment { get; set; }
        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; }
        public int PuntajeObtenido { get; set; } 
        public string Observaciones { get; set; } = string.Empty;


    }
}
