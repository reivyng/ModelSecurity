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
    /// Controlador para la gestión de empresas en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class EnterpriseController : ControllerBase
    {
        private readonly EnterpriseBusiness _enterpriseBusiness;
        private readonly ILogger<EnterpriseController> _logger;

        /// <summary>
        /// Constructor del controlador de empresas
        /// </summary>
        /// <param name="enterpriseBusiness">Capa de negocio de empresas</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public EnterpriseController(EnterpriseBusiness enterpriseBusiness, ILogger<EnterpriseController> logger)
        {
            _enterpriseBusiness = enterpriseBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las empresas del sistema
        /// </summary>
        /// <returns>Lista de empresas</returns>
        /// <response code="200">Retorna la lista de empresas</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EnterpriseDTO>), 200)]
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
        /// <param name="id">ID de la empresa</param>
        /// <returns>Empresa solicitada</returns>
        /// <response code="200">Retorna la empresa solicitada</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Empresa no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EnterpriseDTO), 200)]
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
        /// <param name="enterpriseDto">Datos de la empresa a crear</param>
        /// <returns>Empresa creada</returns>
        /// <response code="201">Retorna la empresa creada</response>
        /// <response code="400">Datos de la empresa no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(EnterpriseDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateEnterprise([FromBody] EnterpriseDTO enterpriseDto)
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
    }
}


