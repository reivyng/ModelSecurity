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
    /// Controlador para la gestión de Enterprise en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class EnterpriseController : ControllerBase
    {
        private readonly EnterpriseBusiness _enterpriseBusiness;
        private readonly ILogger<EnterpriseController> _logger;

        /// <summary>
        /// Constructor del controlador de Enterprise
        /// </summary>
        public EnterpriseController(EnterpriseBusiness enterpriseBusiness, ILogger<EnterpriseController> logger)
        {
            _enterpriseBusiness = enterpriseBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las empresas del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EnterpriseDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllEnterprises()
        {
            try
            {
                var enterprises = await _enterpriseBusiness.GetAllEnterprisesAsync();
                return Ok(enterprises);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener empresas");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una empresa específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EnterpriseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetEnterpriseById(int id)
        {
            try
            {
                var enterprise = await _enterpriseBusiness.GetEnterpriseByIdAsync(id);
                return Ok(enterprise);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la empresa con ID: {EnterpriseId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Empresa no encontrada con ID: {EnterpriseId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener empresa con ID: {EnterpriseId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva empresa en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(EnterpriseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateEnterprise([FromBody] EnterpriseDto enterpriseDto)
        {
            try
            {
                var createdEnterprise = await _enterpriseBusiness.CreateEnterpriseAsync(enterpriseDto);
                return CreatedAtAction(nameof(GetEnterpriseById), new { id = createdEnterprise.Id }, createdEnterprise);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear empresa");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear empresa");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una empresa existente
        /// </summary>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateEnterprise([FromBody] EnterpriseDto enterpriseDto)
        {
            try
            {
                var result = await _enterpriseBusiness.UpdateEnterpriseAsync(enterpriseDto);
                if (result)
                {
                    return Ok(new { message = "La empresa fue actualizada exitosamente." });
                }
                return NotFound(new { message = "La empresa no fue encontrada." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar empresa");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar empresa");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente una empresa existente
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialEnterprise(int id, [FromBody] EnterpriseDto updatedFields)
        {
            try
            {
                var result = await _enterpriseBusiness.UpdatePartialEnterpriseAsync(id, updatedFields);
                if (result)
                {
                    return Ok(new { message = "La empresa fue actualizada parcialmente con éxito." });
                }
                return NotFound(new { message = "La empresa no fue encontrada." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente empresa con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente empresa con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza una eliminación lógica de una empresa
        /// </summary>
        [HttpDelete("soft/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteEnterprise(int id)
        {
            try
            {
                var result = await _enterpriseBusiness.SoftDeleteEnterpriseAsync(id);
                if (result)
                {
                    return Ok(new { message = "La empresa fue eliminada lógicamente con éxito." });
                }
                return NotFound(new { message = "La empresa no fue encontrada." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al realizar eliminación lógica de la empresa con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar eliminación lógica de la empresa con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una empresa por su ID
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteEnterprise(int id)
        {
            try
            {
                var result = await _enterpriseBusiness.DeleteEnterpriseAsync(id);
                if (result)
                {
                    return Ok(new { message = "La empresa fue eliminada exitosamente." });
                }
                return NotFound(new { message = "La empresa no fue encontrada." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar empresa con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar empresa con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}