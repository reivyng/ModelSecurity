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
    /// Controlador para la gestión de registros de Sofia en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RegisterySofiaController : ControllerBase
    {
        private readonly RegisterySofiaBusiness _registerySofiaBusiness;
        private readonly ILogger<RegisterySofiaController> _logger;

        /// <summary>
        /// Constructor del controlador de registros de Sofia
        /// </summary>
        /// <param name="registerySofiaBusiness">Capa de negocio de registros de Sofia</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public RegisterySofiaController(RegisterySofiaBusiness registerySofiaBusiness, ILogger<RegisterySofiaController> logger)
        {
            _registerySofiaBusiness = registerySofiaBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de Sofia del sistema
        /// </summary>
        /// <returns>Lista de registros de Sofia</returns>
        /// <response code="200">Retorna la lista de registros de Sofia</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RegisterySofiaDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRegisterySofias()
        {
            try
            {
                var registerySofias = await _registerySofiaBusiness.GetAllRegisterySofiasAsync();
                return Ok(registerySofias);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener registros de Sofia");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un registro de Sofia específico por su ID
        /// </summary>
        /// <param name="id">ID del registro de Sofia</param>
        /// <returns>Registro de Sofia solicitado</returns>
        /// <response code="200">Retorna el registro de Sofia solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Registro de Sofia no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RegisterySofiaDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRegisterySofiaById(int id)
        {
            try
            {
                var registerySofia = await _registerySofiaBusiness.GetRegisterySofiaByIdAsync(id);
                return Ok(registerySofia);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el registro de Sofia con ID: {RegisterySofiaId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro de Sofia no encontrado con ID: {RegisterySofiaId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener registro de Sofia con ID: {RegisterySofiaId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo registro de Sofia en el sistema
        /// </summary>
        /// <param name="registerySofiaDto">Datos del registro de Sofia a crear</param>
        /// <returns>Registro de Sofia creado</returns>
        /// <response code="201">Retorna el registro de Sofia creado</response>
        /// <response code="400">Datos del registro de Sofia no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RegisterySofiaDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRegisterySofia([FromBody] RegisterySofiaDTO registerySofiaDto)
        {
            try
            {
                var createdRegisterySofia = await _registerySofiaBusiness.CreateRegisterySofiaAsync(registerySofiaDto);
                return CreatedAtAction(nameof(GetRegisterySofiaById), new { id = createdRegisterySofia.Id }, createdRegisterySofia);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear registro de Sofia");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear registro de Sofia");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}






