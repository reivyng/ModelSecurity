﻿using Business;
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
    /// Controlador para la gestión de módulos en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ModuleController : ControllerBase
    {
        private readonly ModuleBusiness _ModuleBusiness;
        private readonly ILogger<ModuleController> _logger;

        /// <summary>
        /// Constructor del controlador de módulos
        /// </summary>
        /// <param name="moduleBusiness">Capa de negocio de módulos</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public ModuleController(ModuleBusiness moduleBusiness, ILogger<ModuleController> logger)
        {
            _ModuleBusiness = moduleBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los módulos del sistema
        /// </summary>
        /// <returns>Lista de módulos</returns>
        /// <response code="200">Retorna la lista de módulos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ModuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllModules()
        {
            try
            {
                var modules = await _ModuleBusiness.GetAllModulesAsync();
                return Ok(modules);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener módulos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un módulo específico por su ID
        /// </summary>
        /// <param name="id">ID del módulo</param>
        /// <returns>Módulo solicitado</returns>
        /// <response code="200">Retorna el módulo solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Módulo no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetModuleById(int id)
        {
            try
            {
                var module = await _ModuleBusiness.GetModuleByIdAsync(id);
                return Ok(module);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el módulo con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener módulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo módulo en el sistema
        /// </summary>
        /// <param name="moduleDto">Datos del módulo a crear</param>
        /// <returns>Módulo creado</returns>
        /// <response code="201">Retorna el módulo creado</response>
        /// <response code="400">Datos del módulo no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ModuleDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateModule([FromBody] ModuleDto moduleDto)
        {
            try
            {
                var createdModule = await _ModuleBusiness.CreateModuleAsync(moduleDto);
                return CreatedAtAction(nameof(GetModuleById), new { id = createdModule.Id }, createdModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear módulo");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear módulo");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un módulo existente
        /// </summary>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateModule([FromBody] ModuleDto moduleDto)
        {
            try
            {
                var result = await _ModuleBusiness.UpdateModuleAsync(moduleDto);
                if (result)
                {
                    return Ok(new { message = "El módulo fue actualizado exitosamente." });
                }
                return NotFound(new { message = "El módulo no fue encontrado." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar módulo");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar módulo");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un módulo existente
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialModule(int id, [FromBody] ModuleDto updatedFields)
        {
            try
            {
                var result = await _ModuleBusiness.UpdatePartialModuleAsync(id, updatedFields);
                if (result)
                {
                    return Ok(new { message = "El módulo fue actualizado parcialmente con éxito." });
                }
                return NotFound(new { message = "El módulo no fue encontrado." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente módulo con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente módulo con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza una eliminación lógica de un módulo
        /// </summary>
        [HttpDelete("soft/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteModule(int id)
        {
            try
            {
                var result = await _ModuleBusiness.SoftDeleteModuleAsync(id);
                if (result)
                {
                    return Ok(new { message = "El módulo fue eliminado lógicamente con éxito." });
                }
                return NotFound(new { message = "El módulo no fue encontrado." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al realizar eliminación lógica del módulo con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar eliminación lógica del módulo con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un módulo por su ID
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteModule(int id)
        {
            try
            {
                var result = await _ModuleBusiness.DeleteModuleAsync(id);
                if (result)
                {
                    return Ok(new { message = "El módulo fue eliminado exitosamente." });
                }
                return NotFound(new { message = "El módulo no fue encontrado." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar módulo con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar módulo con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


    }
}
