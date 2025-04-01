using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los módulos del sistema.
    /// Implementa la lógica de negocio para la gestión de módulos, incluyendo operaciones CRUD.
    /// </summary>
    public class ModuleBusiness
    {
        // Dependencias inyectadas
        private readonly ModuleData _moduleData;        // Acceso a la capa de datos
        private readonly ILogger _logger;         // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="moduleData">Servicio de acceso a datos para módulos</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public ModuleBusiness(ModuleData moduleData, ILogger logger)
        {
            _moduleData = moduleData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los módulos del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de módulos en formato DTO</returns>
        public async Task<IEnumerable<ModuleDTOAuto>> GetAllModulesAsync()
        {
            try
            {
                // Obtener módulos de la capa de datos
                var modules = await _moduleData.GetAllAsync();
                var modulesDTO = new List<ModuleDTOAuto>();

                // Convertir cada módulo a DTO
                foreach (var module in modules)
                {
                    modulesDTO.Add(new ModuleDTOAuto
                    {
                        id = module.id,
                        active = module.active
                        
                    });
                }

                return modulesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los módulos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de módulos", ex);
            }
        }

        /// <summary>
        /// Obtiene un módulo específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del módulo</param>
        /// <returns>Módulo en formato DTO</returns>
        public async Task<ModuleDTOAuto> GetModuleByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un módulo con ID inválido: {ModuleId}", id);
                throw new ValidationException("id", "El ID del módulo debe ser mayor que cero");
            }

            try
            {
                // Buscar el módulo en la base de datos
                var module = await _moduleData.GetByidAsync(id);
                if (module == null)
                {
                    _logger.LogInformation("No se encontró ningún módulo con ID: {ModuleId}", id);
                    throw new EntityNotFoundException("Module", id);
                }

                // Convertir el módulo a DTO
                return new ModuleDTOAuto
                {
                    id = module.id,
                    active = module.active,
              
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el módulo con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el módulo con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo módulo en el sistema
        /// </summary>
        /// <param name="moduleDto">Datos del módulo a crear</param>
        /// <returns>Módulo creado en formato DTO</returns>
        public async Task<ModuleDTOAuto> CreateModuleAsync(ModuleDTOAuto moduleDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateModule(moduleDto);

                // Crear la entidad Module desde el DTO
                var module = new Module
                {
                    id = moduleDto.id,
                    name = moduleDto.name,
                    active = moduleDto.active
                   
                };

                // Guardar el módulo en la base de datos
                var moduleCreado = await _moduleData.CreateAsync(module);

                // Convertir el módulo creado a DTO para la respuesta
                return new ModuleDTOAuto
                {
                    id = moduleCreado.id,
                    name = moduleCreado.name,
                    active = moduleCreado.active,
                    
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo módulo: {ModuleName}", moduleDto?.name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el módulo", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de módulo
        /// </summary>
        /// <param name="moduleDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateModule(ModuleDTOAuto moduleDto)
        {
            // Validar que el DTO no sea nulo
            if (moduleDto == null)
            {
                throw new ValidationException("El objeto módulo no puede ser nulo");
            }

            // Validar que el nombre no esté vacío
            if (string.IsNullOrWhiteSpace(moduleDto.name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un módulo con nombre vacío");
                throw new ValidationException("Name", "El nombre del módulo es obligatorio");
            }
        }
    }
} 