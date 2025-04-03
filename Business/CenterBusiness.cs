using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los centros.
    /// Implementa la lógica de negocio para la gestión de centros, incluyendo operaciones CRUD.
    /// </summary>
    public class CenterBusiness
    {
        // Dependencias inyectadas
        private readonly CenterData _centerData; // Acceso a la capa de datos
        private readonly ILogger _logger;       // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="centerData">Servicio de acceso a datos para centros</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public CenterBusiness(CenterData centerData, ILogger logger)
        {
            _centerData = centerData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los centros del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de centros en formato DTO</returns>
        public async Task<IEnumerable<CenterDTO>> GetAllCentersAsync()
        {
            try
            {
                // Obtener centros de la capa de datos
                var centers = await _centerData.GetAllAsync();
                var centersDTO = new List<CenterDTO>();

                // Convertir cada centro a DTO
                foreach (var center in centers)
                {
                    centersDTO.Add(new CenterDTO
                    {
                        Id = center.Id,
                        Name = center.Name,
                        CodeCenter = center.CodeCenter
                    });
                }

                return centersDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los centros");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de centros", ex);
            }
        }

        /// <summary>
        /// Obtiene un centro específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del centro</param>
        /// <returns>Centro en formato DTO</returns>
        public async Task<CenterDTO> GetCenterByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un centro con ID inválido: {CenterId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del centro debe ser mayor que cero");
            }

            try
            {
                // Buscar el centro en la base de datos
                var center = await _centerData.GetByIdAsync(id);
                if (center == null)
                {
                    _logger.LogInformation("No se encontró ningún centro con ID: {CenterId}", id);
                    throw new EntityNotFoundException("Center", id);
                }

                // Convertir el centro a DTO
                return new CenterDTO
                {
                    Id = center.Id,
                    Name = center.Name,
                    CodeCenter = center.CodeCenter
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el centro con ID: {CenterId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el centro con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo centro en el sistema
        /// </summary>
        /// <param name="centerDto">Datos del centro a crear</param>
        /// <returns>Centro creado en formato DTO</returns>
        public async Task<CenterDTO> CreateCenterAsync(CenterDTO centerDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateCenter(centerDto);

                // Crear la entidad Center desde el DTO
                var center = new Center
                {
                    Name = centerDto.Name,
                    CodeCenter = centerDto.CodeCenter
                };

                // Guardar el centro en la base de datos
                var centerCreado = await _centerData.CreateAsync(center);

                // Convertir el centro creado a DTO para la respuesta
                return new CenterDTO
                {
                    Id = center.Id,
                    Name = center.Name,
                    CodeCenter = center.CodeCenter
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo centro: {CenterName}", centerDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el centro", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de centro
        /// </summary>
        /// <param name="centerDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateCenter(CenterDTO centerDto)
        {
            // Validar que el DTO no sea nulo
            if (centerDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto centro no puede ser nulo");
            }

            // Validar que el Name no esté vacío
            if (string.IsNullOrWhiteSpace(centerDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un centro con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del centro es obligatorio");
            }
        }
    }
}
