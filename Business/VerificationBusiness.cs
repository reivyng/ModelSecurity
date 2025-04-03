using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las verificaciones.
    /// Implementa la lógica de negocio para la gestión de verificaciones, incluyendo operaciones CRUD.
    /// </summary>
    internal class VerificationBusiness
    {
        // Dependencias inyectadas
        private readonly VerificationData _verificationData; // Acceso a la capa de datos
        private readonly ILogger _logger;                   // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="verificationData">Servicio de acceso a datos para verificaciones</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public VerificationBusiness(VerificationData verificationData, ILogger logger)
        {
            _verificationData = verificationData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las verificaciones del sistema y las convierte a DTOs
        /// </summary>
        /// <returns>Lista de verificaciones en formato DTO</returns>
        public async Task<IEnumerable<VerificationDTO>> GetAllVerificationsAsync()
        {
            try
            {
                // Obtener verificaciones de la capa de datos
                var verifications = await _verificationData.GetAllAsync();
                var verificationsDTO = new List<VerificationDTO>();

                // Convertir cada verificación a DTO
                foreach (var verification in verifications)
                {
                    verificationsDTO.Add(new VerificationDTO
                    {
                        Id = verification.Id,
                        Observation = verification.Observation
                    });
                }

                return verificationsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las verificaciones");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de verificaciones", ex);
            }
        }

        /// <summary>
        /// Obtiene una verificación específica por su ID
        /// </summary>
        /// <param name="id">Identificador único de la verificación</param>
        /// <returns>Verificación en formato DTO</returns>
        public async Task<VerificationDTO> GetVerificationByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una verificación con ID inválido: {VerificationId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la verificación debe ser mayor que cero");
            }

            try
            {
                // Buscar la verificación en la base de datos
                var verification = await _verificationData.GetByIdAsync(id);
                if (verification == null)
                {
                    _logger.LogInformation("No se encontró ninguna verificación con ID: {VerificationId}", id);
                    throw new EntityNotFoundException("Verification", id);
                }

                // Convertir la verificación a DTO
                return new VerificationDTO
                {
                    Id = verification.Id,
                    Observation = verification.Observation
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la verificación con ID: {VerificationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la verificación con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea una nueva verificación en el sistema
        /// </summary>
        /// <param name="verificationDto">Datos de la verificación a crear</param>
        /// <returns>Verificación creada en formato DTO</returns>
        public async Task<VerificationDTO> CreateVerificationAsync(VerificationDTO verificationDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateVerification(verificationDto);

                // Crear la entidad Verification desde el DTO
                var verification = new Verification
                {
                    Observation = verificationDto.Observation
                };

                // Guardar la verificación en la base de datos
                var verificationCreada = await _verificationData.CreateAsync(verification);

                // Convertir la verificación creada a DTO para la respuesta
                return new VerificationDTO
                {
                    Id = verificationCreada.Id,
                    Observation = verificationCreada.Observation
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva verificación: {VerificationObservation}", verificationDto?.Observation ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la verificación", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de verificación
        /// </summary>
        /// <param name="verificationDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateVerification(VerificationDTO verificationDto)
        {
            // Validar que el DTO no sea nulo
            if (verificationDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto verificación no puede ser nulo");
            }

            // Validar que el Observation no esté vacío
            if (string.IsNullOrWhiteSpace(verificationDto.Observation))
            {
                _logger.LogWarning("Se intentó crear/actualizar una verificación con Observation vacío");
                throw new Utilities.Exceptions.ValidationException("Observation", "El Observation de la verificación es obligatorio");
            }
        }
    }
}








