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
    /// Controlador para la gestión de instructores en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class InstructorController : ControllerBase
    {
        private readonly InstructorBusiness _instructorBusiness;
        private readonly ILogger<InstructorController> _logger;

        /// <summary>
        /// Constructor del controlador de instructores
        /// </summary>
        /// <param name="instructorBusiness">Capa de negocio de instructores</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public InstructorController(InstructorBusiness instructorBusiness, ILogger<InstructorController> logger)
        {
            _instructorBusiness = instructorBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los instructores del sistema
        /// </summary>
        /// <returns>Lista de instructores</returns>
        /// <response code="200">Retorna la lista de instructores</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InstructorDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllInstructors()
        {
            try
            {
                var instructors = await _instructorBusiness.GetAllInstructorsAsync();
                return Ok(instructors);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener instructores");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un instructor específico por su ID
        /// </summary>
        /// <param name="id">ID del instructor</param>
        /// <returns>Instructor solicitado</returns>
        /// <response code="200">Retorna el instructor solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Instructor no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InstructorDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetInstructorById(int id)
        {
            try
            {
                var instructor = await _instructorBusiness.GetInstructorByIdAsync(id);
                return Ok(instructor);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el instructor con ID: {InstructorId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Instructor no encontrado con ID: {InstructorId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener instructor con ID: {InstructorId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo instructor en el sistema
        /// </summary>
        /// <param name="instructorDto">Datos del instructor a crear</param>
        /// <returns>Instructor creado</returns>
        /// <response code="201">Retorna el instructor creado</response>
        /// <response code="400">Datos del instructor no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(InstructorDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateInstructor([FromBody] InstructorDTO instructorDto)
        {
            try
            {
                var createdInstructor = await _instructorBusiness.CreateInstructorAsync(instructorDto);
                return CreatedAtAction(nameof(GetInstructorById), new { id = createdInstructor.Id }, createdInstructor);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear instructor");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear instructor");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}


