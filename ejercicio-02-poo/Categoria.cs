namespace GestorTareasApp
{
    /// <summary>
    /// Representa una categoría que agrupa tareas (ej: "Estudio", "Trabajo").
    /// </summary>
    public class Categoria
    {
        public string Nombre { get; set; }
        public string Color { get; set; }
        public string Descripcion { get; set; }

        public Categoria(string nombre, string color, string descripcion)
        {
            Nombre = nombre;
            Color = color;
            Descripcion = descripcion;
        }

        public override string ToString()
        {
            return $"{Nombre} ({Color}) - {Descripcion}";
        }
    }
}
