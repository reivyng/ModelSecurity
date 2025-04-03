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
    /// Controlador para la gestión de programas de aprendices en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AprendizProgramController : ControllerBase
    {
        private readonly AprendizProgramBusiness _aprendizProgramBusiness;
        private readonly ILogger<AprendizProgramController> _logger;

        /// <summary>
        /// Constructor del controlador de programas de aprendices
        /// </summary>
        /// <param name="aprendizProgramBusiness">Capa de negocio de programas de aprendices</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public AprendizProgramController(AprendizProgramBusiness aprendizProgramBusiness, ILogger<AprendizProgramController> logger)
        {
            _aprendizProgramBusiness = aprendizProgramBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los programas de aprendices del sistema
        /// </summary>
        /// <returns>Lista de programas de aprendices</returns>
        /// <response code="200">Retorna la lista de programas de aprendices</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AprendizProgramDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAprendizPrograms()
        {
            try
            {
                var aprendizPrograms = await _aprendizProgramBusiness.GetAllAprendizProgramsAsync();
                return Ok(aprendizPrograms);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener programas de aprendices");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un programa de aprendiz específico por su ID
        /// </summary>
        /// <param name="id">ID del programa de aprendiz</param>
        /// <returns>Programa de aprendiz solicitado</returns>
        /// <response code="200">Retorna el programa de aprendiz solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Programa de aprendiz no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AprendizProgramDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAprendizProgramById(int id)
        {
            try
            {
                var aprendizProgram = await _aprendizProgramBusiness.GetAprendizProgramByIdAsync(id);
                return Ok(aprendizProgram);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el programa de aprendiz con ID: {AprendizProgramId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Programa de aprendiz no encontrado con ID: {AprendizProgramId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener programa de aprendiz con ID: {AprendizProgramId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo programa de aprendiz en el sistema
        /// </summary>
        /// <param name="aprendizProgramDto">Datos del programa de aprendiz a crear</param>
        /// <returns>Programa de aprendiz creado</returns>
        /// <response code="201">Retorna el programa de aprendiz creado</response>
        /// <response code="400">Datos del programa de aprendiz no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(AprendizProgramDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateAprendizProgram([FromBody] AprendizProgramDTO aprendizProgramDto)
        {
            try
            {
                var createdAprendizProgram = await _aprendizProgramBusiness.CreateAprendizProgramAsync(aprendizProgramDto);
                return CreatedAtAction(nameof(GetAprendizProgramById), new { id = createdAprendizProgram.Id }, createdAprendizProgram);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear programa de aprendiz");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear programa de aprendiz");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
