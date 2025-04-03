using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los registros de Stade.
    /// Implementa la lógica de negocio para la gestión de registros de Stade, incluyendo operaciones CRUD.
    /// </summary>
    internal class StadeBusiness
    {
        // Dependencias inyectadas
        private readonly StadeData _stadeData; // Acceso a la capa de datos
        private readonly ILogger _logger;     // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="stadeData">Servicio de acceso a datos para registros de Stade</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public StadeBusiness(StadeData stadeData, ILogger logger)
        {
            _stadeData = stadeData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de Stade del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de registros de Stade en formato DTO</returns>
        public async Task<IEnumerable<StadeDTO>> GetAllStadesAsync()
        {
            try
            {
                // Obtener registros de Stade de la capa de datos
                var stades = await _stadeData.GetAllAsync();
                var stadesDTO = new List<StadeDTO>();

                // Convertir cada registro de Stade a DTO
                foreach (var stade in stades)
                {
                    stadesDTO.Add(new StadeDTO
                    {
                        Id = stade.Id,
                        Description = stade.Description,
                        TypeStade = stade.TypeStade
                    });
                }

                return stadesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de Stade");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de registros de Stade", ex);
            }
        }

        /// <summary>
        /// Obtiene un registro de Stade específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del registro de Stade</param>
        /// <returns>Registro de Stade en formato DTO</returns>
        public async Task<StadeDTO> GetStadeByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un registro de Stade con ID inválido: {StadeId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del registro de Stade debe ser mayor que cero");
            }

            try
            {
                // Buscar el registro de Stade en la base de datos
                var stade = await _stadeData.GetByIdAsync(id);
                if (stade == null)
                {
                    _logger.LogInformation("No se encontró ningún registro de Stade con ID: {StadeId}", id);
                    throw new EntityNotFoundException("Stade", id);
                }

                // Convertir el registro de Stade a DTO
                return new StadeDTO
                {
                    Id = stade.Id,
                    Description = stade.Description,
                    TypeStade = stade.TypeStade
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro de Stade con ID: {StadeId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el registro de Stade con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo registro de Stade en el sistema
        /// </summary>
        /// <param name="stadeDto">Datos del registro de Stade a crear</param>
        /// <returns>Registro de Stade creado en formato DTO</returns>
        public async Task<StadeDTO> CreateStadeAsync(StadeDTO stadeDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateStade(stadeDto);

                // Crear la entidad Stade desde el DTO
                var stade = new Stade
                {
                    Description = stadeDto.Description,
                    TypeStade = stadeDto.TypeStade
                };

                // Guardar el registro de Stade en la base de datos
                var stadeCreado = await _stadeData.CreateAsync(stade);

                // Convertir el registro de Stade creado a DTO para la respuesta
                return new StadeDTO
                {
                    Id = stade.Id,
                    Description = stade.Description,
                    TypeStade = stade.TypeStade
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo registro de Stade: {StadeDescription}", stadeDto?.Description ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el registro de Stade", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de registro de Stade
        /// </summary>
        /// <param name="stadeDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateStade(StadeDTO stadeDto)
        {
            // Validar que el DTO no sea nulo
            if (stadeDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto registro de Stade no puede ser nulo");
            }

            // Validar que el Description no esté vacío
            if (string.IsNullOrWhiteSpace(stadeDto.Description))
            {
                _logger.LogWarning("Se intentó crear/actualizar un registro de Stade con Description vacío");
                throw new Utilities.Exceptions.ValidationException("Description", "El Description del registro de Stade es obligatorio");
            }
        }
    }
}






