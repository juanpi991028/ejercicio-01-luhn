# Validador de Tarjetas — Algoritmo de Luhn

Aplicación de consola en C# (.NET 8) que valida números de tarjeta de
crédito/débito usando el algoritmo de Luhn e identifica la marca
(Visa, Mastercard, American Express, Discover).

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download) o superior instalado.

## Cómo ejecutar

Desde esta carpeta (`ejercicio-01-luhn/`):

```bash
dotnet run
```

## Menú

```
=== VALIDADOR DE TARJETAS ===
1. Validar una tarjeta
2. Validar desde archivo
3. Generar número válido
4. Estadísticas
5. Salir
```

1. **Validar una tarjeta**: pide un número por teclado y muestra marca + estado.
2. **Validar desde archivo**: pide una ruta (por ejemplo `tarjetas_prueba.txt`,
   incluido en este repo) y procesa cada línea, mostrando un resumen al final.
3. **Generar número válido**: genera un número aleatorio que pasa Luhn,
   con prefijo real de alguna marca conocida.
4. **Estadísticas**: muestra el total de tarjetas procesadas en la sesión
   (válidas/inválidas) y un desglose por marca.
5. **Salir**.

## Números de prueba

| Número              | Marca esperada     | Resultado esperado |
|---------------------|---------------------|---------------------|
| 4532015112830366    | Visa                | ✅ VÁLIDA           |
| 4532015112830367    | Desconocida         | ❌ INVÁLIDA         |
| 5555555555554444    | Mastercard          | ✅ VÁLIDA           |
| 371449635398431     | American Express    | ✅ VÁLIDA           |
| 6011111111111117    | Discover            | ✅ VÁLIDA           |

## Estructura del código

- `ValidarTarjeta(string numero)`: implementa el algoritmo de Luhn.
- `IdentificarMarca(string numero)`: identifica la marca según prefijo/longitud.
- `ValidarDesdeArchivo(string ruta)`: lee un archivo y valida cada línea.
- `GenerarNumeroValido()`: genera un número aleatorio válido según Luhn.
- Métodos auxiliares con responsabilidad única (`EsVisa`, `EsMastercard`,
  `EsAmericanExpress`, `EsDiscover`, `LimpiarNumero`, `CalcularSumaLuhn`, etc.)
  para mantener el código modular y legible.

## Manejo de errores

- Entradas no numéricas o vacías al validar una tarjeta.
- Archivo inexistente o sin permisos de lectura al usar la opción 2.
- Líneas inválidas dentro del archivo se reportan y se ignoran sin
  detener el procesamiento del resto.
