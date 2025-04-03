using Data;
using Entity.DTOautogestion;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los programas de instructores.
    /// Implementa la lógica de negocio para la gestión de programas de instructores, incluyendo operaciones CRUD.
    /// </summary>
    internal class InstructorProgramBusiness
    {
        // Dependencias inyectadas
        private readonly InstructorProgramData _instructorProgramData; // Acceso a la capa de datos
        private readonly ILogger _logger;                             // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="instructorProgramData">Servicio de acceso a datos para programas de instructores</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public InstructorProgramBusiness(InstructorProgramData instructorProgramData, ILogger logger)
        {
            _instructorProgramData = instructorProgramData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los programas de instructores del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de programas de instructores en formato DTO</returns>
        public async Task<IEnumerable<InstructorProgramDTO>> GetAllInstructorProgramsAsync()
        {
            try
            {
                // Obtener programas de instructores de la capa de datos
                var instructorPrograms = await _instructorProgramData.GetAllAsync();
                var instructorProgramsDTO = new List<InstructorProgramDTO>();

                // Convertir cada programa de instructor a DTO
                foreach (var instructorProgram in instructorPrograms)
                {
                    instructorProgramsDTO.Add(new InstructorProgramDTO
                    {
                        Id = instructorProgram.Id,
                        InstructorId = instructorProgram.InstructorId,
                        ProgramId = instructorProgram.ProgramId
                    });
                }

                return instructorProgramsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los programas de instructores");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de programas de instructores", ex);
            }
        }

        /// <summary>
        /// Obtiene un programa de instructor específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del programa de instructor</param>
        /// <returns>Programa de instructor en formato DTO</returns>
        public async Task<InstructorProgramDTO> GetInstructorProgramByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un programa de instructor con ID inválido: {InstructorProgramId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del programa de instructor debe ser mayor que cero");
            }

            try
            {
                // Buscar el programa de instructor en la base de datos
                var instructorProgram = await _instructorProgramData.GetByIdAsync(id);
                if (instructorProgram == null)
                {
                    _logger.LogInformation("No se encontró ningún programa de instructor con ID: {InstructorProgramId}", id);
                    throw new EntityNotFoundException("InstructorProgram", id);
                }

                // Convertir el programa de instructor a DTO
                return new InstructorProgramDTO
                {
                    Id = instructorProgram.Id,
                    InstructorId = instructorProgram.InstructorId,
                    ProgramId = instructorProgram.ProgramId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el programa de instructor con ID: {InstructorProgramId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el programa de instructor con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo programa de instructor en el sistema
        /// </summary>
        /// <param name="instructorProgramDto">Datos del programa de instructor a crear</param>
        /// <returns>Programa de instructor creado en formato DTO</returns>
        public async Task<InstructorProgramDTO> CreateInstructorProgramAsync(InstructorProgramDTO instructorProgramDto)
        {
            
                // Validar los datos del DTO
                ValidateInstructorProgram(instructorProgramDto);

                // Crear la entidad InstructorProgram desde el DTO
                var instructorProgram = new InstructorProgram
                {
                    InstructorId = instructorProgramDto.InstructorId,
                    ProgramId = instructorProgramDto.ProgramId
                };

                // Guardar el programa de instructor en la base de datos
                var instructorProgramCreado = await _instructorProgramData.CreateAsync(instructorProgram);

                // Convertir el programa de instructor creado a DTO para la respuesta
                return new InstructorProgramDTO
                {
                    Id = instructorProgram.Id,
                    InstructorId = instructorProgram.InstructorId,
                    ProgramId = instructorProgram.ProgramId
                };                
        }

        /// <summary>
        /// Valida los datos del DTO de programa de instructor
        /// </summary>
        /// <param name="instructorProgramDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateInstructorProgram(InstructorProgramDTO instructorProgramDto)
        {
            // Validar que el DTO no sea nulo
            if (instructorProgramDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto programa de instructor no puede ser nulo");
            }

            // Validar que el InstructorId no esté vacío
            if (instructorProgramDto.InstructorId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un programa de instructor con InstructorId inválido");
                throw new Utilities.Exceptions.ValidationException("InstructorId", "El InstructorId del programa de instructor es obligatorio");
            }

            // Validar que el ProgramId no esté vacío
            if (instructorProgramDto.ProgramId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un programa de instructor con ProgramId inválido");
                throw new Utilities.Exceptions.ValidationException("ProgramId", "El ProgramId del programa de instructor es obligatorio");
            }
        }
    }
}


