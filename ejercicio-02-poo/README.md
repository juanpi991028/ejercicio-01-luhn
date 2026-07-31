# Gestor de Tareas (POO)

Aplicación de consola en C# (.NET) que gestiona tareas usando Programación
Orientada a Objetos: herencia, polimorfismo, encapsulamiento, interfaces
y persistencia en JSON.

## Cómo ejecutar

Desde esta carpeta:

```bash
dotnet run
```

## Menú

```
=== GESTOR DE TAREAS ===
1. Agregar tarea
2. Listar todas
3. Listar por categoría
4. Listar por prioridad
5. Marcar como completada
6. Mostrar tareas vencidas
7. Eliminar tarea
8. Exportar a JSON
9. Salir
```

Al iniciar, el programa carga automáticamente `tareas.json` si existe.
Al salir (opción 9), guarda automáticamente los cambios en ese mismo archivo.

## Estructura del proyecto

| Archivo                     | Contenido                                                   |
|------------------------------|--------------------------------------------------------------|
| `Prioridad.cs`               | Enum con los 4 niveles de prioridad                          |
| `IExportable.cs`             | Interfaz con el método `Exportar()`                          |
| `Categoria.cs`                | Clase simple para representar una categoría                  |
| `Tarea.cs`                    | Clase base: encapsulamiento, `MostrarInfo()`, `Exportar()`   |
| `TareaConVencimiento.cs`      | Hereda de `Tarea`, agrega `FechaVencimiento` y `DiasRestantes` |
| `TareaDTO.cs`                 | DTO usado para serializar/deserializar de forma polimórfica  |
| `GestorTareas.cs`             | Lógica de negocio: filtros, persistencia JSON                |
| `Program.cs`                  | Menú principal                                                |

## Sobre la persistencia polimórfica

`System.Text.Json` no reconstruye automáticamente si un objeto guardado era
una `Tarea` o una `TareaConVencimiento`. Para resolver esto, `GestorTareas`
convierte cada tarea a un `TareaDTO` con un campo `Tipo` ("Simple" o
"ConVencimiento") antes de guardar, y usa ese mismo campo para reconstruir
el objeto correcto al cargar.

## Cómo probar cada requisito

- **Polimorfismo**: usa la opción 2 (Listar todas) después de crear una
  tarea simple y una con vencimiento — vas a ver que cada una imprime su
  propia versión de `MostrarInfo()`.
- **Persistencia**: agrega un par de tareas, sal con la opción 9, vuelve a
  correr `dotnet run` y usa la opción 2 — los datos deben seguir ahí.
- **Vencidas**: crea una tarea con vencimiento en una fecha pasada y
  pruébala con la opción 6.
