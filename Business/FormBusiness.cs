using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los formularios del sistema.
    /// Implementa la lógica de negocio para la gestión de formularios, incluyendo operaciones CRUD.
    /// </summary>
    public class FormBusiness
    {
        // Dependencias inyectadas
        private readonly FormData _formData;        // Acceso a la capa de datos
        private readonly ILogger _logger;         // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="formData">Servicio de acceso a datos para formularios</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public FormBusiness(FormData formData, ILogger logger)
        {
            _formData = formData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los formularios del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de formularios en formato DTO</returns>
        public async Task<IEnumerable<FormDTOAuto>> GetAllFormsAsync()
        {
            try
            {
                // Obtener formularios de la capa de datos
                var forms = await _formData.GetAllAsync();
                var formsDTO = new List<FormDTOAuto>();

                // Convertir cada formulario a DTO
                foreach (var form in forms)
                {
                    formsDTO.Add(new FormDTOAuto
                    {
                        Id = form.Id,
                        Code = form.Code,
                        Name = form.Name,
                        // Active = form.Active,
                        // CreateDate = form.CreateDate,
                        // UpdateDate = form.UpdateDate,
                        // DeleteDate = form.DeleteDate
                    });
                }

                return formsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los formularios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de formularios", ex);
            }
        }

        /// <summary>
        /// Obtiene un formulario específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del formulario</param>
        /// <returns>Formulario en formato DTO</returns>
        public async Task<FormDTOAuto> GetFormByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un formulario con ID inválido: {FormId}", id);
                throw new ValidationException("id", "El ID del formulario debe ser mayor que cero");
            }

            try
            {
                // Buscar el formulario en la base de datos
                var form = await _formData.GetByIdAsync(id);
                if (form == null)
                {
                    _logger.LogInformation("No se encontró ningún formulario con ID: {FormId}", id);
                    throw new EntityNotFoundException("Form", id);
                }

                // Convertir el formulario a DTO
                return new FormDTOAuto
                {
                    Id = form.Id,
                    Code = form.Code,
                    Name = form.Name,
                    Active = form.Active,
                    CreateDate = form.CreateDate,
                    UpdateDate = form.UpdateDate,
                    DeleteDate = form.DeleteDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el formulario con ID: {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el formulario con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo formulario en el sistema
        /// </summary>
        /// <param name="formDto">Datos del formulario a crear</param>
        /// <returns>Formulario creado en formato DTO</returns>
        public async Task<FormDTOAuto> CreateFormAsync(FormDTOAuto formDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateForm(formDto);

                // Crear la entidad Form desde el DTO
                var form = new Form
                {
                    Code = formDto.Code,
                    Name = formDto.Name,
                    Active = formDto.Active,
                    CreateDate = DateTime.Now
                };

                // Guardar el formulario en la base de datos
                var formCreado = await _formData.CreateAsync(form);

                // Convertir el formulario creado a DTO para la respuesta
                return new FormDTOAuto
                {
                    Id = formCreado.Id,
                    Code = formCreado.Code,
                    Name = formCreado.Name,
                    Active = formCreado.Active,
                    CreateDate = formCreado.CreateDate,
                    UpdateDate = formCreado.UpdateDate,
                    DeleteDate = formCreado.DeleteDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo formulario: {FormName}", formDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el formulario", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de formulario
        /// </summary>
        /// <param name="formDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateForm(FormDTOAuto formDto)
        {
            // Validar que el DTO no sea nulo
            if (formDto == null)
            {
                throw new ValidationException("El objeto formulario no puede ser nulo");
            }

            // Validar que el código no esté vacío
            if (string.IsNullOrWhiteSpace(formDto.Code))
            {
                _logger.LogWarning("Se intentó crear/actualizar un formulario con código vacío");
                throw new ValidationException("Code", "El código del formulario es obligatorio");
            }

            // Validar que el nombre no esté vacío
            if (string.IsNullOrWhiteSpace(formDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un formulario con nombre vacío");
                throw new ValidationException("Name", "El nombre del formulario es obligatorio");
            }
        }
    }
} 