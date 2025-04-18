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
    /// Clase de negocio encargada de la lógica relacionada con las personas en el sistema.
    /// </summary>
    public class PersonBusiness
    {
        private readonly PersonData _personData;
        private readonly ILogger<PersonBusiness> _logger;

        public PersonBusiness(PersonData personData, ILogger<PersonBusiness> logger)
        {
            _personData = personData;
            _logger = logger;
        }

        // Método para obtener todas las personas como DTOs
        public async Task<IEnumerable<PersonDto>> GetAllPersonsAsync()
        {
            try
            {
                var persons = await _personData.GetAllAsync();
                return MapToDTOList(persons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las personas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de personas", ex);
            }
        }

        // Método para obtener una persona por ID como DTO
        public async Task<PersonDto> GetPersonByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una persona con ID inválido: {PersonId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la persona debe ser mayor que cero");
            }

            try
            {
                var person = await _personData.GetByIdAsync(id);
                if (person == null)
                {
                    _logger.LogInformation("No se encontró ninguna persona con ID: {PersonId}", id);
                    throw new EntityNotFoundException("Person", id);
                }

                return MapToDTO(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la persona con ID: {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la persona con ID {id}", ex);
            }
        }

        // Método para crear una persona desde un DTO
        public async Task<PersonDto> CreatePersonAsync(PersonDto personDto)
        {
            try
            {
                ValidatePerson(personDto);

                var person = MapToEntity(personDto);
                person.CreateDate = DateTime.Now;
                var personCreada = await _personData.CreateAsync(person);

                return MapToDTO(personCreada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva persona: {Name}", personDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la persona", ex);
            }
        }

        // Método para actualizar una persona existente
        public async Task<bool> UpdatePersonAsync(PersonDto personDto)
        {
            try
            {
                // Validar que el DTO no contenga datos vacíos
                ValidatePerson(personDto);

                // Obtener la persona existente desde la base de datos
                var existingPerson = await _personData.GetByIdAsync(personDto.Id);
                if (existingPerson == null)
                {
                    _logger.LogWarning("No se encontró la persona con ID {PersonId} para actualizar", personDto.Id);
                    throw new EntityNotFoundException("Person", personDto.Id);
                }

                // Mapear el DTO a la entidad, preservando la fecha de creación
                var person = MapToEntity(personDto);
                person.CreateDate = existingPerson.CreateDate; // Preservar la fecha de creación
                person.UpdateDate = DateTime.Now; // Establecer la fecha de actualización

                // Actualizar la persona en la base de datos
                var result = await _personData.UpdateAsync(person);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar la persona con ID {PersonId}", personDto.Id);
                    throw new EntityNotFoundException("Person", personDto.Id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la persona con ID {PersonId}", personDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la persona con ID {personDto.Id}", ex);
            }
        }


        // Método para actualizar campos específicos de una persona
        // Método para actualizar campos específicos de una persona
        public async Task<bool> UpdatePartialPersonAsync(int id, PersonDto updatedFields)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar una persona con un ID inválido: {PersonId}", id);
                throw new ValidationException("id", "El ID de la persona debe ser mayor a 0");
            }

            try
            {
                // Obtener la persona existente
                var existingPerson = await _personData.GetByIdAsync(id);
                if (existingPerson == null)
                {
                    _logger.LogInformation("No se encontró la persona con ID {PersonId} para actualización parcial", id);
                    throw new EntityNotFoundException("Person", id);
                }

                // Actualizar solo los campos proporcionados en el DTO
                if (!string.IsNullOrWhiteSpace(updatedFields.Name))
                {
                    existingPerson.Name = updatedFields.Name;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.FirstName))
                {
                    existingPerson.FirstName = updatedFields.FirstName;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.SecondName))
                {
                    existingPerson.SecondName = updatedFields.SecondName;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.FirstLastName))
                {
                    existingPerson.FirstLastName = updatedFields.FirstLastName;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.SecondLastName))
                {
                    existingPerson.SecondLastName = updatedFields.SecondLastName;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.PhoneNumber))
                {
                    existingPerson.PhoneNumber = updatedFields.PhoneNumber;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.Email))
                {
                    existingPerson.Email = updatedFields.Email;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.TypeIdentification))
                {
                    existingPerson.TypeIdentification = updatedFields.TypeIdentification;
                }
                if (updatedFields.NumberIdentification != 0)
                {
                    existingPerson.NumberIdentification = updatedFields.NumberIdentification;
                }
                if (updatedFields.Signig != existingPerson.Signig)
                {
                    existingPerson.Signig = updatedFields.Signig;
                }
                if (updatedFields.Active != existingPerson.Active)
                {
                    existingPerson.Active = updatedFields.Active;
                }

                // Guardar los cambios
                var result = await _personData.UpdateAsync(existingPerson);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar parcialmente la persona con ID {PersonId}", id);
                    throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la persona con ID {id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la persona con ID {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la persona con ID {id}", ex);
            }
        }


        // Método para realizar una eliminación lógica de una persona (marcar como inactiva)
        public async Task<bool> SoftDeletePersonAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {PersonId}", id);
                throw new ValidationException("id", "El ID de la persona debe ser mayor a 0");
            }

            try
            {
                // Obtener la persona por ID
                var person = await _personData.GetByIdAsync(id);
                if (person == null)
                {
                    _logger.LogInformation("No se encontró la persona con ID {PersonId} para eliminación lógica", id);
                    throw new EntityNotFoundException("Person", id);
                }

                // Marcar la persona como inactiva sin modificar la fecha de creación
                person.Active = false;

                // Actualizar la persona en la base de datos
                var result = await _personData.UpdateAsync(person);

                if (!result)
                {
                    _logger.LogWarning("No se pudo realizar la eliminación lógica de la persona con ID {PersonId}", id);
                    throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica de la persona con ID {id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica de la persona con ID {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica de la persona con ID {id}", ex);
            }
        }


        // Método para eliminar una persona por su ID
        public async Task<bool> DeletePersonAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una persona con un ID inválido: {PersonId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la persona debe ser mayor a 0");
            }

            try
            {
                var result = await _personData.DeleteAsync(id);

                if (!result)
                {
                    _logger.LogInformation("No se encontró la persona con ID {PersonId} para eliminar", id);
                    throw new EntityNotFoundException("Person", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la persona con ID {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la persona con ID {id}", ex);
            }
        }


        // Método para validar el DTO
        private void ValidatePerson(PersonDto personDto)
        {
            if (personDto == null)
            {
                throw new ValidationException("El objeto persona no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(personDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con Name vacío");
                throw new ValidationException("Name", "El Name de la persona es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(personDto.FirstName))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con FirstName vacío");
                throw new ValidationException("FirstName", "El FirstName de la persona es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(personDto.FirstLastName))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con FirstLastName vacío");
                throw new ValidationException("FirstLastName", "El FirstLastName de la persona es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(personDto.Email))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con Email vacío");
                throw new ValidationException("Email", "El Email de la persona es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(personDto.TypeIdentification))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con TypeIdentification vacío");
                throw new ValidationException("TypeIdentification", "El TypeIdentification de la persona es obligatorio");
            }

            if (personDto.NumberIdentification <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con NumberIdentification inválido");
                throw new ValidationException("NumberIdentification", "El NumberIdentification de la persona debe ser mayor a 0");
            }
        }


        // Método para mapear de Person a PersonDto
        private PersonDto MapToDTO(Person person)
        {
            return new PersonDto
            {
                Id = person.Id,
                Active = person.Active,
                Name = person.Name,
                FirstName = person.FirstName,
                SecondName = person.SecondName,
                FirstLastName = person.FirstLastName,
                SecondLastName = person.SecondLastName,
                PhoneNumber = person.PhoneNumber,
                Email = person.Email,
                TypeIdentification = person.TypeIdentification,
                NumberIdentification = person.NumberIdentification,
                Signig = person.Signig,
            };
        }

        // Método para mapear de PersonDto a Person
        private Person MapToEntity(PersonDto personDto)
        {
            return new Person
            {
                Id = personDto.Id,
                Name = personDto.Name,
                FirstName = personDto.FirstName,
                SecondName = personDto.SecondName,
                FirstLastName = personDto.FirstLastName,
                SecondLastName = personDto.SecondLastName,
                PhoneNumber = personDto.PhoneNumber,
                Email = personDto.Email,
                TypeIdentification = personDto.TypeIdentification,
                NumberIdentification = personDto.NumberIdentification,
                Signig = personDto.Signig,
                Active = personDto.Active,
            };
        }

        // Método para mapear una lista de Person a una lista de PersonDto
        private IEnumerable<PersonDto> MapToDTOList(IEnumerable<Person> persons)
        {
            var personsDTO = new List<PersonDto>();
            foreach (var person in persons)
            {
                personsDTO.Add(MapToDTO(person));
            }
            return personsDTO;
        }
    }
}



