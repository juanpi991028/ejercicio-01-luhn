using System;
using System.Collections.Generic;

namespace GestorTareasApp
{
    class Program
    {
        const string ArchivoTareas = "tareas.json";
        static GestorTareas gestor = new GestorTareas();

        static void Main(string[] args)
        {
            // Cargar datos existentes al iniciar (si el archivo existe).
            gestor.CargarDeJSON(ArchivoTareas);
            Console.WriteLine($"Se cargaron {gestor.ListarTodas().Count} tarea(s) desde {ArchivoTareas}.");

            int opcion;

            do
            {
                MostrarMenu();
                opcion = LeerEntero("Seleccione una opción: ");

                switch (opcion)
                {
                    case 1: OpcionAgregarTarea(); break;
                    case 2: OpcionListarTodas(); break;
                    case 3: OpcionListarPorCategoria(); break;
                    case 4: OpcionListarPorPrioridad(); break;
                    case 5: OpcionMarcarCompletada(); break;
                    case 6: OpcionMostrarVencidas(); break;
                    case 7: OpcionEliminarTarea(); break;
                    case 8: OpcionExportarJSON(); break;
                    case 9:
                        gestor.GuardarEnJSON(ArchivoTareas);
                        Console.WriteLine($"\nDatos guardados en {ArchivoTareas}. ¡Hasta luego!");
                        break;
                    default:
                        Console.WriteLine("\nOpción no válida.");
                        break;
                }

                if (opcion != 9)
                {
                    Console.WriteLine("\nPresione ENTER para continuar...");
                    Console.ReadLine();
                }

            } while (opcion != 9);
        }

        static void MostrarMenu()
        {
            Console.Clear();
            Console.WriteLine("=== GESTOR DE TAREAS ===");
            Console.WriteLine("1. Agregar tarea");
            Console.WriteLine("2. Listar todas");
            Console.WriteLine("3. Listar por categoría");
            Console.WriteLine("4. Listar por prioridad");
            Console.WriteLine("5. Marcar como completada");
            Console.WriteLine("6. Mostrar tareas vencidas");
            Console.WriteLine("7. Eliminar tarea");
            Console.WriteLine("8. Exportar a JSON");
            Console.WriteLine("9. Salir");
            Console.WriteLine();
        }

        // ------------------------------------------------------------------
        //  OPCIÓN 1: AGREGAR TAREA
        // ------------------------------------------------------------------

        static void OpcionAgregarTarea()
        {
            Console.WriteLine("\n--- Nueva tarea ---");
            Console.WriteLine("¿Qué tipo de tarea es?");
            Console.WriteLine("1. Simple (sin fecha límite)");
            Console.WriteLine("2. Con vencimiento");
            int tipo = LeerEntero("Opción: ");

            Console.Write("Título: ");
            string titulo = Console.ReadLine() ?? string.Empty;

            Console.Write("Descripción: ");
            string descripcion = Console.ReadLine() ?? string.Empty;

            Prioridad prioridad = LeerPrioridad();

            Console.Write("Categoría: ");
            string categoria = Console.ReadLine() ?? string.Empty;

            if (tipo == 2)
            {
                DateTime fechaVencimiento = LeerFecha("Fecha de vencimiento (dd/MM/yyyy): ");
                TareaConVencimiento tarea = new TareaConVencimiento(titulo, descripcion, prioridad, categoria, fechaVencimiento);
                gestor.Agregar(tarea);
                Console.WriteLine("\n✅ Tarea con vencimiento agregada:");
                tarea.MostrarInfo();
            }
            else
            {
                Tarea tarea = new Tarea(titulo, descripcion, prioridad, categoria);
                gestor.Agregar(tarea);
                Console.WriteLine("\n✅ Tarea simple agregada:");
                tarea.MostrarInfo();
            }
        }

        // ------------------------------------------------------------------
        //  OPCIÓN 2: LISTAR TODAS (demuestra polimorfismo)
        // ------------------------------------------------------------------

        static void OpcionListarTodas()
        {
            Console.WriteLine("\n--- Todas las tareas ---");
            List<Tarea> todas = gestor.ListarTodas();

            if (todas.Count == 0)
            {
                Console.WriteLine("No hay tareas registradas.");
                return;
            }

            // Lista polimórfica: puede contener Tarea y TareaConVencimiento.
            // Al llamar MostrarInfo() cada objeto ejecuta su propia versión
            // (la de Tarea o la sobreescrita en TareaConVencimiento).
            foreach (Tarea tarea in todas)
            {
                tarea.MostrarInfo();
                Console.WriteLine($"      Exportar(): {tarea.Exportar()}");
                Console.WriteLine();
            }
        }

