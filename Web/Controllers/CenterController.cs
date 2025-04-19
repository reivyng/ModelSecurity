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
    /// Controlador para la gestión de Center en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CenterController : ControllerBase
    {
        private readonly CenterBusiness _centerBusiness;
        private readonly ILogger<CenterController> _logger;

        /// <summary>
        /// Constructor del controlador de Center
        /// </summary>
        public CenterController(CenterBusiness centerBusiness, ILogger<CenterController> logger)
        {
            _centerBusiness = centerBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los centros del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CenterDto>), 200)]
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
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CenterDto), 200)]
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
        [HttpPost]
        [ProducesResponseType(typeof(CenterDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateCenter([FromBody] CenterDto centerDto)
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

        /// <summary>
        /// Actualiza un centro existente
        /// </summary>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCenter([FromBody] CenterDto centerDto)
        {
            try
            {
                var result = await _centerBusiness.UpdateCenterAsync(centerDto);
                if (result)
                {
                    return Ok(new { message = "El centro fue actualizado exitosamente." });
                }
                return NotFound(new { message = "El centro no fue encontrado." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar centro");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar centro");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un centro existente
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialCenter(int id, [FromBody] CenterDto updatedFields)
        {
            try
            {
                var result = await _centerBusiness.UpdatePartialCenterAsync(id, updatedFields);
                if (result)
                {
                    return Ok(new { message = "El centro fue actualizado parcialmente con éxito." });
                }
                return NotFound(new { message = "El centro no fue encontrado." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente centro con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente centro con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza una eliminación lógica de un centro
        /// </summary>
        [HttpDelete("soft/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteCenter(int id)
        {
            try
            {
                var result = await _centerBusiness.SoftDeleteCenterAsync(id);
                if (result)
                {
                    return Ok(new { message = "El centro fue eliminado lógicamente con éxito." });
                }
                return NotFound(new { message = "El centro no fue encontrado." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al realizar eliminación lógica del centro con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar eliminación lógica del centro con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un centro por su ID
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCenter(int id)
        {
            try
            {
                var result = await _centerBusiness.DeleteCenterAsync(id);
                if (result)
                {
                    return Ok(new { message = "El centro fue eliminado exitosamente." });
                }
                return NotFound(new { message = "El centro no fue encontrado." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar centro con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar centro con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
