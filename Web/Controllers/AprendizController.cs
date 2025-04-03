using Business;
using Entity.DTOautogestion;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de aprendices en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AprendizController : ControllerBase
    {
        private readonly AprendizBusiness _AprendizBusiness;
        private readonly ILogger<AprendizController> _logger;

        /// <summary>
        /// Constructor del controlador de aprendices
        /// </summary>
        /// <param name="AprendizBusiness">Capa de negocio de aprendices</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public AprendizController(AprendizBusiness AprendizBusiness, ILogger<AprendizController> logger)
        {
            _AprendizBusiness = AprendizBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los aprendices del sistema
        /// </summary>
        /// <returns>Lista de aprendices</returns>
        /// <response code="200">Retorna la lista de aprendices</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AprendizDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAprendices()
        {
            try
            {
                var aprendices = await _AprendizBusiness.GetAllAprendicesAsync();
                return Ok(aprendices);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener aprendices");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un aprendiz específico por su ID
        /// </summary>
        /// <param name="id">ID del aprendiz</param>
        /// <returns>Aprendiz solicitado</returns>
        /// <response code="200">Retorna el aprendiz solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Aprendiz no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AprendizDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAprendizById(int id)
        {
            try
            {
                var aprendiz = await _AprendizBusiness.GetAprendizByIdAsync(id);
                return Ok(aprendiz);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el aprendiz con ID: {AprendizId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Aprendiz no encontrado con ID: {AprendizId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener aprendiz con ID: {AprendizId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo aprendiz en el sistema
        /// </summary>
        /// <param name="AprendizDto">Datos del aprendiz a crear</param>
        /// <returns>Aprendiz creado</returns>
        /// <response code="201">Retorna el aprendiz creado</response>
        /// <response code="400">Datos del aprendiz no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(AprendizDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateAprendiz([FromBody] AprendizDTO AprendizDto)
        {
            try
            {
                var createdAprendiz = await _AprendizBusiness.CreateAprendizAsync(AprendizDto);
                return CreatedAtAction(nameof(GetAprendizById), new { id = createdAprendiz.Id }, createdAprendiz);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear aprendiz");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear aprendiz");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
