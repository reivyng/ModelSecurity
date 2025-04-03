using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los registros de cambios del sistema.
    /// Implementa la lógica de negocio para la gestión de registros de cambios, incluyendo operaciones CRUD.
    /// </summary>
    public class ChangeLogBusiness
    {
        // Dependencias inyectadas
        private readonly ChangeLogData _changeLogData; // Acceso a la capa de datos
        private readonly ILogger _logger;             // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="changeLogData">Servicio de acceso a datos para registros de cambios</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public ChangeLogBusiness(ChangeLogData changeLogData, ILogger logger)
        {
            _changeLogData = changeLogData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de cambios del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de registros de cambios en formato DTO</returns>
        public async Task<IEnumerable<ChangeLogDTO>> GetAllChangeLogsAsync()
        {
            try
            {
                // Obtener registros de cambios de la capa de datos
                var changeLogs = await _changeLogData.GetAllAsync();
                var changeLogsDTO = new List<ChangeLogDTO>();

                // Convertir cada registro de cambio a DTO
                foreach (var changeLog in changeLogs)
                {
                    changeLogsDTO.Add(new ChangeLogDTO
                    {
                        Id = changeLog.Id,
                        TableName = changeLog.TableName,
                        IdTable = changeLog.IdTable,
                        OldValues = changeLog.OldValues,
                        NewValues = changeLog.NewValues,
                        Action = changeLog.Action,
                        UserName = changeLog.UserName   
                    });
                }

                return changeLogsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de cambios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de registros de cambios", ex);
            }
        }

        /// <summary>
        /// Obtiene un registro de cambio específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del registro de cambio</param>
        /// <returns>Registro de cambio en formato DTO</returns>
        public async Task<ChangeLogDTO> GetChangeLogByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un registro de cambio con ID inválido: {ChangeLogId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del registro de cambio debe ser mayor que cero");
            }

            try
            {
                // Buscar el registro de cambio en la base de datos
                var changeLog = await _changeLogData.GetByIdAsync(id);
                if (changeLog == null)
                {
                    _logger.LogInformation("No se encontró ningún registro de cambio con ID: {ChangeLogId}", id);
                    throw new EntityNotFoundException("ChangeLog", id);
                }

                // Convertir el registro de cambio a DTO
                return new ChangeLogDTO
                {
                    Id = changeLog.Id,
                    TableName = changeLog.TableName,
                    IdTable = changeLog.IdTable,
                    OldValues = changeLog.OldValues,
                    NewValues = changeLog.NewValues,
                    Action = changeLog.Action,
                    UserName = changeLog.UserName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro de cambio con ID: {ChangeLogId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el registro de cambio con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo registro de cambio en el sistema
        /// </summary>
        /// <param name="changeLogDto">Datos del registro de cambio a crear</param>
        /// <returns>Registro de cambio creado en formato DTO</returns>
        public async Task<ChangeLogDTO> CreateChangeLogAsync(ChangeLogDTO changeLogDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateChangeLog(changeLogDto);

                // Crear la entidad ChangeLog desde el DTO
                var changeLog = new ChangeLog
                {
                    TableName = changeLogDto.TableName,
                    IdTable = changeLogDto.IdTable,
                    OldValues = changeLogDto.OldValues,
                    NewValues = changeLogDto.NewValues,
                    Action = changeLogDto.Action,
                    UserName = changeLogDto.UserName
                };

                // Guardar el registro de cambio en la base de datos
                var changeLogCreado = await _changeLogData.CreateAsync(changeLog);

                // Convertir el registro de cambio creado a DTO para la respuesta
                return new ChangeLogDTO
                {
                    Id = changeLog.Id,
                    TableName = changeLog.TableName,
                    IdTable = changeLog.IdTable,
                    OldValues = changeLog.OldValues,
                    NewValues = changeLog.NewValues,
                    Action = changeLog.Action,
                    UserName = changeLog.UserName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo registro de cambio: {ChangeLogDescription}", changeLogDto?.TableName ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el registro de cambio", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de registro de cambio
        /// </summary>
        /// <param name="changeLogDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateChangeLog(ChangeLogDTO changeLogDto)
        {
            // Validar que el DTO no sea nulo
            if (changeLogDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto registro de cambio no puede ser nulo");
            }

            // Validar que el action no esté vacío
            if (string.IsNullOrWhiteSpace(changeLogDto.Action))
            {
                _logger.LogWarning("Se intentó crear/actualizar un registro de cambio con ChangeType vacío");
                throw new Utilities.Exceptions.ValidationException("ChangeType", "El ChangeType del registro de cambio es obligatorio");
            }
        }
    }
}
