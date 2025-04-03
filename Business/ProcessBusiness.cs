using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los procesos.
    /// Implementa la lógica de negocio para la gestión de procesos, incluyendo operaciones CRUD.
    /// </summary>
    public class ProcessBusiness
    {
        // Dependencias inyectadas
        private readonly ProcessData _processData; // Acceso a la capa de datos
        private readonly ILogger _logger;         // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="processData">Servicio de acceso a datos para procesos</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public ProcessBusiness(ProcessData processData, ILogger logger)
        {
            _processData = processData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los procesos del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de procesos en formato DTO</returns>
        public async Task<IEnumerable<ProcessDTO>> GetAllProcessesAsync()
        {
            try
            {
                // Obtener procesos de la capa de datos
                var processes = await _processData.GetAllAsync();
                var processesDTO = new List<ProcessDTO>();

                // Convertir cada proceso a DTO
                foreach (var process in processes)
                {
                    processesDTO.Add(new ProcessDTO
                    {
                        Id = process.Id,
                        TypeProcess = process.TypeProcess,
                        Start_Aprendiz = process.Start_Aprendiz,
                        Observation = process.Observation
                    });
                }

                return processesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los procesos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de procesos", ex);
            }
        }

        /// <summary>
        /// Obtiene un proceso específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del proceso</param>
        /// <returns>Proceso en formato DTO</returns>
        public async Task<ProcessDTO> GetProcessByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un proceso con ID inválido: {ProcessId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del proceso debe ser mayor que cero");
            }

            try
            {
                // Buscar el proceso en la base de datos
                var process = await _processData.GetByIdAsync(id);
                if (process == null)
                {
                    _logger.LogInformation("No se encontró ningún proceso con ID: {ProcessId}", id);
                    throw new EntityNotFoundException("Process", id);
                }

                // Convertir el proceso a DTO
                return new ProcessDTO
                {
                    Id = process.Id,
                    TypeProcess = process.TypeProcess,
                    Start_Aprendiz = process.Start_Aprendiz,
                    Observation = process.Observation
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el proceso con ID: {ProcessId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el proceso con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo proceso en el sistema
        /// </summary>
        /// <param name="processDto">Datos del proceso a crear</param>
        /// <returns>Proceso creado en formato DTO</returns>
        public async Task<ProcessDTO> CreateProcessAsync(ProcessDTO processDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateProcess(processDto);

                // Crear la entidad Process desde el DTO
                var process = new Process
                {
                    TypeProcess = processDto.TypeProcess,
                    Start_Aprendiz = processDto.Start_Aprendiz,
                    Observation = processDto.Observation
                };

                // Guardar el proceso en la base de datos
                var processCreado = await _processData.CreateAsync(process);

                // Convertir el proceso creado a DTO para la respuesta
                return new ProcessDTO
                {
                    Id = process.Id,
                    TypeProcess = process.TypeProcess,
                    Start_Aprendiz = process.Start_Aprendiz,
                    Observation = process.Observation
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo proceso: {ProcessType}", processDto?.TypeProcess ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el proceso", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de proceso
        /// </summary>
        /// <param name="processDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateProcess(ProcessDTO processDto)
        {
            // Validar que el DTO no sea nulo
            if (processDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto proceso no puede ser nulo");
            }

            // Validar que el TypeProcess no esté vacío
            if (string.IsNullOrWhiteSpace(processDto.TypeProcess))
            {
                _logger.LogWarning("Se intentó crear/actualizar un proceso con TypeProcess vacío");
                throw new Utilities.Exceptions.ValidationException("TypeProcess", "El TypeProcess del proceso es obligatorio");
            }
        }
    }
}