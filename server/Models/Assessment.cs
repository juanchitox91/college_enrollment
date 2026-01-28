using System.Diagnostics.Contracts;

namespace WebSqliteApp.Models
{
    public class Assessment
    {
        public int Id { get; set; }
        public int CourseId { get; set; } 
        public Course Course { get; set; } 
        public string Nombre { get; set; } = string.Empty;
        public int Puntaje { get; set; } 
        public DateTime FechaEvaluacion { get; set; }
        public string Descripcion { get; set; } = string.Empty;


    }
}
