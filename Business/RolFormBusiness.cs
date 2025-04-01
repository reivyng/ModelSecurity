using Data;
using Entity.DTOautogestion.pivote;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con la relación entre roles y formularios.
    /// Implementa la lógica de negocio para la gestión de las relaciones entre roles y formularios.
    /// </summary>
    public class RolFormBusiness
    {
        // Dependencias inyectadas
        private readonly RolFormData _rolFormData;        // Acceso a la capa de datos
        private readonly ILogger _logger;         // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="rolFormData">Servicio de acceso a datos para la relación rol-form</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public RolFormBusiness(RolFormData rolFormData, ILogger logger)
        {
            _rolFormData = rolFormData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las relaciones rol-form del sistema y las convierte a DTOs
        /// </summary>
        /// <returns>Lista de relaciones rol-form en formato DTO</returns>
        public async Task<IEnumerable<RolFormDTOAuto>> GetAllRolFormsAsync()
        {
            try
            {
                // Obtener relaciones de la capa de datos
                var rolForms = await _rolFormData.GetAllAsync();
                var rolFormsDTO = new List<RolFormDTOAuto>();

                // Convertir cada relación a DTO
                foreach (var rolForm in rolForms)
                {
                    rolFormsDTO.Add(new RolFormDTOAuto
                    {
                        id = rolForm.id,
                        RolId = rolForm.RolId,
                        FormId = rolForm.FormId,
                        Permission = rolForm.Permission

                    });
                }

                return rolFormsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las relaciones rol-form");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de relaciones rol-form", ex);
            }
        }

        /// <summary>
        /// Obtiene una relación rol-form específica por su ID
        /// </summary>
        /// <param name="id">Identificador único de la relación</param>
        /// <returns>Relación rol-form en formato DTO</returns>
        public async Task<RolFormDTOAuto> GetRolFormByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una relación rol-form con ID inválido: {RolFormId}", id);
                throw new ValidationException("id", "El ID de la relación debe ser mayor que cero");
            }

            try
            {
                // Buscar la relación en la base de datos
                var rolForm = await _rolFormData.GetByidAsync(id);
                if (rolForm == null)
                {
                    _logger.LogInformation("No se encontró ninguna relación rol-form con ID: {RolFormId}", id);
                    throw new EntityNotFoundException("RolForm", id);
                }

                // Convertir la relación a DTO
                return new RolFormDTOAuto
                {
                    id = rolForm.id,
                    Permission = rolForm.Permission,
                    RolId = rolForm.RolId,
                    FormId = rolForm.FormId,
                   
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la relación rol-form con ID: {RolFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la relación rol-form con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea una nueva relación rol-form en el sistema
        /// </summary>
        /// <param name="rolFormDto">Datos de la relación a crear</param>
        /// <returns>Relación creada en formato DTO</returns>
        public async Task<RolFormDTOAuto> CreateRolFormAsync(RolFormDTOAuto rolFormDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateRolForm(rolFormDto);

                // Crear la entidad RolForm desde el DTO
                var rolForm = new RolFormDTOAuto
                {
                    id = rolFormDto.id,
                    RolId = rolFormDto.RolId,
                    FormId = rolFormDto.FormId,
                    Permission = rolFormDto.Permission


                };

                // Guardar la relación en la base de datos
                var rolFormCreado = await _rolFormData.CreateAsync(rolForm);

                // Convertir la relación creada a DTO para la respuesta
                return new RolFormDTOAuto
                {
                    id = rolFormCreado.Id,
                    RolId = rolFormCreado.RolId,
                    FormId = rolFormCreado.FormId,
                    
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva relación rol-form");
                throw new ExternalServiceException("Base de datos", "Error al crear la relación rol-form", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de relación rol-form
        /// </summary>
        /// <param name="rolFormDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateRolForm(RolFormDTOAuto rolFormDto)
        {
            // Validar que el DTO no sea nulo
            if (rolFormDto == null)
            {
                throw new ValidationException("El objeto relación rol-form no puede ser nulo");
            }

            // Validar que el RolId sea válido
            if (rolFormDto.RolId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una relación con RolId inválido: {RolId}", rolFormDto.RolId);
                throw new ValidationException("RolId", "El ID del rol debe ser mayor que cero");
            }

            // Validar que el FormId sea válido
            if (rolFormDto.FormId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una relación con FormId inválido: {FormId}", rolFormDto.FormId);
                throw new ValidationException("FormId", "El ID del formulario debe ser mayor que cero");
            }
        }
    }
} 