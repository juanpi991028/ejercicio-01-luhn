using System;

namespace GestorTareasApp
{
    /// <summary>
    /// System.Text.Json no maneja bien la serialización polimórfica por
    /// defecto (perdería si una tarea es Tarea o TareaConVencimiento).
    /// Este DTO "aplana" ambos tipos en una sola forma, usando el campo
    /// Tipo como discriminador para poder reconstruir el objeto correcto
    /// al cargar el archivo.
    /// </summary>
    public class TareaDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public bool Completada { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Discriminador de tipo: "Simple" o "ConVencimiento"
        public string Tipo { get; set; } = "Simple";

        // Solo se usa cuando Tipo == "ConVencimiento"
        public DateTime? FechaVencimiento { get; set; }
    }
}