        // ------------------------------------------------------------------
        //  OPCIÓN 3: LISTAR POR CATEGORÍA
        // ------------------------------------------------------------------

        static void OpcionListarPorCategoria()
        {
            Console.Write("\nIngrese la categoría a buscar: ");
            string categoria = Console.ReadLine() ?? string.Empty;

            List<Tarea> resultado = gestor.ListarPorCategoria(categoria);
            MostrarListaOMensajeVacio(resultado, $"No hay tareas en la categoría '{categoria}'.");
        }

        // ------------------------------------------------------------------
        //  OPCIÓN 4: LISTAR POR PRIORIDAD
        // ------------------------------------------------------------------

        static void OpcionListarPorPrioridad()
        {
            Prioridad prioridad = LeerPrioridad();
            List<Tarea> resultado = gestor.ListarPorPrioridad(prioridad);
            MostrarListaOMensajeVacio(resultado, $"No hay tareas con prioridad '{prioridad}'.");
        }

        // ------------------------------------------------------------------
        //  OPCIÓN 5: MARCAR COMO COMPLETADA
        // ------------------------------------------------------------------

        static void OpcionMarcarCompletada()
        {
            int id = LeerEntero("\nIngrese el Id de la tarea a completar: ");
            bool exito = gestor.Completar(id);

            Console.WriteLine(exito
                ? $"\n✅ Tarea {id} marcada como completada."
                : $"\n⚠ No se encontró ninguna tarea con Id {id}.");
        }

        // ------------------------------------------------------------------
        //  OPCIÓN 6: MOSTRAR VENCIDAS
        // ------------------------------------------------------------------

        static void OpcionMostrarVencidas()
        {
            List<Tarea> vencidas = gestor.ObtenerVencidas();
            MostrarListaOMensajeVacio(vencidas, "No hay tareas vencidas. 🎉");
        }

        // ------------------------------------------------------------------
        //  OPCIÓN 7: ELIMINAR TAREA
        // ------------------------------------------------------------------

        static void OpcionEliminarTarea()
        {
            int id = LeerEntero("\nIngrese el Id de la tarea a eliminar: ");
            bool exito = gestor.Eliminar(id);

            Console.WriteLine(exito
                ? $"\n🗑 Tarea {id} eliminada."
                : $"\n⚠ No se encontró ninguna tarea con Id {id}.");
        }

        // ------------------------------------------------------------------
        //  OPCIÓN 8: EXPORTAR A JSON (manual, además del guardado automático al salir)
        // ------------------------------------------------------------------

        static void OpcionExportarJSON()
        {
            try
            {
                gestor.GuardarEnJSON(ArchivoTareas);
                Console.WriteLine($"\n✅ Tareas exportadas correctamente a {ArchivoTareas}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n⚠ Error al exportar: {ex.Message}");
            }
        }

        // ------------------------------------------------------------------
        //  UTILIDADES DE LECTURA
        // ------------------------------------------------------------------

        static void MostrarListaOMensajeVacio(List<Tarea> lista, string mensajeVacio)
        {
            Console.WriteLine();

            if (lista.Count == 0)
            {
                Console.WriteLine(mensajeVacio);
                return;
            }

            foreach (Tarea tarea in lista)
            {
                tarea.MostrarInfo();
                Console.WriteLine();
            }
        }

        static int LeerEntero(string mensaje)
        {
            Console.Write(mensaje);
            int valor;
            while (!int.TryParse(Console.ReadLine(), out valor))
            {
                Console.Write("Ingrese un número válido: ");
            }
            return valor;
        }

        static Prioridad LeerPrioridad()
        {
            Console.WriteLine("Prioridad: 1.Baja 2.Media 3.Alta 4.Critica");
            int opcion = LeerEntero("Opción: ");

            return opcion switch
            {
                1 => Prioridad.Baja,
                2 => Prioridad.Media,
                3 => Prioridad.Alta,
                4 => Prioridad.Critica,
                _ => Prioridad.Media
            };
        }

        static DateTime LeerFecha(string mensaje)
        {
            Console.Write(mensaje);
            string entrada = Console.ReadLine() ?? string.Empty;

            while (!DateTime.TryParse(entrada, out DateTime fecha))
            {
                Console.Write("Formato inválido. Use dd/MM/yyyy: ");
                entrada = Console.ReadLine() ?? string.Empty;
            }

            return DateTime.Parse(entrada);
        }
    }
}