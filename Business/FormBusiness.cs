using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los formularios en el sistema.
    /// </summary>
    public class FormBusiness
    {
        private readonly FormData _formData;
        private readonly ILogger<FormBusiness> _logger;

        public FormBusiness(FormData formData, ILogger<FormBusiness> logger)
        {
            _formData = formData;
            _logger = logger;
        }

        // Método para obtener todos los formularios como DTOs
        public async Task<IEnumerable<FormDto>> GetAllFormsAsync()
        {
            try
            {
                var forms = await _formData.GetAllAsync();
                return MapToDTOList(forms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los formularios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de formularios", ex);
            }
        }

        // Método para obtener un formulario por ID como DTO
        public async Task<FormDto> GetFormByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un formulario con ID inválido: {FormId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del formulario debe ser mayor que cero");
            }

            try
            {
                var form = await _formData.GetByidAsync(id);
                if (form == null)
                {
                    _logger.LogInformation("No se encontró ningún formulario con ID: {FormId}", id);
                    throw new EntityNotFoundException("Form", id);
                }

                return MapToDTO(form);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el formulario con ID: {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el formulario con ID {id}", ex);
            }
        }

        // Método para crear un formulario desde un DTO
        public async Task<FormDto> CreateFormAsync(FormDto formDto)
        {
            try
            {
                ValidateForm(formDto);

                var form = MapToEntity(formDto);
                form.CreateDate = DateTime.Now;        
                var formCreado = await _formData.CreateAsync(form);

                return MapToDTO(formCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo formulario: {Name}", formDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el formulario", ex);
            }
        }

        // Método para actualizar un formulario existente
        public async Task<bool> UpdateFormAsync(FormDto formDto)
        {
            try
            {
                ValidateForm(formDto);

                var form = MapToEntity(formDto);

                var result = await _formData.UpdateAsync(form);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar el formulario con ID {FormId}", formDto.Id);
                    throw new EntityNotFoundException("Form", formDto.Id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el formulario con ID {FormId}", formDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el formulario con ID {formDto.Id}", ex);
            }
        }

        // Método para actualizar campos específicos de un formulario
        public async Task<bool> UpdatePartialFormAsync(int id, FormDto updatedFields)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un formulario con un ID inválido: {FormId}", id);
                throw new ValidationException("id", "El ID del formulario debe ser mayor a 0");
            }

            try
            {
                var existingForm = await _formData.GetByidAsync(id);
                if (existingForm == null)
                {
                    _logger.LogInformation("No se encontró el formulario con ID {FormId} para actualización parcial", id);
                    throw new EntityNotFoundException("Form", id);
                }

                if (!string.IsNullOrWhiteSpace(updatedFields.Name))
                {
                    existingForm.Name = updatedFields.Name;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.Description))
                {
                    existingForm.Description = updatedFields.Description;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.Cuestion))
                {
                    existingForm.Cuestion = updatedFields.Cuestion;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.TypeCuestion))
                {
                    existingForm.TypeCuestion = updatedFields.TypeCuestion;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.Answer))
                {
                    existingForm.Answer = updatedFields.Answer;
                }
                if (updatedFields.Active != existingForm.Active)
                {
                    existingForm.Active = updatedFields.Active;
                }

                var result = await _formData.UpdateAsync(existingForm);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar parcialmente el formulario con ID {FormId}", id);
                    throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el formulario con ID {id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el formulario con ID {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el formulario con ID {id}", ex);
            }
        }

        // Método para realizar una eliminación lógica de un formulario
        public async Task<bool> SoftDeleteFormAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {FormId}", id);
                throw new ValidationException("id", "El ID del formulario debe ser mayor a 0");
            }

            try
            {
                var form = await _formData.GetByidAsync(id);
                if (form == null)
                {
                    _logger.LogInformation("No se encontró el formulario con ID {FormId} para eliminación lógica", id);
                    throw new EntityNotFoundException("Form", id);
                }

                form.Active = false;

                var result = await _formData.UpdateAsync(form);

                if (!result)
                {
                    _logger.LogWarning("No se pudo realizar la eliminación lógica del formulario con ID {FormId}", id);
                    throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del formulario con ID {id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica del formulario con ID {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del formulario con ID {id}", ex);
            }
        }

        // Método para eliminar un formulario por su ID
        public async Task<bool> DeleteFormAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un formulario con un ID inválido: {FormId}", id);
                throw new ValidationException("id", "El ID del formulario debe ser mayor a 0");
            }

            try
            {
                var result = await _formData.DeleteAsync(id);

                if (!result)
                {
                    _logger.LogInformation("No se encontró el formulario con ID {FormId} para eliminar", id);
                    throw new EntityNotFoundException("Form", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el formulario con ID {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el formulario con ID {id}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateForm(FormDto formDto)
        {
            if (formDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto formulario no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(formDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un formulario con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del formulario es obligatorio");
            }
        }

        // Método para mapear de Form a FormDto
        private FormDto MapToDTO(Form form)
        {
            return new FormDto
            {
                Id = form.Id,
                Name = form.Name,
                Description = form.Description,
                Cuestion = form.Cuestion,
                TypeCuestion = form.TypeCuestion,
                Answer = form.Answer,
                Active = form.Active,
            };
        }

        // Método para mapear de FormDto a Form
        private Form MapToEntity(FormDto formDto)
        {
            return new Form
            {
                Id = formDto.Id,
                Name = formDto.Name,
                Description = formDto.Description,
                Cuestion = formDto.Cuestion,
                TypeCuestion = formDto.TypeCuestion,
                Answer = formDto.Answer,
                Active = formDto.Active,
            };
        }

        // Método para mapear una lista de Form a una lista de FormDto
        private IEnumerable<FormDto> MapToDTOList(IEnumerable<Form> forms)
        {
            var formsDTO = new List<FormDto>();
            foreach (var form in forms)
            {
                formsDTO.Add(MapToDTO(form));
            }
            return formsDTO;
        }
    }
}

