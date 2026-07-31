using System;

namespace GestorTareasApp
{
    /// <summary>
    /// Clase base que representa una tarea simple (sin fecha de vencimiento).
    /// </summary>
    public class Tarea : IExportable
    {
        // Contador estático para generar IDs autoincrementales.
        private static int contadorId = 0;

        // Propiedades con encapsulamiento real: no hay campos públicos sueltos.
        public int Id { get; internal set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public Prioridad Prioridad { get; set; }
        public string Categoria { get; set; }
        public bool Completada { get; private set; }
        public DateTime FechaCreacion { get; internal set; }

        public Tarea(string titulo, string descripcion, Prioridad prioridad, string categoria)
        {
            Id = ++contadorId;
            Titulo = titulo;
            Descripcion = descripcion;
            Prioridad = prioridad;
            Categoria = categoria;
            Completada = false;
            FechaCreacion = DateTime.Now;
        }

        /// <summary>
        /// Marca la tarea como completada. Se expone como método (no como
        /// setter público) para mantener el encapsulamiento del estado.
        /// </summary>
        public void MarcarCompletada()
        {
            Completada = true;
        }

        /// <summary>
        /// Muestra la información de la tarea en consola.
        /// Es "virtual" para que las clases derivadas puedan
        /// extender el comportamiento (polimorfismo).
        /// </summary>
        public virtual void MostrarInfo()
        {
            Console.WriteLine($"[{Id}] {Titulo} | Prioridad: {Prioridad} | Categoría: {Categoria} | " +
                               $"{(Completada ? "✅ Completada" : "⏳ Pendiente")} | Creada: {FechaCreacion:dd/MM/yyyy}");
            if (!string.IsNullOrWhiteSpace(Descripcion))
            {
                Console.WriteLine($"      Descripción: {Descripcion}");
            }
        }

        /// <summary>
        /// Implementación de IExportable: representación compacta en texto plano.
        /// </summary>
        public string Exportar()
        {
            return $"{Id}|{Titulo}|{Prioridad}|{Completada}";
        }

        /// <summary>
        /// Usado internamente por GestorTareas al cargar desde JSON,
        /// para asegurar que el próximo Id autoincremental no colisione
        /// con los ids ya guardados.
        /// </summary>
        internal static void ActualizarContador(int idUsado)
        {
            if (idUsado > contadorId)
            {
                contadorId = idUsado;
            }
        }

        /// <summary>
        /// Usado internamente por GestorTareas al reconstruir una tarea
        /// desde el archivo JSON, para restaurar su estado "Completada".
        /// </summary>
        internal void RestaurarCompletada(bool completada)
        {
            Completada = completada;
        }
    }
}
