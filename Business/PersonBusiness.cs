using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las personas del sistema.
    /// Implementa la lógica de negocio para la gestión de personas, incluyendo operaciones CRUD.
    /// </summary>
    public class PersonBusiness
    {
        // Dependencias inyectadas
        private readonly PersonData _personData;        // Acceso a la capa de datos
        private readonly ILogger _logger;         // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="rolData">Servicio de acceso a datos para personas</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public PersonBusiness(PersonData personData, ILogger logger)
        {
            _personData = personData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las personas del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de personas en formato DTO</returns>
        public async Task<IEnumerable<PersonDTO>> GetAllPersonAsync()
        {
            try
            {
                // Obtener roles de la capa de datos
                var persons = await _personData.GetAllAsync();
                var personDTO = new List<PersonDTO>();

                // Convertir cada persona a DTO
                foreach (var person in persons)
                {
                    personDTO.Add(new PersonDTO
                    {
                        Id = person.Id,
                        Name = person.Name,
                        First_Name = person.First_Name,
                        Second_Name = person.Second_Name,
                        First_Last_Name = person.First_Last_Name,
                        Second_Last_Name = person.Second_Last_Name,
                        Phone_Number = person.Phone_Number,
                        Email = person.Email,
                        Type_Identification = person.Type_Identification,
                        Number_Identification = person.Number_Identification,
                        Signig = person.Signig
                    });
                }

                return personDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos las personas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de personas", ex);
            }
        }

        /// <summary>
        /// Obtiene una persona específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del rol</param>
        /// <returns>Rol en formato DTO</returns>
        public async Task<PersonDTO> GetPersonByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol con ID inválido: {RolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                // Buscar el rol en la base de datos
                var person = await _personData.GetByIdAsync(id);
                if (person == null)
                {
                    _logger.LogInformation("No se encontró ningúna persona con ID: {PersonaId}", id);
                    throw new EntityNotFoundException("Person", id);
                }

                // Convertir el rol a DTO
                return new PersonDTO
                {
                    Id = person.Id,
                    Name = person.Name,
                    First_Name = person.First_Name,
                    Second_Name = person.Second_Name,
                    First_Last_Name = person.First_Last_Name,
                    Second_Last_Name = person.Second_Last_Name,
                    Phone_Number = person.Phone_Number,
                    Email = person.Email,
                    Type_Identification = person.Type_Identification,
                    Number_Identification = person.Number_Identification,
                    Signig = person.Signig
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la persona con ID: {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la persona con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo person en el sistema
        /// </summary>
        /// <param name="personDto">Datos de la persona a crear</param>
        /// <returns>persona creado en formato DTO</returns>
        public async Task<PersonDTO> CreatePersonAsync(PersonDTO personDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidatePerson(personDto);

                // Crear la entidad Rol desde el DTO
                var person = new Person
                {
                    Id = personDto.Id,
                    Name = personDto.Name,
                    First_Name = personDto.First_Name,
                    Second_Name = personDto.Second_Name,
                    First_Last_Name = personDto.First_Last_Name,
                    Second_Last_Name = personDto.Second_Last_Name,
                    Phone_Number = personDto.Phone_Number,
                    Email = personDto.Email,
                    Type_Identification = personDto.Type_Identification,
                    Number_Identification = personDto.Number_Identification,
                    Signig = personDto.Signig
                };

                // Guardar el rol en la base de datos
                var personaCreada = await _personData.CreateAsync(person);

                // Convertir el rol creado a DTO para la respuesta
                return new PersonDTO
                {
                    Id = person.Id,
                    Name = person.Name,
                    First_Name = person.First_Name,
                    Second_Name = person.Second_Name,
                    First_Last_Name = person.First_Last_Name,
                    Second_Last_Name = person.Second_Last_Name,
                    Phone_Number = person.Phone_Number,
                    Email = person.Email,
                    Type_Identification = person.Type_Identification,
                    Number_Identification = person.Number_Identification,
                    Signig = person.Signig
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol: {RolNombre}", personDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de persona
        /// </summary>
        /// <param name="personDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidatePerson(PersonDTO personDto)
        {
            // Validar que el DTO no sea nulo
            if (personDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto rol no puede ser nulo");
            }

            // Validar que el TypeRol no esté vacío
            if (string.IsNullOrWhiteSpace(personDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con TypeRol vacío");
                throw new Utilities.Exceptions.ValidationException("TypeRol", "El TypeRol del rol es obligatorio");
            }
        }
    }
}