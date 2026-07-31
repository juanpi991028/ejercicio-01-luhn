using System;

namespace GestorTareasApp
{
    /// <summary>
    /// Tarea que además tiene una fecha límite de vencimiento.
    /// Hereda de Tarea y sobreescribe MostrarInfo() para agregar
    /// la información de días restantes.
    /// </summary>
    public class TareaConVencimiento : Tarea
    {
        public DateTime FechaVencimiento { get; set; }

        /// <summary>
        /// Propiedad calculada en tiempo real: cuántos días faltan
        /// para el vencimiento (puede ser negativa si ya venció).
        /// </summary>
        public int DiasRestantes => (FechaVencimiento.Date - DateTime.Now.Date).Days;

        public TareaConVencimiento(string titulo, string descripcion, Prioridad prioridad,
                                    string categoria, DateTime fechaVencimiento)
            : base(titulo, descripcion, prioridad, categoria)
        {
            FechaVencimiento = fechaVencimiento;
        }

        public override void MostrarInfo()
        {
            base.MostrarInfo();

            if (DiasRestantes < 0)
            {
                Console.WriteLine($"      ⚠ Venció hace {Math.Abs(DiasRestantes)} día(s) (vencimiento: {FechaVencimiento:dd/MM/yyyy})");
            }
            else if (DiasRestantes == 0)
            {
                Console.WriteLine($"      ⏰ Vence HOY ({FechaVencimiento:dd/MM/yyyy})");
            }
            else
            {
                Console.WriteLine($"      📅 Vence: {FechaVencimiento:dd/MM/yyyy} (quedan {DiasRestantes} día(s))");
            }
        }
    }
}
