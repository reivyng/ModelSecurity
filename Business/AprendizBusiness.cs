using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los aprendices del sistema.
    /// Implementa la lógica de negocio para la gestión de aprendices, incluyendo operaciones CRUD.
    /// </summary>
    public class AprendizBusiness
    {
        // Dependencias inyectadas
        private readonly AprendizData _aprendizData;        // Acceso a la capa de datos
        private readonly ILogger _logger;         // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="aprendizData">Servicio de acceso a datos para aprendices</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public AprendizBusiness(AprendizData aprendizData, ILogger logger)
        {
            _aprendizData = aprendizData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los aprendices del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de aprendices en formato DTO</returns>
        public async Task<IEnumerable<AprendizDTO>> GetAllAprendicesAsync()
        {
            try
            {
                // Obtener aprendices de la capa de datos
                var aprendices = await _aprendizData.GetAllAsync();
                var aprendicesDTO = new List<AprendizDTO>();

                // Convertir cada aprendiz a DTO
                foreach (var aprendiz in aprendices)
                {
                    aprendicesDTO.Add(new AprendizDTO
                    {
                        Id = aprendiz.Id,
                        PreviuosProgram = aprendiz.PreviuosProgram                
                    });
                }

                return aprendicesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los aprendices");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de aprendices", ex);
            }
        }

        /// <summary>
        /// Obtiene un aprendiz específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del aprendiz</param>
        /// <returns>Aprendiz en formato DTO</returns>
        public async Task<AprendizDTO> GetAprendizByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un aprendiz con ID inválido: {AprendizId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del aprendiz debe ser mayor que cero");
            }

            try
            {
                // Buscar el aprendiz en la base de datos
                var aprendiz = await _aprendizData.GetByIdAsync(id);
                if (aprendiz == null)
                {
                    _logger.LogInformation("No se encontró ningún aprendiz con ID: {AprendizId}", id);
                    throw new EntityNotFoundException("Aprendiz", id);
                }

                // Convertir el aprendiz a DTO
                return new AprendizDTO
                {
                    Id = aprendiz.Id,
                    PreviuosProgram = aprendiz.PreviuosProgram
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el aprendiz con ID: {AprendizId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el aprendiz con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo aprendiz en el sistema
        /// </summary>
        /// <param name="aprendizDto">Datos del aprendiz a crear</param>
        /// <returns>Aprendiz creado en formato DTO</returns>
        public async Task<AprendizDTO> CreateAprendizAsync(AprendizDTO aprendizDto)
        {
            
                // Validar los datos del DTO
                ValidateAprendiz(aprendizDto);

                // Crear la entidad Aprendiz desde el DTO
                var aprendiz = new Aprendiz
                {
                    PreviuosProgram = aprendizDto.PreviuosProgram
                };

                // Guardar el aprendiz en la base de datos
                var aprendizCreado = await _aprendizData.CreateAsync(aprendiz);

                // Convertir el aprendiz creado a DTO para la respuesta
                return new AprendizDTO
                {
                    Id = aprendiz.Id,
                    PreviuosProgram = aprendiz.PreviuosProgram
                };
            
        }

        /// <summary>
        /// Valida los datos del DTO de aprendiz
        /// </summary>
        /// <param name="aprendizDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateAprendiz(AprendizDTO aprendizDto)
        {
            // Validar que el DTO no sea nulo
            if (aprendizDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto aprendiz no puede ser nulo");
            }                        
        }
    }
}
