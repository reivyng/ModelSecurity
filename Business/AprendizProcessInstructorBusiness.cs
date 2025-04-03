using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con el proceso de instructores de aprendices.
    /// Implementa la lógica de negocio para la gestión de procesos de instructores, incluyendo operaciones CRUD.
    /// </summary>
    internal class AprendizProcessInstructorBusiness
    {
        // Dependencias inyectadas
        private readonly AprendizProcessInstructorData _aprendizProcessInstructorData; // Acceso a la capa de datos
        private readonly ILogger _logger;                                             // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="aprendizProcessInstructorData">Servicio de acceso a datos para procesos de instructores de aprendices</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public AprendizProcessInstructorBusiness(AprendizProcessInstructorData aprendizProcessInstructorData, ILogger logger)
        {
            _aprendizProcessInstructorData = aprendizProcessInstructorData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los procesos de instructores de aprendices del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de procesos de instructores de aprendices en formato DTO</returns>
        public async Task<IEnumerable<AprendizProcessInstructorDTO>> GetAllAprendizProcessInstructorsAsync()
        {
            try
            {
                // Obtener procesos de instructores de aprendices de la capa de datos
                var aprendizProcessInstructors = await _aprendizProcessInstructorData.GetAllAsync();
                var aprendizProcessInstructorsDTO = new List<AprendizProcessInstructorDTO>();

                // Convertir cada proceso de instructor de aprendiz a DTO
                foreach (var aprendizProcessInstructor in aprendizProcessInstructors)
                {
                    aprendizProcessInstructorsDTO.Add(new AprendizProcessInstructorDTO
                    {
                        Id = aprendizProcessInstructor.Id,
                        InstructorId = aprendizProcessInstructor.InstructorId,
                        AprendizId = aprendizProcessInstructor.AprendizId,
                        RegisterySofiaId = aprendizProcessInstructor.RegisterySofiaId,
                        ConceptId = aprendizProcessInstructor.ConceptId,
                        EnterpriseId = aprendizProcessInstructor.EnterpriseId,
                        ProcessId = aprendizProcessInstructor.ProcessId,
                        TypeModalityId = aprendizProcessInstructor.TypeModalityId,
                        StadeId = aprendizProcessInstructor.StadeId,
                        VerificationId = aprendizProcessInstructor.VerificationId
                    });
                }

                return aprendizProcessInstructorsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los procesos de instructores de aprendices");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de procesos de instructores de aprendices", ex);
            }
        }

        /// <summary>
        /// Obtiene un proceso de instructor de aprendiz específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del proceso de instructor de aprendiz</param>
        /// <returns>Proceso de instructor de aprendiz en formato DTO</returns>
        public async Task<AprendizProcessInstructorDTO> GetAprendizProcessInstructorByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un proceso de instructor de aprendiz con ID inválido: {AprendizProcessInstructorId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del proceso de instructor de aprendiz debe ser mayor que cero");
            }

            try
            {
                // Buscar el proceso de instructor de aprendiz en la base de datos
                var aprendizProcessInstructor = await _aprendizProcessInstructorData.GetByIdAsync(id);
                if (aprendizProcessInstructor == null)
                {
                    _logger.LogInformation("No se encontró ningún proceso de instructor de aprendiz con ID: {AprendizProcessInstructorId}", id);
                    throw new EntityNotFoundException("AprendizProcessInstructor", id);
                }

                // Convertir el proceso de instructor de aprendiz a DTO
                return new AprendizProcessInstructorDTO
                {
                    Id = aprendizProcessInstructor.Id,
                    InstructorId = aprendizProcessInstructor.InstructorId,
                    AprendizId = aprendizProcessInstructor.AprendizId,
                    RegisterySofiaId = aprendizProcessInstructor.RegisterySofiaId,
                    ConceptId = aprendizProcessInstructor.ConceptId,
                    EnterpriseId = aprendizProcessInstructor.EnterpriseId,
                    ProcessId = aprendizProcessInstructor.ProcessId,
                    TypeModalityId = aprendizProcessInstructor.TypeModalityId,
                    StadeId = aprendizProcessInstructor.StadeId,
                    VerificationId = aprendizProcessInstructor.VerificationId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el proceso de instructor de aprendiz con ID: {AprendizProcessInstructorId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el proceso de instructor de aprendiz con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo proceso de instructor de aprendiz en el sistema
        /// </summary>
        /// <param name="aprendizProcessInstructorDto">Datos del proceso de instructor de aprendiz a crear</param>
        /// <returns>Proceso de instructor de aprendiz creado en formato DTO</returns>
        public async Task<AprendizProcessInstructorDTO> CreateAprendizProcessInstructorAsync(AprendizProcessInstructorDTO aprendizProcessInstructorDto)
        {
            
                // Validar los datos del DTO
                ValidateAprendizProcessInstructor(aprendizProcessInstructorDto);

                // Crear la entidad AprendizProcessInstructor desde el DTO
                var aprendizProcessInstructor = new AprendizProcessInstructor
                {                    
                    InstructorId = aprendizProcessInstructorDto.InstructorId,
                    AprendizId = aprendizProcessInstructorDto.AprendizId,
                    RegisterySofiaId = aprendizProcessInstructorDto.RegisterySofiaId,
                    ConceptId = aprendizProcessInstructorDto.ConceptId,
                    EnterpriseId = aprendizProcessInstructorDto.EnterpriseId,
                    ProcessId = aprendizProcessInstructorDto.ProcessId,
                    TypeModalityId = aprendizProcessInstructorDto.TypeModalityId,
                    StadeId = aprendizProcessInstructorDto.StadeId,
                    VerificationId = aprendizProcessInstructorDto.VerificationId
                };

                // Guardar el proceso de instructor de aprendiz en la base de datos
                var aprendizProcessInstructorCreado = await _aprendizProcessInstructorData.CreateAsync(aprendizProcessInstructor);

                // Convertir el proceso de instructor de aprendiz creado a DTO para la respuesta
                return new AprendizProcessInstructorDTO
                {
                    Id = aprendizProcessInstructor.Id,
                    InstructorId = aprendizProcessInstructor.InstructorId,
                    AprendizId = aprendizProcessInstructor.AprendizId,
                    RegisterySofiaId = aprendizProcessInstructor.RegisterySofiaId,
                    ConceptId = aprendizProcessInstructor.ConceptId,
                    EnterpriseId = aprendizProcessInstructor.EnterpriseId,
                    ProcessId = aprendizProcessInstructor.ProcessId,
                    TypeModalityId = aprendizProcessInstructor.TypeModalityId,
                    StadeId = aprendizProcessInstructor.StadeId,
                    VerificationId = aprendizProcessInstructor.VerificationId
                };
            
            
        }

        /// <summary>
        /// Valida los datos del DTO de proceso de instructor de aprendiz
        /// </summary>
        /// <param name="aprendizProcessInstructorDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateAprendizProcessInstructor(AprendizProcessInstructorDTO aprendizProcessInstructorDto)
        {
            // Validar que el DTO no sea nulo
            if (aprendizProcessInstructorDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto proceso de instructor de aprendiz no puede ser nulo");
            }

            // Validar que el InstructorId no esté vacío
            if (aprendizProcessInstructorDto.InstructorId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un proceso de instructor de aprendiz con InstructorId inválido");
                throw new Utilities.Exceptions.ValidationException("InstructorId", "El InstructorId del proceso de instructor de aprendiz es obligatorio");
            }

            // Validar que el AprendizId no esté vacío
            if (aprendizProcessInstructorDto.AprendizId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un proceso de instructor de aprendiz con AprendizId inválido");
                throw new Utilities.Exceptions.ValidationException("AprendizId", "El AprendizId del proceso de instructor de aprendiz es obligatorio");
            }
        }
    }
}
