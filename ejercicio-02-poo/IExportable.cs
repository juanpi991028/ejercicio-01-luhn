namespace GestorTareasApp
{
    /// <summary>
    /// Contrato para cualquier clase que pueda exportarse
    /// a una representación de texto simple.
    /// </summary>
    public interface IExportable
    {
        /// <summary>
        /// Devuelve una representación en formato: "ID|Titulo|Prioridad|Completada"
        /// </summary>
        string Exportar();
    }
}
