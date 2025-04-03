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
    /// Controlador para la gestión de programas de instructores en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class InstructorProgramController : ControllerBase
    {
        private readonly InstructorProgramBusiness _instructorProgramBusiness;
        private readonly ILogger<InstructorProgramController> _logger;

        /// <summary>
        /// Constructor del controlador de programas de instructores
        /// </summary>
        /// <param name="instructorProgramBusiness">Capa de negocio de programas de instructores</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public InstructorProgramController(InstructorProgramBusiness instructorProgramBusiness, ILogger<InstructorProgramController> logger)
        {
            _instructorProgramBusiness = instructorProgramBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los programas de instructores del sistema
        /// </summary>
        /// <returns>Lista de programas de instructores</returns>
        /// <response code="200">Retorna la lista de programas de instructores</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InstructorProgramDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllInstructorPrograms()
        {
            try
            {
                var instructorPrograms = await _instructorProgramBusiness.GetAllInstructorProgramsAsync();
                return Ok(instructorPrograms);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener programas de instructores");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un programa de instructor específico por su ID
        /// </summary>
        /// <param name="id">ID del programa de instructor</param>
        /// <returns>Programa de instructor solicitado</returns>
        /// <response code="200">Retorna el programa de instructor solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Programa de instructor no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InstructorProgramDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetInstructorProgramById(int id)
        {
            try
            {
                var instructorProgram = await _instructorProgramBusiness.GetInstructorProgramByIdAsync(id);
                return Ok(instructorProgram);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el programa de instructor con ID: {InstructorProgramId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Programa de instructor no encontrado con ID: {InstructorProgramId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener programa de instructor con ID: {InstructorProgramId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo programa de instructor en el sistema
        /// </summary>
        /// <param name="instructorProgramDto">Datos del programa de instructor a crear</param>
        /// <returns>Programa de instructor creado</returns>
        /// <response code="201">Retorna el programa de instructor creado</response>
        /// <response code="400">Datos del programa de instructor no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(InstructorProgramDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateInstructorProgram([FromBody] InstructorProgramDTO instructorProgramDto)
        {
            try
            {
                var createdInstructorProgram = await _instructorProgramBusiness.CreateInstructorProgramAsync(instructorProgramDto);
                return CreatedAtAction(nameof(GetInstructorProgramById), new { id = createdInstructorProgram.Id }, createdInstructorProgram);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear programa de instructor");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear programa de instructor");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}



