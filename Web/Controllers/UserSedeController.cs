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
    /// Controlador para la gestión de usuarios y sedes en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserSedeController : ControllerBase
    {
        private readonly UserSedeBusiness _userSedeBusiness;
        private readonly ILogger<UserSedeController> _logger;

        /// <summary>
        /// Constructor del controlador de usuarios y sedes
        /// </summary>
        /// <param name="userSedeBusiness">Capa de negocio de usuarios y sedes</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public UserSedeController(UserSedeBusiness userSedeBusiness, ILogger<UserSedeController> logger)
        {
            _userSedeBusiness = userSedeBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios y sedes del sistema
        /// </summary>
        /// <returns>Lista de usuarios y sedes</returns>
        /// <response code="200">Retorna la lista de usuarios y sedes</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserSedeDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllUserSedes()
        {
            try
            {
                var userSedes = await _userSedeBusiness.GetAllUserSedesAsync();
                return Ok(userSedes);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios y sedes");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un usuario o sede específico por su ID
        /// </summary>
        /// <param name="id">ID del usuario o sede</param>
        /// <returns>Usuario o sede solicitado</returns>
        /// <response code="200">Retorna el usuario o sede solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Usuario o sede no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserSedeDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUserSedeById(int id)
        {
            try
            {
                var userSede = await _userSedeBusiness.GetUserSedeByIdAsync(id);
                return Ok(userSede);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el usuario o sede con ID: {UserSedeId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario o sede no encontrado con ID: {UserSedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener usuario o sede con ID: {UserSedeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo usuario o sede en el sistema
        /// </summary>
        /// <param name="userSedeDto">Datos del usuario o sede a crear</param>
        /// <returns>Usuario o sede creado</returns>
        /// <response code="201">Retorna el usuario o sede creado</response>
        /// <response code="400">Datos del usuario o sede no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserSedeDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateUserSede([FromBody] UserSedeDTO userSedeDto)
        {
            try
            {
                var createdUserSede = await _userSedeBusiness.CreateUserSedeAsync(userSedeDto);
                return CreatedAtAction(nameof(GetUserSedeById), new { id = createdUserSede.Id }, createdUserSede);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear usuario o sede");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear usuario o sede");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
