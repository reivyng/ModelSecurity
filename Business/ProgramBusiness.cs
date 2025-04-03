using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los programas.
    /// Implementa la lógica de negocio para la gestión de programas, incluyendo operaciones CRUD.
    /// </summary>
    internal class ProgramBusiness
    {
        // Dependencias inyectadas
        private readonly ProgramData _programData; // Acceso a la capa de datos
        private readonly ILogger _logger;         // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="programData">Servicio de acceso a datos para programas</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public ProgramBusiness(ProgramData programData, ILogger logger)
        {
            _programData = programData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los programas del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de programas en formato DTO</returns>
        public async Task<IEnumerable<ProgramDTO>> GetAllProgramsAsync()
        {
            try
            {
                // Obtener programas de la capa de datos
                var programs = await _programData.GetAllAsync();
                var programsDTO = new List<ProgramDTO>();

                // Convertir cada programa a DTO
                foreach (var program in programs)
                {
                    programsDTO.Add(new ProgramDTO
                    {
                        Id = program.Id,
                        CodeProgram = program.Code_Program,
                        Name = program.Name,
                        TypeProgram = program.Type_Program,
                        Description = program.Description
                    });
                }

                return programsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los programas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de programas", ex);
            }
        }

        /// <summary>
        /// Obtiene un programa específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del programa</param>
        /// <returns>Programa en formato DTO</returns>
        public async Task<ProgramDTO> GetProgramByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un programa con ID inválido: {ProgramId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del programa debe ser mayor que cero");
            }

            try
            {
                // Buscar el programa en la base de datos
                var program = await _programData.GetByIdAsync(id);
                if (program == null)
                {
                    _logger.LogInformation("No se encontró ningún programa con ID: {ProgramId}", id);
                    throw new EntityNotFoundException("Program", id);
                }

                // Convertir el programa a DTO
                return new ProgramDTO
                {
                    Id = program.Id,
                    CodeProgram = program.Code_Program,
                    Name = program.Name,
                    TypeProgram = program.Type_Program,
                    Description = program.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el programa con ID: {ProgramId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el programa con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo programa en el sistema
        /// </summary>
        /// <param name="programDto">Datos del programa a crear</param>
        /// <returns>Programa creado en formato DTO</returns>
        public async Task<ProgramDTO> CreateProgramAsync(ProgramDTO programDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateProgram(programDto);

                // Crear la entidad Program desde el DTO
                var program = new Program
                {
                    Code_Program = programDto.CodeProgram,
                    Name = programDto.Name,
                    Type_Program = programDto.TypeProgram,
                    Description = programDto.Description
                };

                // Guardar el programa en la base de datos
                var programCreado = await _programData.CreateAsync(program);

                // Convertir el programa creado a DTO para la respuesta
                return new ProgramDTO
                {
                    Id = program.Id,
                    CodeProgram = program.Code_Program,
                    Name = program.Name,
                    TypeProgram = program.Type_Program,
                    Description = program.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo programa: {ProgramName}", programDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el programa", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de programa
        /// </summary>
        /// <param name="programDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateProgram(ProgramDTO programDto)
        {
            // Validar que el DTO no sea nulo
            if (programDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto programa no puede ser nulo");
            }

            // Validar que el Name no esté vacío
            if (string.IsNullOrWhiteSpace(programDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un programa con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del programa es obligatorio");
            }
        }
    }
}



