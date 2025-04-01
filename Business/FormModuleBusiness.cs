using Data;
using Entity.DTOautogestion.pivote;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con la relación entre formularios y módulos.
    /// Implementa la lógica de negocio para la gestión de las relaciones entre formularios y módulos.
    /// </summary>
    public class FormModuleBusiness
    {
        // Dependencias inyectadas
        private readonly FormModuleData _formModuleData;        // Acceso a la capa de datos
        private readonly ILogger _logger;         // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="formModuleData">Servicio de acceso a datos para la relación form-module</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public FormModuleBusiness(FormModuleData formModuleData, ILogger logger)
        {
            _formModuleData = formModuleData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las relaciones form-module del sistema y las convierte a DTOs
        /// </summary>
        /// <returns>Lista de relaciones form-module en formato DTO</returns>
        public async Task<IEnumerable<FormModuleDTOAuto>> GetAllFormModulesAsync()
        {
            try
            {
                // Obtener relaciones de la capa de datos
                var formModules = await _formModuleData.GetAllAsync();
                var formModulesDTO = new List<FormModuleDTOAuto>();

                // Convertir cada relación a DTO
                foreach (var formModule in formModules)
                {
                    formModulesDTO.Add(new FormModuleDTOAuto
                    {
                        Id = formModule.Id,
                        StatusProcedure = formModule.status_procedure
                    });
                }

                return formModulesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las relaciones form-module");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de relaciones form-module", ex);
            }
        }

        /// <summary>
        /// Obtiene una relación form-module específica por su ID
        /// </summary>
        /// <param name="id">Identificador único de la relación</param>
        /// <returns>Relación form-module en formato DTO</returns>
        public async Task<FormModuleDTOAuto> GetFormModuleByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una relación form-module con ID inválido: {FormModuleId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación debe ser mayor que cero");
            }

            try
            {
                // Buscar la relación en la base de datos
                var formModule = await _formModuleData.GetByIdAsync(id);
                if (formModule == null)
                {
                    _logger.LogInformation("No se encontró ninguna relación form-module con ID: {FormModuleId}", id);
                    throw new EntityNotFoundException("FormModule", id);
                }

                // Convertir la relación a DTO
                return new FormModuleDTOAuto
                {
                    Id = formModule.Id,
                    StatusProcedure = formModule.status_procedure
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la relación form-module con ID: {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la relación form-module con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea una nueva relación form-module en el sistema
        /// </summary>
        /// <param name="formModuleDto">Datos de la relación a crear</param>
        /// <returns>Relación creada en formato DTO</returns>
        public async Task<FormModuleDTOAuto> CreateFormModuleAsync(FormModuleDTOAuto formModuleDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateFormModule(formModuleDto);

                // Crear la entidad FormModule desde el DTO
                var formModule = new FormModule
                {
                    status_procedure = formModuleDto.StatusProcedure
                };

                // Guardar la relación en la base de datos
                var formModuleCreado = await _formModuleData.CreateAsync(formModule);

                // Convertir la relación creada a DTO para la respuesta
                return new FormModuleDTOAuto
                {
                    Id = formModuleCreado.Id,
                    StatusProcedure = formModuleCreado.status_procedure
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva relación form-module");
                throw new ExternalServiceException("Base de datos", "Error al crear la relación form-module", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de relación form-module
        /// </summary>
        /// <param name="formModuleDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateFormModule(FormModuleDTOAuto formModuleDto)
        {
            // Validar que el DTO no sea nulo
            if (formModuleDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto relación form-module no puede ser nulo");
            }

            // Validar que el StatusProcedure no esté vacío
            if (string.IsNullOrWhiteSpace(formModuleDto.StatusProcedure))
            {
                _logger.LogWarning("Se intentó crear/actualizar una relación con StatusProcedure vacío");
                throw new Utilities.Exceptions.ValidationException("StatusProcedure", "El StatusProcedure es obligatorio");
            }
        }
    }
} 