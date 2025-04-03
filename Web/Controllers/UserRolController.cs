﻿using Business;
using Data;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de relaciones entre roles y usuarios en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserRolController : ControllerBase
    {
        private readonly RolUserBusiness _RolUserBusiness;
        private readonly ILogger<UserRolController> _logger;

        /// <summary>
        /// Constructor del controlador de relaciones entre roles y usuarios
        /// </summary>
        /// <param name="RolUserBusiness">Capa de negocio de relaciones entre roles y usuarios</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public UserRolController(RolUserBusiness RolUserBusiness, ILogger<UserRolController> logger)
        {
            _RolUserBusiness = RolUserBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las relaciones de roles y usuarios
        /// </summary>
        /// <returns>Lista de relaciones</returns>
        /// <response code="200">Retorna la lista de relaciones</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolUserDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRolUsers()
        {
            try
            {
                var rolUsers = await _RolUserBusiness.GetAllRolUsersAsync();
                return Ok(rolUsers);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener relaciones de roles y usuarios");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una relación específica por su ID
        /// </summary>
        /// <param name="id">ID de la relación</param>
        /// <returns>Relación solicitada</returns>
        /// <response code="200">Retorna la relación solicitada</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Relación no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolUserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolUserById(int id)
        {
            try
            {
                var rolUser = await _RolUserBusiness.GetRolUserByIdAsync(id);
                return Ok(rolUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la relación con ID: {RolUserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Relación no encontrada con ID: {RolUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener la relación con ID: {RolUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva relación entre rol y usuario
        /// </summary>
        /// <param name="RolUserDto">Datos de la relación a crear</param>
        /// <returns>Relación creada</returns>
        /// <response code="201">Retorna la relación creada</response>
        /// <response code="400">Datos de la relación no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RolUserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRolUser([FromBody] RolUserDto RolUserDto)
        {
            try
            {
                var createdRolUser = await _RolUserBusiness.CreateRolUserAsync(RolUserDto);
                return CreatedAtAction(nameof(GetRolUserById), new { id = createdRolUser.Id }, createdRolUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear la relación rol-usuario");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear la relación rol-usuario");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
