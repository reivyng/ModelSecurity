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
    /// Controlador para la gestión de programas en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProgramController : ControllerBase
    {
        private readonly ProgramBusiness _programBusiness;
        private readonly ILogger<ProgramController> _logger;

        /// <summary>
        /// Constructor del controlador de programas
        /// </summary>
        /// <param name="programBusiness">Capa de negocio de programas</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public ProgramController(ProgramBusiness programBusiness, ILogger<ProgramController> logger)
        {
            _programBusiness = programBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los programas del sistema
        /// </summary>
        /// <returns>Lista de programas</returns>
        /// <response code="200">Retorna la lista de programas</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProgramDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPrograms()
        {
            try
            {
                var programs = await _programBusiness.GetAllProgramsAsync();
                return Ok(programs);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener programas");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un programa específico por su ID
        /// </summary>
        /// <param name="id">ID del programa</param>
        /// <returns>Programa solicitado</returns>
        /// <response code="200">Retorna el programa solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Programa no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProgramDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProgramById(int id)
        {
            try
            {
                var program = await _programBusiness.GetProgramByIdAsync(id);
                return Ok(program);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el programa con ID: {ProgramId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Programa no encontrado con ID: {ProgramId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener programa con ID: {ProgramId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo programa en el sistema
        /// </summary>
        /// <param name="programDto">Datos del programa a crear</param>
        /// <returns>Programa creado</returns>
        /// <response code="201">Retorna el programa creado</response>
        /// <response code="400">Datos del programa no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProgramDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateProgram([FromBody] ProgramDTO programDto)
        {
            try
            {
                var createdProgram = await _programBusiness.CreateProgramAsync(programDto);
                return CreatedAtAction(nameof(GetProgramById), new { id = createdProgram.Id }, createdProgram);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear programa");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear programa");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}




