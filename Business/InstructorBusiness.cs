using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los instructores.
    /// Implementa la lógica de negocio para la gestión de instructores, incluyendo operaciones CRUD.
    /// </summary>
    internal class InstructorBusiness
    {
        // Dependencias inyectadas
        private readonly InstructorData _instructorData; // Acceso a la capa de datos
        private readonly ILogger _logger;               // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="instructorData">Servicio de acceso a datos para instructores</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public InstructorBusiness(InstructorData instructorData, ILogger logger)
        {
            _instructorData = instructorData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los instructores del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de instructores en formato DTO</returns>
        public async Task<IEnumerable<InstructorDTO>> GetAllInstructorsAsync()
        {
            try
            {
                // Obtener instructores de la capa de datos
                var instructors = await _instructorData.GetAllAsync();
                var instructorsDTO = new List<InstructorDTO>();

                // Convertir cada instructor a DTO
                foreach (var instructor in instructors)
                {
                    instructorsDTO.Add(new InstructorDTO
                    {
                        Id = instructor.Id,
                        UserId = instructor.UserId
                    });
                }

                return instructorsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los instructores");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de instructores", ex);
            }
        }

        /// <summary>
        /// Obtiene un instructor específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del instructor</param>
        /// <returns>Instructor en formato DTO</returns>
        public async Task<InstructorDTO> GetInstructorByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un instructor con ID inválido: {InstructorId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del instructor debe ser mayor que cero");
            }

            try
            {
                // Buscar el instructor en la base de datos
                var instructor = await _instructorData.GetByIdAsync(id);
                if (instructor == null)
                {
                    _logger.LogInformation("No se encontró ningún instructor con ID: {InstructorId}", id);
                    throw new EntityNotFoundException("Instructor", id);
                }

                // Convertir el instructor a DTO
                return new InstructorDTO
                {
                    Id = instructor.Id,
                    UserId = instructor.UserId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el instructor con ID: {InstructorId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el instructor con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo instructor en el sistema
        /// </summary>
        /// <param name="instructorDto">Datos del instructor a crear</param>
        /// <returns>Instructor creado en formato DTO</returns>
        public async Task<InstructorDTO> CreateInstructorAsync(InstructorDTO instructorDto)
        {
            
                // Validar los datos del DTO
                ValidateInstructor(instructorDto);

                // Crear la entidad Instructor desde el DTO
                var instructor = new Instructor
                {
                    UserId = instructorDto.UserId
                };

                // Guardar el instructor en la base de datos
                var instructorCreado = await _instructorData.CreateAsync(instructor);

                // Convertir el instructor creado a DTO para la respuesta
                return new InstructorDTO
                {
                    Id = instructor.Id,
                    UserId = instructor.UserId
                };
            
        }

        /// <summary>
        /// Valida los datos del DTO de instructor
        /// </summary>
        /// <param name="instructorDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateInstructor(InstructorDTO instructorDto)
        {
            // Validar que el DTO no sea nulo
            if (instructorDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto instructor no puede ser nulo");
            }            
        }
    }
}


