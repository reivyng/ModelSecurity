using Business;
using Entity.DTOautogestion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de centros en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CenterController : ControllerBase
    {
        private readonly CenterBusiness _centerBusiness;
        private readonly ILogger<CenterController> _logger;

        /// <summary>
        /// Constructor del controlador de centros
        /// </summary>
        /// <param name="centerBusiness">Capa de negocio de centros</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public CenterController(CenterBusiness centerBusiness, ILogger<CenterController> logger)
        {
            _centerBusiness = centerBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los centros del sistema
        /// </summary>
        /// <returns>Lista de centros</returns>
        /// <response code="200">Retorna la lista de centros</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CenterDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllCenters()
        {
            try
            {
                var centers = await _centerBusiness.GetAllCentersAsync();
                return Ok(centers);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener centros");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un centro específico por su ID
        /// </summary>
        /// <param name="id">ID del centro</param>
        /// <returns>Centro solicitado</returns>
        /// <response code="200">Retorna el centro solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Centro no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CenterDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCenterById(int id)
        {
            try
            {
                var center = await _centerBusiness.GetCenterByIdAsync(id);
                return Ok(center);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el centro con ID: {CenterId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Centro no encontrado con ID: {CenterId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener centro con ID: {CenterId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo centro en el sistema
        /// </summary>
        /// <param name="centerDto">Datos del centro a crear</param>
        /// <returns>Centro creado</returns>
        /// <response code="201">Retorna el centro creado</response>
        /// <response code="400">Datos del centro no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(CenterDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateCenter([FromBody] CenterDTO centerDto)
        {
            try
            {
                var createdCenter = await _centerBusiness.CreateCenterAsync(centerDto);
                return CreatedAtAction(nameof(GetCenterById), new { id = createdCenter.Id }, createdCenter);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear centro");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear centro");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}

