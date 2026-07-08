using System.ComponentModel.DataAnnotations;

namespace BarInventoryAPI.Models;

public class Producto
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La categoría (Licor, Mezclador, Fruta) es obligatoria.")]
    [StringLength(50)]
    public string Categoria { get; set; } = string.Empty;

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "El stock actual no puede ser negativo.")]
    public int StockActual { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    public int StockMinimo { get; set; }

    [Required]
    [StringLength(20)]
    public string UnidadMedida { get; set; } = "Piezas"; // Ej: "Mililitros", "Piezas", "Onzas"
}