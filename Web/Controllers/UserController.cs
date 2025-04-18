using Business;
using Data;
using Entity.DTOautogestion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de usuarios en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly UserBusiness _UserBusiness;
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Constructor del controlador de usuarios
        /// </summary>
        public UserController(UserBusiness UserBusiness, ILogger<UserController> logger)
        {
            _UserBusiness = UserBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _UserBusiness.GetAllUsersAsync();
                return Ok(users);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un usuario específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _UserBusiness.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el usuario con ID: {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado con ID: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo usuario en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateUser([FromBody] UserDto UserDto)
        {
            try
            {
                var createdUser = await _UserBusiness.CreateUserAsync(UserDto);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear usuario");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            if (id != userDto.Id)
            {
                return BadRequest(new { message = "El ID del usuario no coincide con el ID proporcionado en el cuerpo de la solicitud" });
            }

            try
            {
                var result = await _UserBusiness.UpdateUserAsync(userDto);
                if (!result)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                // Retornar un mensaje indicando que el registro fue actualizado correctamente
                return Ok(new { message = "Registro actualizado correctamente" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar usuario con ID: {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }



        /// <summary>
        /// Actualiza campos específicos de un usuario
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialUser(int id, [FromBody] UserDto updatedFields)
        {
            try
            {
                var result = await _UserBusiness.UpdatePartialUserAsync(id, updatedFields);
                if (!result)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                // Retornar un mensaje indicando que la actualización parcial fue exitosa
                return Ok(new { message = "Campos actualizados correctamente" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente usuario con ID: {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza una eliminación lógica de un usuario (marcar como inactivo)
        /// </summary>
        [HttpDelete("soft/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteUser(int id)
        {
            try
            {
                var result = await _UserBusiness.SoftDeleteUserAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                // Retornar un mensaje indicando que la eliminación lógica fue exitosa
                return Ok(new { message = "Usuario marcado como inactivo correctamente" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al realizar eliminación lógica del usuario con ID: {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar eliminación lógica del usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un usuario por su ID
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _UserBusiness.DeleteUserAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                // Retornar un mensaje indicando que el usuario fue eliminado correctamente
                return Ok(new { message = "Usuario eliminado correctamente" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar usuario con ID: {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}

