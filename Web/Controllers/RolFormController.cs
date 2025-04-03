using Business;
using Entity.DTOautogestion.pivote;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de relaciones rol-form en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolFormController : ControllerBase
    {
        private readonly RolFormBusiness _rolFormBusiness;
        private readonly ILogger<RolFormController> _logger;

        /// <summary>
        /// Constructor del controlador de relaciones rol-form
        /// </summary>
        /// <param name="rolFormBusiness">Capa de negocio de relaciones rol-form</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public RolFormController(RolFormBusiness rolFormBusiness, ILogger<RolFormController> logger)
        {
            _rolFormBusiness = rolFormBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las relaciones rol-form del sistema
        /// </summary>
        /// <returns>Lista de relaciones rol-form</returns>
        /// <response code="200">Retorna la lista de relaciones rol-form</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolFormDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRolForms()
        {
            try
            {
                var rolForms = await _rolFormBusiness.GetAllRolFormsAsync();
                return Ok(rolForms);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener relaciones rol-form");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una relación rol-form específica por su ID
        /// </summary>
        /// <param name="id">ID de la relación rol-form</param>
        /// <returns>Relación rol-form solicitada</returns>
        /// <response code="200">Retorna la relación rol-form solicitada</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Relación rol-form no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolFormDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolFormById(int id)
        {
            try
            {
                var rolForm = await _rolFormBusiness.GetRolFormByIdAsync(id);
                return Ok(rolForm);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la relación rol-form con ID: {RolFormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Relación rol-form no encontrada con ID: {RolFormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener relación rol-form con ID: {RolFormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva relación rol-form en el sistema
        /// </summary>
        /// <param name="rolFormDto">Datos de la relación a crear</param>
        /// <returns>Relación rol-form creada</returns>
        /// <response code="201">Retorna la relación rol-form creada</response>
        /// <response code="400">Datos de la relación no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RolFormDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRolForm([FromBody] RolFormDTO rolFormDto)
        {
            try
            {
                var createdRolForm = await _rolFormBusiness.CreateRolFormAsync(rolFormDto);
                return CreatedAtAction(nameof(GetRolFormById), new { id = createdRolForm.Id }, createdRolForm);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear relación rol-form");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear relación rol-form");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}