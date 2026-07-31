using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GestorTareasApp
{
    /// <summary>
    /// Administra la colección de tareas: alta, baja, filtros y
    /// persistencia en un archivo JSON.
    /// </summary>
    public class GestorTareas
    {
        private List<Tarea> tareas = new List<Tarea>();

        public void Agregar(Tarea tarea)
        {
            tareas.Add(tarea);
        }

        public bool Completar(int id)
        {
            Tarea? tarea = tareas.FirstOrDefault(t => t.Id == id);
            if (tarea == null)
            {
                return false;
            }

            tarea.MarcarCompletada();
            return true;
        }

        public List<Tarea> ListarTodas()
        {
            return tareas;
        }

        public List<Tarea> ListarPorCategoria(string categoria)
        {
            return tareas
                .Where(t => t.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Tarea> ListarPorPrioridad(Prioridad prioridad)
        {
            return tareas.Where(t => t.Prioridad == prioridad).ToList();
        }

        /// <summary>
        /// Devuelve las tareas con vencimiento cuya fecha ya pasó y
        /// que todavía no han sido completadas.
        /// </summary>
        public List<Tarea> ObtenerVencidas()
        {
            return tareas
                .OfType<TareaConVencimiento>()
                .Where(t => !t.Completada && DateTime.Compare(t.FechaVencimiento, DateTime.Now) < 0)
                .Cast<Tarea>()
                .ToList();
        }

        public bool Eliminar(int id)
        {
            Tarea? tarea = tareas.FirstOrDefault(t => t.Id == id);
            if (tarea == null)
            {
                return false;
            }

            tareas.Remove(tarea);
            return true;
        }

        // ------------------------------------------------------------------
        //  PERSISTENCIA JSON
        // ------------------------------------------------------------------

        public void GuardarEnJSON(string archivo)
        {
            List<TareaDTO> dtos = tareas.Select(MapearADTO).ToList();

            JsonSerializerOptions opciones = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(dtos, opciones);
            File.WriteAllText(archivo, json);
        }

        /// <summary>
        /// Carga las tareas desde un archivo JSON. Si el archivo no existe
        /// o está corrupto, devuelve una lista vacía en lugar de lanzar
        /// una excepción hacia el programa principal.
        /// </summary>
        public List<Tarea> CargarDeJSON(string archivo)
        {
            if (!File.Exists(archivo))
            {
                return new List<Tarea>();
            }

            try
            {
                string json = File.ReadAllText(archivo);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<Tarea>();
                }

                List<TareaDTO>? dtos = JsonSerializer.Deserialize<List<TareaDTO>>(json);

                if (dtos == null)
                {
                    return new List<Tarea>();
                }

                List<Tarea> tareasCargadas = dtos.Select(MapearDesdeDTO).ToList();
                tareas = tareasCargadas;
                return tareas;
            }
            catch (JsonException)
            {
                Console.WriteLine("⚠ El archivo tareas.json está corrupto o mal formado. Se iniciará con una lista vacía.");
                return new List<Tarea>();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"⚠ No se pudo leer el archivo: {ex.Message}. Se iniciará con una lista vacía.");
                return new List<Tarea>();
            }
        }

        private TareaDTO MapearADTO(Tarea tarea)
        {
            TareaDTO dto = new TareaDTO
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                Descripcion = tarea.Descripcion,
                Prioridad = tarea.Prioridad.ToString(),
                Categoria = tarea.Categoria,
                Completada = tarea.Completada,
                FechaCreacion = tarea.FechaCreacion
            };

            if (tarea is TareaConVencimiento conVencimiento)
            {
                dto.Tipo = "ConVencimiento";
                dto.FechaVencimiento = conVencimiento.FechaVencimiento;
            }
            else
            {
                dto.Tipo = "Simple";
            }

            return dto;
        }

        private Tarea MapearDesdeDTO(TareaDTO dto)
        {
            Prioridad prioridad = Enum.TryParse(dto.Prioridad, out Prioridad p) ? p : Prioridad.Media;

            Tarea tarea;

            if (dto.Tipo == "ConVencimiento" && dto.FechaVencimiento.HasValue)
            {
                tarea = new TareaConVencimiento(dto.Titulo, dto.Descripcion, prioridad, dto.Categoria, dto.FechaVencimiento.Value);
            }
            else
            {
                tarea = new Tarea(dto.Titulo, dto.Descripcion, prioridad, dto.Categoria);
            }

            // Restaurar el estado original (Id, fecha de creación y completada)
            // en lugar de dejar los valores nuevos generados por el constructor.
            tarea.Id = dto.Id;
            tarea.FechaCreacion = dto.FechaCreacion;
            tarea.RestaurarCompletada(dto.Completada);

            Tarea.ActualizarContador(dto.Id);

            return tarea;
        }
    }
}
