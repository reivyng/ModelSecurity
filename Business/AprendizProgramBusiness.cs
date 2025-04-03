using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los programas de aprendices.
    /// Implementa la lógica de negocio para la gestión de programas de aprendices, incluyendo operaciones CRUD.
    /// </summary>
    public class AprendizProgramBusiness
    {
        // Dependencias inyectadas
        private readonly AprendizProgramData _aprendizProgramData; // Acceso a la capa de datos
        private readonly ILogger _logger;                         // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="aprendizProgramData">Servicio de acceso a datos para programas de aprendices</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public AprendizProgramBusiness(AprendizProgramData aprendizProgramData, ILogger logger)
        {
            _aprendizProgramData = aprendizProgramData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los programas de aprendices del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de programas de aprendices en formato DTO</returns>
        public async Task<IEnumerable<AprendizProgramDTO>> GetAllAprendizProgramsAsync()
        {
            try
            {
                // Obtener programas de aprendices de la capa de datos
                var aprendizPrograms = await _aprendizProgramData.GetAllAsync();
                var aprendizProgramsDTO = new List<AprendizProgramDTO>();

                // Convertir cada programa de aprendiz a DTO
                foreach (var aprendizProgram in aprendizPrograms)
                {
                    aprendizProgramsDTO.Add(new AprendizProgramDTO
                    {
                        Id = aprendizProgram.Id,
                        AprendizId = aprendizProgram.AprendizId,
                        ProgramId = aprendizProgram.ProgramId
                    });
                }

                return aprendizProgramsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los programas de aprendices");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de programas de aprendices", ex);
            }
        }

        /// <summary>
        /// Obtiene un programa de aprendiz específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del programa de aprendiz</param>
        /// <returns>Programa de aprendiz en formato DTO</returns>
        public async Task<AprendizProgramDTO> GetAprendizProgramByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un programa de aprendiz con ID inválido: {AprendizProgramId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del programa de aprendiz debe ser mayor que cero");
            }

            try
            {
                // Buscar el programa de aprendiz en la base de datos
                var aprendizProgram = await _aprendizProgramData.GetByIdAsync(id);
                if (aprendizProgram == null)
                {
                    _logger.LogInformation("No se encontró ningún programa de aprendiz con ID: {AprendizProgramId}", id);
                    throw new EntityNotFoundException("AprendizProgram", id);
                }

                // Convertir el programa de aprendiz a DTO
                return new AprendizProgramDTO
                {
                    Id = aprendizProgram.Id,
                    AprendizId = aprendizProgram.AprendizId,
                    ProgramId = aprendizProgram.ProgramId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el programa de aprendiz con ID: {AprendizProgramId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el programa de aprendiz con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo programa de aprendiz en el sistema
        /// </summary>
        /// <param name="aprendizProgramDto">Datos del programa de aprendiz a crear</param>
        /// <returns>Programa de aprendiz creado en formato DTO</returns>
        public async Task<AprendizProgramDTO> CreateAprendizProgramAsync(AprendizProgramDTO aprendizProgramDto)
        {
           
                // Validar los datos del DTO
                ValidateAprendizProgram(aprendizProgramDto);

                // Crear la entidad AprendizProgram desde el DTO
                var aprendizProgram = new AprendizProgram
                {
                    AprendizId = aprendizProgramDto.AprendizId,
                    ProgramId = aprendizProgramDto.ProgramId
                };

                // Guardar el programa de aprendiz en la base de datos
                var aprendizProgramCreado = await _aprendizProgramData.CreateAsync(aprendizProgram);

                // Convertir el programa de aprendiz creado a DTO para la respuesta
                return new AprendizProgramDTO
                {
                    Id = aprendizProgram.Id,
                    AprendizId = aprendizProgram.AprendizId,
                    ProgramId = aprendizProgram.ProgramId
                };
            
            
        }

        /// <summary>
        /// Valida los datos del DTO de programa de aprendiz
        /// </summary>
        /// <param name="aprendizProgramDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateAprendizProgram(AprendizProgramDTO aprendizProgramDto)
        {
            // Validar que el DTO no sea nulo
            if (aprendizProgramDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto programa de aprendiz no puede ser nulo");
            } 
        }
    }
}
