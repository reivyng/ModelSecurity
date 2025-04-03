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
    /// Controlador para la gestión de registros regionales en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RegionalController : ControllerBase
    {
        private readonly RegionalBusiness _regionalBusiness;
        private readonly ILogger<RegionalController> _logger;

        /// <summary>
        /// Constructor del controlador de registros regionales
        /// </summary>
        /// <param name="regionalBusiness">Capa de negocio de registros regionales</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public RegionalController(RegionalBusiness regionalBusiness, ILogger<RegionalController> logger)
        {
            _regionalBusiness = regionalBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros regionales del sistema
        /// </summary>
        /// <returns>Lista de registros regionales</returns>
        /// <response code="200">Retorna la lista de registros regionales</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RegionalDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRegionals()
        {
            try
            {
                var regionals = await _regionalBusiness.GetAllRegionalsAsync();
                return Ok(regionals);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener registros regionales");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un registro regional específico por su ID
        /// </summary>
        /// <param name="id">ID del registro regional</param>
        /// <returns>Registro regional solicitado</returns>
        /// <response code="200">Retorna el registro regional solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Registro regional no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RegionalDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRegionalById(int id)
        {
            try
            {
                var regional = await _regionalBusiness.GetRegionalByIdAsync(id);
                return Ok(regional);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el registro regional con ID: {RegionalId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro regional no encontrado con ID: {RegionalId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener registro regional con ID: {RegionalId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo registro regional en el sistema
        /// </summary>
        /// <param name="regionalDto">Datos del registro regional a crear</param>
        /// <returns>Registro regional creado</returns>
        /// <response code="201">Retorna el registro regional creado</response>
        /// <response code="400">Datos del registro regional no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RegionalDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRegional([FromBody] RegionalDTO regionalDto)
        {
            try
            {
                var createdRegional = await _regionalBusiness.CreateRegionalAsync(regionalDto);
                return CreatedAtAction(nameof(GetRegionalById), new { id = createdRegional.Id }, createdRegional);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear registro regional");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear registro regional");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}





