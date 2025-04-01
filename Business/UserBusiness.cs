using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los usuarios.
    /// Implementa la lógica de negocio para la gestión de usuarios.
    /// </summary>
    public class UserBusiness
    {
        // Dependencias inyectadas
        private readonly UserData _userData;        // Acceso a la capa de datos
        private readonly ILogger _logger;         // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="userData">Servicio de acceso a datos para usuarios</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public UserBusiness(UserData userData, ILogger logger)
        {
            _userData = userData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de usuarios en formato DTO</returns>
        public async Task<IEnumerable<UserDTOAuto>> GetAllUsersAsync()
        {
            try
            {
                // Obtener usuarios de la capa de datos
                var users = await _userData.GetAllAsync();
                var usersDTO = new List<UserDTOAuto>();

                // Convertir cada usuario a DTO
                foreach (var user in users)
                {
                    usersDTO.Add(new UserDTOAuto
                    {
                        Id = user.Id,
                        Username = user.username,
                        Email = user.email,
                        Password = user.password,
                        PersonId = user.PersonId
                    });
                }

                return usersDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de usuarios", ex);
            }
        }

        /// <summary>
        /// Obtiene un usuario específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del usuario</param>
        /// <returns>Usuario en formato DTO</returns>
        public async Task<UserDTOAuto> GetUserByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un usuario con ID inválido: {UserId}", id);
                throw new ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }

            try
            {
                // Buscar el usuario en la base de datos
                var user = await _userData.GetByidAsync(id);
                if (user == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario con ID: {UserId}", id);
                    throw new EntityNotFoundException("User", id);
                }

                // Convertir el usuario a DTO
                return new UserDTOAuto
                {
                    Id = user.Id,
                    Username = user.username,
                    Email = user.email,
                    Password = user.password,
                    PersonId = user.PersonId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el usuario con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo usuario en el sistema
        /// </summary>
        /// <param name="userDto">Datos del usuario a crear</param>
        /// <returns>Usuario creado en formato DTO</returns>
        public async Task<UserDTOAuto> CreateUserAsync(UserDTOAuto userDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateUser(userDto);

                // Crear la entidad User desde el DTO
                var user = new User
                {
                    username = userDto.Username,
                    email = userDto.Email,
                    password = userDto.Password,
                    PersonId = userDto.PersonId
                };

                // Guardar el usuario en la base de datos
                var userCreado = await _userData.CreateAsync(user);

                // Convertir el usuario creado a DTO para la respuesta
                return new UserDTOAuto
                {
                    Id = userCreado.Id,
                    Username = userCreado.username,
                    Email = userCreado.email,
                    Password = userCreado.password,
                    PersonId = userCreado.PersonId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo usuario");
                throw new ExternalServiceException("Base de datos", "Error al crear el usuario", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de usuario
        /// </summary>
        /// <param name="userDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateUser(UserDTOAuto userDto)
        {
            // Validar que el DTO no sea nulo
            if (userDto == null)
            {
                throw new ValidationException("El objeto usuario no puede ser nulo");
            }

            // Validar que el username no sea nulo o vacío
            if (string.IsNullOrEmpty(userDto.Username))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con username inválido");
                throw new ValidationException("Username", "El nombre de usuario no puede estar vacío");
            }

            // Validar que el email no sea nulo o vacío
            if (string.IsNullOrEmpty(userDto.Email))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con email inválido");
                throw new ValidationException("Email", "El email no puede estar vacío");
            }

            // Validar que el password no sea nulo o vacío
            if (string.IsNullOrEmpty(userDto.Password))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con password inválido");
                throw new ValidationException("Password", "La contraseña no puede estar vacía");
            }

            // Validar que el PersonId sea válido
            if (userDto.PersonId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con PersonId inválido: {PersonId}", userDto.PersonId);
                throw new ValidationException("PersonId", "El ID de la persona debe ser mayor que cero");
            }
        }
    }
} 