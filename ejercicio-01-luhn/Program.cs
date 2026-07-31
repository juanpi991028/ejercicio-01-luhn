using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ValidadorTarjetas
{
    /// <summary>
    /// Representa el resultado de validar una tarjeta,
    /// usado para acumular estadísticas durante la sesión.
    /// </summary>
    class ResultadoValidacion
    {
        public string Numero { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public bool EsValida { get; set; }
    }

    class Program
    {
        // Historial en memoria de todas las validaciones hechas en la sesión
        // (se usa para armar la opción 4 - Estadísticas)
        static List<ResultadoValidacion> historial = new List<ResultadoValidacion>();

        static void Main(string[] args)
        {
            int opcion;

            do
            {
                MostrarMenu();
                opcion = LeerOpcionMenu();

                switch (opcion)
                {
                    case 1:
                        OpcionValidarUnaTarjeta();
                        break;
                    case 2:
                        OpcionValidarDesdeArchivo();
                        break;
                    case 3:
                        OpcionGenerarNumeroValido();
                        break;
                    case 4:
                        MostrarEstadisticas();
                        break;
                    case 5:
                        Console.WriteLine("\n¡Hasta luego!");
                        break;
                    default:
                        Console.WriteLine("\nOpción no válida. Intente de nuevo.");
                        break;
                }

                if (opcion != 5)
                {
                    Console.WriteLine("\nPresione ENTER para continuar...");
                    Console.ReadLine();
                }

            } while (opcion != 5);
        }

        // ------------------------------------------------------------------
        //  MENÚ
        // ------------------------------------------------------------------

        static void MostrarMenu()
        {
            Console.Clear();
            Console.WriteLine("=== VALIDADOR DE TARJETAS ===");
            Console.WriteLine("1. Validar una tarjeta");
            Console.WriteLine("2. Validar desde archivo");
            Console.WriteLine("3. Generar número válido");
            Console.WriteLine("4. Estadísticas");
            Console.WriteLine("5. Salir");
            Console.Write("\nSeleccione una opción: ");
        }

        /// <summary>
        /// Lee la opción del menú manejando entradas inválidas (no numéricas).
        /// </summary>
        static int LeerOpcionMenu()
        {
            string? entrada = Console.ReadLine();

            if (int.TryParse(entrada, out int opcion))
            {
                return opcion;
            }

            // Si no se pudo convertir, devolvemos un valor fuera de rango
            // para que caiga en el "default" del switch.
            return -1;
        }

        // ------------------------------------------------------------------
        //  OPCIÓN 1: VALIDAR UNA TARJETA
        // ------------------------------------------------------------------

        static void OpcionValidarUnaTarjeta()
        {
            Console.Write("\nIngrese el número de tarjeta: ");
            string? entrada = Console.ReadLine();

            try
            {
                ResultadoValidacion resultado = ProcesarNumero(entrada ?? string.Empty);
                MostrarResultado(resultado);
                historial.Add(resultado);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n⚠ Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Valida el número, identifica la marca y arma el resultado.
        /// Lanza ArgumentException si el formato es inválido.
        /// </summary>
        static ResultadoValidacion ProcesarNumero(string numeroOriginal)
        {
            string numeroLimpio = LimpiarNumero(numeroOriginal);

            if (string.IsNullOrWhiteSpace(numeroLimpio))
            {
                throw new ArgumentException("El número de tarjeta no puede estar vacío.");
            }

            if (!SoloContieneDigitos(numeroLimpio))
            {
                throw new ArgumentException("El número de tarjeta solo debe contener dígitos.");
            }

            bool esValida = ValidarTarjeta(numeroLimpio);
            string marca = IdentificarMarca(numeroLimpio);

            return new ResultadoValidacion
            {
                Numero = numeroLimpio,
                Marca = marca,
                EsValida = esValida
            };
        }

        static void MostrarResultado(ResultadoValidacion resultado)
        {
            Console.WriteLine();
            Console.WriteLine($"Número: {resultado.Numero}");
            Console.WriteLine($"Marca: {resultado.Marca}");
            Console.WriteLine($"Estado: {(resultado.EsValida ? "✅ VÁLIDA" : "❌ INVÁLIDA")}");
        }

        // ------------------------------------------------------------------
        //  OPCIÓN 2: VALIDAR DESDE ARCHIVO
        // ------------------------------------------------------------------

        static void OpcionValidarDesdeArchivo()
        {
            Console.Write("\nIngrese la ruta del archivo: ");
            string? ruta = Console.ReadLine();

            ValidarDesdeArchivo(ruta ?? string.Empty);
        }

        /// <summary>
        /// Lee un archivo de texto con un número de tarjeta por línea,
        /// valida cada uno y muestra un resumen al final.
        /// </summary>
        static void ValidarDesdeArchivo(string ruta)
        {
            try
            {
                if (!File.Exists(ruta))
                {
                    throw new FileNotFoundException($"No se encontró el archivo: {ruta}");
                }

                string[] lineas = File.ReadAllLines(ruta);

                int validas = 0;
                int invalidas = 0;
                int lineasIgnoradas = 0;

                Console.WriteLine("\n--- Resultados ---");

                foreach (string linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea))
                    {
                        continue; // ignorar líneas vacías
                    }

                    try
                    {
                        ResultadoValidacion resultado = ProcesarNumero(linea);
                        MostrarResultado(resultado);
                        Console.WriteLine("------------------");

                        historial.Add(resultado);

                        if (resultado.EsValida) validas++;
                        else invalidas++;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"\n⚠ Línea ignorada ('{linea}'): {ex.Message}");
                        Console.WriteLine("------------------");
                        lineasIgnoradas++;
                    }
                }

                Console.WriteLine("\n--- Resumen del archivo ---");
                Console.WriteLine($"Total de líneas procesadas: {validas + invalidas}");
                Console.WriteLine($"✅ Válidas: {validas}");
                Console.WriteLine($"❌ Inválidas: {invalidas}");
                if (lineasIgnoradas > 0)
                {
                    Console.WriteLine($"⚠ Líneas ignoradas por formato inválido: {lineasIgnoradas}");
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"\n⚠ Error: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"\n⚠ Error de lectura del archivo: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"\n⚠ No tiene permisos para leer el archivo: {ex.Message}");
            }
        }

        // ------------------------------------------------------------------
        //  OPCIÓN 3: GENERAR NÚMERO VÁLIDO
        // ------------------------------------------------------------------

        static void OpcionGenerarNumeroValido()
        {
            string numeroGenerado = GenerarNumeroValido();
            string marca = IdentificarMarca(numeroGenerado);

            Console.WriteLine("\n--- Número generado ---");
            Console.WriteLine($"Número: {numeroGenerado}");
            Console.WriteLine($"Marca: {marca}");
            Console.WriteLine($"Estado: {(ValidarTarjeta(numeroGenerado) ? "✅ VÁLIDA" : "❌ INVÁLIDA")}");

            historial.Add(new ResultadoValidacion
            {
                Numero = numeroGenerado,
                Marca = marca,
                EsValida = true
            });
        }

        /// <summary>
        /// Genera un número de tarjeta aleatorio (con prefijo de marca real)
        /// que cumple con el algoritmo de Luhn.
        /// </summary>
        static string GenerarNumeroValido()
        {
            Random rnd = new Random();

            // Prefijos típicos de cada marca para que el número generado
            // también sea identificable por IdentificarMarca().
            string[] prefijosPosibles =
            {
                "4",        // Visa (13 o 16)
                "51", "52", "53", "54", "55", // Mastercard (16)
                "34", "37", // American Express (15)
                "6011", "65" // Discover (16)
            };

            string prefijo = prefijosPosibles[rnd.Next(prefijosPosibles.Length)];

            int longitudTotal = DeterminarLongitudParaPrefijo(prefijo, rnd);

            StringBuilder sb = new StringBuilder(prefijo);

            // Rellenar con dígitos aleatorios hasta dejar 1 espacio
            // para el dígito verificador de Luhn.
            while (sb.Length < longitudTotal - 1)
            {
                sb.Append(rnd.Next(0, 10));
            }

            string numeroSinVerificador = sb.ToString();
            int digitoVerificador = CalcularDigitoVerificador(numeroSinVerificador);

            return numeroSinVerificador + digitoVerificador;
        }

        static int DeterminarLongitudParaPrefijo(string prefijo, Random rnd)
        {
            if (prefijo == "34" || prefijo == "37")
            {
                return 15; // American Express
            }

            if (prefijo == "4")
            {
                // Visa puede ser de 13 o 16 dígitos
                return rnd.Next(2) == 0 ? 13 : 16;
            }

            // Mastercard y Discover en este generador: 16 dígitos
            return 16;
        }

        /// <summary>
        /// Calcula el dígito verificador de Luhn que hace que el número
        /// completo (numeroParcial + dígito) sea válido.
        /// </summary>
        static int CalcularDigitoVerificador(string numeroParcial)
        {
            // Se agrega un "0" temporal en la posición del dígito verificador
            // para poder calcular la suma de Luhn del resto del número.
            string numeroConPlaceholder = numeroParcial + "0";

            int suma = CalcularSumaLuhn(numeroConPlaceholder);
            int residuo = suma % 10;

            return residuo == 0 ? 0 : 10 - residuo;
        }

        // ------------------------------------------------------------------
        //  OPCIÓN 4: ESTADÍSTICAS
        // ------------------------------------------------------------------

        static void MostrarEstadisticas()
        {
            Console.WriteLine("\n--- Estadísticas de la sesión ---");

            if (historial.Count == 0)
            {
                Console.WriteLine("Todavía no se ha validado ninguna tarjeta.");
                return;
            }

            int totalValidas = historial.Count(r => r.EsValida);
            int totalInvalidas = historial.Count(r => !r.EsValida);

            Console.WriteLine($"Total de tarjetas procesadas: {historial.Count}");
            Console.WriteLine($"✅ Válidas: {totalValidas}");
            Console.WriteLine($"❌ Inválidas: {totalInvalidas}");

            Console.WriteLine("\nDesglose por marca:");

            var porMarca = historial
                .GroupBy(r => r.Marca)
                .OrderByDescending(g => g.Count());

            foreach (var grupo in porMarca)
            {
                int validasDelGrupo = grupo.Count(r => r.EsValida);
                int invalidasDelGrupo = grupo.Count(r => !r.EsValida);
                Console.WriteLine($"  - {grupo.Key}: {grupo.Count()} total (✅ {validasDelGrupo} / ❌ {invalidasDelGrupo})");
            }
        }

        // ------------------------------------------------------------------
        //  ALGORITMO DE LUHN
        // ------------------------------------------------------------------

        /// <summary>
        /// Implementa el algoritmo de Luhn para validar un número de tarjeta.
        /// </summary>
        static bool ValidarTarjeta(string numero)
        {
            string limpio = LimpiarNumero(numero);

            if (limpio.Length < 12 || limpio.Length > 19)
            {
                return false;
            }

            if (!SoloContieneDigitos(limpio))
            {
                return false;
            }

            int suma = CalcularSumaLuhn(limpio);
            return suma % 10 == 0;
        }

        /// <summary>
        /// Calcula la suma de Luhn: invierte el número y duplica los
        /// dígitos en posición par (1-indexada del número invertido).
        /// </summary>
        static int CalcularSumaLuhn(string numero)
        {
            char[] invertido = numero.Reverse().ToArray();
            int suma = 0;

            for (int i = 0; i < invertido.Length; i++)
            {
                int digito = (int)char.GetNumericValue(invertido[i]);
                int posicion = i + 1; // 1-indexado

                if (posicion % 2 == 0)
                {
                    digito *= 2;
                    if (digito >= 10)
                    {
                        digito = digito / 10 + digito % 10; // ej: 16 -> 1+6=7
                    }
                }

                suma += digito;
            }

            return suma;
        }

        // ------------------------------------------------------------------
        //  IDENTIFICACIÓN DE MARCA
        // ------------------------------------------------------------------

        static string IdentificarMarca(string numero)
        {
            string limpio = LimpiarNumero(numero);
            int longitud = limpio.Length;

            if (EsVisa(limpio, longitud)) return "Visa";
            if (EsMastercard(limpio, longitud)) return "Mastercard";
            if (EsAmericanExpress(limpio, longitud)) return "American Express";
            if (EsDiscover(limpio, longitud)) return "Discover";

            return "Desconocida";
        }

        static bool EsVisa(string numero, int longitud)
        {
            return numero.StartsWith("4") && (longitud == 13 || longitud == 16);
        }

        static bool EsMastercard(string numero, int longitud)
        {
            if (longitud != 16 || numero.Length < 2)
            {
                return false;
            }

            int prefijo2 = int.Parse(numero.Substring(0, 2));
            return prefijo2 >= 51 && prefijo2 <= 55;
        }

        static bool EsAmericanExpress(string numero, int longitud)
        {
            return longitud == 15 && (numero.StartsWith("34") || numero.StartsWith("37"));
        }

        static bool EsDiscover(string numero, int longitud)
        {
            if (longitud < 16 || longitud > 19)
            {
                return false;
            }

            if (numero.StartsWith("6011")) return true;
            if (numero.StartsWith("65")) return true;

            if (numero.Length >= 3)
            {
                int prefijo3 = int.Parse(numero.Substring(0, 3));
                if (prefijo3 >= 644 && prefijo3 <= 649) return true;
            }

            if (numero.Length >= 6)
            {
                int prefijo6 = int.Parse(numero.Substring(0, 6));
                if (prefijo6 >= 622126 && prefijo6 <= 622925) return true;
            }

            return false;
        }

        // ------------------------------------------------------------------
        //  UTILIDADES
        // ------------------------------------------------------------------

        /// <summary>
        /// Quita espacios y guiones que suelen usarse para separar
        /// grupos de dígitos en un número de tarjeta.
        /// </summary>
        static string LimpiarNumero(string numero)
        {
            return numero.Replace(" ", "").Replace("-", "").Trim();
        }

        static bool SoloContieneDigitos(string texto)
        {
            foreach (char c in texto)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
