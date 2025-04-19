using Business;
using Entity.DTOautogestion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de Instructor en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class InstructorController : ControllerBase
    {
        private readonly InstructorBusiness _instructorBusiness;
        private readonly ILogger<InstructorController> _logger;

        /// <summary>
        /// Constructor del controlador de Instructor
        /// </summary>
        public InstructorController(InstructorBusiness instructorBusiness, ILogger<InstructorController> logger)
        {
            _instructorBusiness = instructorBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los instructores del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InstructorDto>), 200)]
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
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InstructorDto), 200)]
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
        [HttpPost]
        [ProducesResponseType(typeof(InstructorDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateInstructor([FromBody] InstructorDto instructorDto)
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

        /// <summary>
        /// Actualiza un instructor existente
        /// </summary>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateInstructor([FromBody] InstructorDto instructorDto)
        {
            try
            {
                var result = await _instructorBusiness.UpdateInstructorAsync(instructorDto);
                if (result)
                {
                    return Ok(new { message = "El instructor fue actualizado exitosamente." });
                }
                return NotFound(new { message = "El instructor no fue encontrado." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar instructor");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar instructor");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un instructor existente
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialInstructor(int id, [FromBody] InstructorDto updatedFields)
        {
            try
            {
                var result = await _instructorBusiness.UpdatePartialInstructorAsync(id, updatedFields);
                if (result)
                {
                    return Ok(new { message = "El instructor fue actualizado parcialmente con éxito." });
                }
                return NotFound(new { message = "El instructor no fue encontrado." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente instructor con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente instructor con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza una eliminación lógica de un instructor
        /// </summary>
        [HttpDelete("soft/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteInstructor(int id)
        {
            try
            {
                var result = await _instructorBusiness.SoftDeleteInstructorAsync(id);
                if (result)
                {
                    return Ok(new { message = "El instructor fue eliminado lógicamente con éxito." });
                }
                return NotFound(new { message = "El instructor no fue encontrado." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al realizar eliminación lógica del instructor con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar eliminación lógica del instructor con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un instructor por su ID
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            try
            {
                var result = await _instructorBusiness.DeleteInstructorAsync(id);
                if (result)
                {
                    return Ok(new { message = "El instructor fue eliminado exitosamente." });
                }
                return NotFound(new { message = "El instructor no fue encontrado." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar instructor con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar instructor con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
