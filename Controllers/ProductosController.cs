using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarInventoryAPI.Data;
using BarInventoryAPI.Models;

namespace BarInventoryAPI.Controllers;

[ApiController]
[Route("api/[controller]")] // La ruta será: api/productos
public class ProductosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    // El constructor recibe el puente a la base de datos
    public ProductosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. GET: api/productos (Obtener todos los productos del inventario)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
    {
        var productos = await _context.Productos.ToListAsync();
        return Ok(productos); // Devuelve un código 200 OK con la lista en JSON
    }

    // 2. POST: api/productos (Crear un nuevo producto)
    [HttpPost]
    public async Task<ActionResult<Producto>> PostProducto(Producto producto)
    {
    // El ID no lo enviamos, MySQL lo genera solo
    _context.Productos.Add(producto);
    await _context.SaveChangesAsync();

    // Devuelve un código 201 Created y el producto con su ID asignado
    return CreatedAtAction(nameof(GetProductos), new { id = producto.Id }, producto);
    }

    // 3. PUT: api/productos/{id}/descontar (Reducir inventario por venta)
    [HttpPut("{id}/descontar")]
    public async Task<IActionResult> DescontarStock(int id, [FromBody] int cantidadAConsumir)
        {
    if (cantidadAConsumir <= 0)
    {
        return BadRequest("La cantidad a consumir debe ser mayor a cero.");
    }

    // Buscamos el producto en la base de datos por su ID
    var producto = await _context.Productos.FindAsync(id);

    if (producto == null)
    {
        return NotFound($"No se encontró el producto con ID {id}.");
    }

    // REGLA DE NEGOCIO: Validar si hay suficiente stock disponible
    if (producto.StockActual < cantidadAConsumir)
    {
        return BadRequest($"Stock insuficiente. Solo quedan {producto.StockActual} {producto.UnidadMedida} de {producto.Nombre}.");
    }

    // Hacemos la resta matemática
    producto.StockActual -= cantidadAConsumir;

    // Guardamos los cambios en MySQL
    await _context.SaveChangesAsync();

    // ALERTA DE STOCK BAJO: Si bajó del mínimo, mandamos un mensaje especial de advertencia
    if (producto.StockActual <= producto.StockMinimo)
    {
        return Ok(new { 
            mensaje = $"Stock actualizado con éxito. ¡ALERTA! El producto {producto.Nombre} ha alcanzado o bajado de su stock mínimo.", 
            producto 
        });
    }

    return Ok(new { mensaje = "Stock descontado correctamente.", producto });
    }

    // 4. GET: api/productos/bajo-stock (Reporte de productos críticos)
    [HttpGet("bajo-stock")]
    public async Task<ActionResult<IEnumerable<Producto>>> GetProductosBajoStock()
    {
    // Filtramos: StockActual debe ser menor o igual al StockMinimo
    var productosCriticos = await _context.Productos
        .Where(p => p.StockActual <= p.StockMinimo)
        .ToListAsync();

    return Ok(productosCriticos);
    }

    // 5. GET: api/productos/{id} (Obtener un solo producto por ID)
    [HttpGet("{id}")]
    public async Task<ActionResult<Producto>> GetProducto(int id)
    {
    var producto = await _context.Productos.FindAsync(id);

    if (producto == null)
    {
        return NotFound($"No se encontró el producto con el ID {id}.");
    }

    return Ok(producto);
    }

    // 6. DELETE: api/productos/{id} (Eliminar un producto)
[HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProducto(int id)
    {
    var producto = await _context.Productos.FindAsync(id);
    if (producto == null)
    {
        return NotFound($"No se encontró el producto con el ID {id} para eliminar.");
    }

    _context.Productos.Remove(producto);
    await _context.SaveChangesAsync();

    return Ok(new { mensaje = $"El producto {producto.Nombre} fue eliminado correctamente." });
    }
}