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
    /// Controlador para la gestión de procesos de instructores de aprendices en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AprendizProcessInstructorController : ControllerBase
    {
        private readonly AprendizProcessInstructorBusiness _aprendizProcessInstructorBusiness;
        private readonly ILogger<AprendizProcessInstructorController> _logger;

        /// <summary>
        /// Constructor del controlador de procesos de instructores de aprendices
        /// </summary>
        /// <param name="aprendizProcessInstructorBusiness">Capa de negocio de procesos de instructores de aprendices</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public AprendizProcessInstructorController(AprendizProcessInstructorBusiness aprendizProcessInstructorBusiness, ILogger<AprendizProcessInstructorController> logger)
        {
            _aprendizProcessInstructorBusiness = aprendizProcessInstructorBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los procesos de instructores de aprendices del sistema
        /// </summary>
        /// <returns>Lista de procesos de instructores de aprendices</returns>
        /// <response code="200">Retorna la lista de procesos de instructores de aprendices</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AprendizProcessInstructorDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAprendizProcessInstructors()
        {
            try
            {
                var aprendizProcessInstructors = await _aprendizProcessInstructorBusiness.GetAllAprendizProcessInstructorsAsync();
                return Ok(aprendizProcessInstructors);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener procesos de instructores de aprendices");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un proceso de instructor de aprendiz específico por su ID
        /// </summary>
        /// <param name="id">ID del proceso de instructor de aprendiz</param>
        /// <returns>Proceso de instructor de aprendiz solicitado</returns>
        /// <response code="200">Retorna el proceso de instructor de aprendiz solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Proceso de instructor de aprendiz no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AprendizProcessInstructorDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAprendizProcessInstructorById(int id)
        {
            try
            {
                var aprendizProcessInstructor = await _aprendizProcessInstructorBusiness.GetAprendizProcessInstructorByIdAsync(id);
                return Ok(aprendizProcessInstructor);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el proceso de instructor de aprendiz con ID: {AprendizProcessInstructorId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Proceso de instructor de aprendiz no encontrado con ID: {AprendizProcessInstructorId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener proceso de instructor de aprendiz con ID: {AprendizProcessInstructorId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo proceso de instructor de aprendiz en el sistema
        /// </summary>
        /// <param name="aprendizProcessInstructorDto">Datos del proceso de instructor de aprendiz a crear</param>
        /// <returns>Proceso de instructor de aprendiz creado</returns>
        /// <response code="201">Retorna el proceso de instructor de aprendiz creado</response>
        /// <response code="400">Datos del proceso de instructor de aprendiz no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(AprendizProcessInstructorDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateAprendizProcessInstructor([FromBody] AprendizProcessInstructorDTO aprendizProcessInstructorDto)
        {
            try
            {
                var createdAprendizProcessInstructor = await _aprendizProcessInstructorBusiness.CreateAprendizProcessInstructorAsync(aprendizProcessInstructorDto);
                return CreatedAtAction(nameof(GetAprendizProcessInstructorById), new { id = createdAprendizProcessInstructor.Id }, createdAprendizProcessInstructor);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear proceso de instructor de aprendiz");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear proceso de instructor de aprendiz");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
