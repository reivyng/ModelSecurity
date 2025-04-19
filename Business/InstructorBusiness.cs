using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los instructores en el sistema.
    /// </summary>
    public class InstructorBusiness
    {
        private readonly InstructorData _instructorData;
        private readonly ILogger<InstructorBusiness> _logger;

        public InstructorBusiness(InstructorData instructorData, ILogger<InstructorBusiness> logger)
        {
            _instructorData = instructorData;
            _logger = logger;
        }

        // Método para obtener todos los instructores como DTOs
        public async Task<IEnumerable<InstructorDto>> GetAllInstructorsAsync()
        {
            try
            {
                var instructors = await _instructorData.GetAllAsync();
                return MapToDTOList(instructors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los instructores");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de instructores", ex);
            }
        }

        // Método para obtener un instructor por ID como DTO
        public async Task<InstructorDto> GetInstructorByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un instructor con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del instructor debe ser mayor que cero");
            }

            try
            {
                var instructor = await _instructorData.GetByIdAsync(id);
                if (instructor == null)
                {
                    _logger.LogInformation("No se encontró ningún instructor con ID: {Id}", id);
                    throw new EntityNotFoundException("instructor", id);
                }

                return MapToDTO(instructor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el instructor con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el instructor con ID {id}", ex);
            }
        }

        // Método para crear un instructor desde un DTO
        public async Task<InstructorDto> CreateInstructorAsync(InstructorDto instructorDto)
        {
            try
            {
                ValidateInstructor(instructorDto);

                var instructor = MapToEntity(instructorDto);

                var instructorCreado = await _instructorData.CreateAsync(instructor);

                return MapToDTO(instructorCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo instructor");
                throw new ExternalServiceException("Base de datos", "Error al crear el instructor", ex);
            }
        }

        // Método para actualizar un instructor existente
        public async Task<bool> UpdateInstructorAsync(InstructorDto instructorDto)
        {
            try
            {
                ValidateInstructor(instructorDto);

                var instructor = MapToEntity(instructorDto);

                var result = await _instructorData.UpdateAsync(instructor);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar el instructor con ID {InstructorId}", instructorDto.Id);
                    throw new EntityNotFoundException("Instructor", instructorDto.Id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el instructor con ID {InstructorId}", instructorDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el instructor con ID {instructorDto.Id}", ex);
            }
        }

        // Método para actualizar campos específicos de un instructor
        public async Task<bool> UpdatePartialInstructorAsync(int id, InstructorDto updatedFields)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un instructor con un ID inválido: {InstructorId}", id);
                throw new ValidationException("id", "El ID del instructor debe ser mayor a 0");
            }

            try
            {
                var existingInstructor = await _instructorData.GetByIdAsync(id);
                if (existingInstructor == null)
                {
                    _logger.LogInformation("No se encontró el instructor con ID {InstructorId} para actualización parcial", id);
                    throw new EntityNotFoundException("Instructor", id);
                }

                if (updatedFields.UserId > 0 && updatedFields.UserId != existingInstructor.UserId)
                {
                    existingInstructor.UserId = updatedFields.UserId;
                }
                if (updatedFields.Active != existingInstructor.Active)
                {
                    existingInstructor.Active = updatedFields.Active;
                }

                var result = await _instructorData.UpdateAsync(existingInstructor);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar parcialmente el instructor con ID {InstructorId}", id);
                    throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el instructor con ID {id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el instructor con ID {InstructorId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el instructor con ID {id}", ex);
            }
        }

        // Método para realizar una eliminación lógica de un instructor
        public async Task<bool> SoftDeleteInstructorAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {InstructorId}", id);
                throw new ValidationException("id", "El ID del instructor debe ser mayor a 0");
            }

            try
            {
                var instructor = await _instructorData.GetByIdAsync(id);
                if (instructor == null)
                {
                    _logger.LogInformation("No se encontró el instructor con ID {InstructorId} para eliminación lógica", id);
                    throw new EntityNotFoundException("Instructor", id);
                }

                instructor.Active = false;

                var result = await _instructorData.UpdateAsync(instructor);

                if (!result)
                {
                    _logger.LogWarning("No se pudo realizar la eliminación lógica del instructor con ID {InstructorId}", id);
                    throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del instructor con ID {id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica del instructor con ID {InstructorId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del instructor con ID {id}", ex);
            }
        }

        // Método para eliminar un instructor por su ID
        public async Task<bool> DeleteInstructorAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un instructor con un ID inválido: {InstructorId}", id);
                throw new ValidationException("id", "El ID del instructor debe ser mayor a 0");
            }

            try
            {
                var result = await _instructorData.DeleteAsync(id);

                if (!result)
                {
                    _logger.LogInformation("No se encontró el instructor con ID {InstructorId} para eliminar", id);
                    throw new EntityNotFoundException("Instructor", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el instructor con ID {InstructorId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el instructor con ID {id}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateInstructor(InstructorDto instructorDto)
        {
            if (instructorDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Instructor no puede ser nulo");
            }

            if (instructorDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un instructor con UserId inválido");
                throw new Utilities.Exceptions.ValidationException("UserId", "El UserId del instructor es obligatorio y debe ser mayor que cero");
            }
        }

        // Método para mapear de Instructor a InstructorDto
        private InstructorDto MapToDTO(Instructor instructor)
        {
            return new InstructorDto
            {
                Id = instructor.Id,
                Active = instructor.Active,
                UserId = instructor.UserId,
            };
        }

        // Método para mapear de InstructorDto a Instructor
        private Instructor MapToEntity(InstructorDto instructorDto)
        {
            return new Instructor
            {
                Id = instructorDto.Id,
                Active = instructorDto.Active,
                UserId = instructorDto.UserId,
            };
        }

        // Método para mapear una lista de Instructor a una lista de InstructorDto
        private IEnumerable<InstructorDto> MapToDTOList(IEnumerable<Instructor> instructors)
        {
            var instructorsDTO = new List<InstructorDto>();
            foreach (var instructor in instructors)
            {
                instructorsDTO.Add(MapToDTO(instructor));
            }
            return instructorsDTO;
        }
    }
}


