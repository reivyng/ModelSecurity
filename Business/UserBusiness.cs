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
    /// Clase de negocio encargada de la lógica relacionada con los usuarios del sistema.
    /// </summary>
    public class UserBusiness
    {
        private readonly UserData _userData;
        private readonly ILogger<UserBusiness> _logger;

        public UserBusiness(UserData userData, ILogger<UserBusiness> logger)
        {
            _userData = userData;
            _logger = logger;
        }

        // Método para obtener todos los usuarios como DTOs
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userData.GetAllAsync();
                return MapToDTOList(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de usuarios", ex);
            }
        }

        // Método para obtener un usuario por ID como DTO
        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un usuario con ID inválido: {UserId}", id);
                throw new ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }

            try
            {
                var user = await _userData.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario con ID: {UserId}", id);
                    throw new EntityNotFoundException("User", id);
                }

                return MapToDTO(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el usuario con ID {id}", ex);
            }
        }

        // Método para crear un usuario desde un DTO
        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            try
            {
                ValidateUser(userDto);

                var user = MapToEntity(userDto);

                var userCreado = await _userData.CreateAsync(user);

                return MapToDTO(userCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo usuario: {UserName}", userDto?.Username ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el usuario", ex);
            }
        }

        // Método para actualizar un usuario existente
        public async Task<bool> UpdateUserAsync(UserDto userDto)
        {
            try
            {
                // Validar el DTO
                ValidateUser(userDto);

                // Obtener la entidad existente desde la base de datos
                var existingUser = await _userData.GetByIdAsync(userDto.Id);
                if (existingUser == null)
                {
                    _logger.LogWarning("No se encontró el usuario con ID {UserId} para actualizar", userDto.Id);
                    throw new EntityNotFoundException("User", userDto.Id);
                }

                // Actualizar los valores directamente en la entidad existente
                existingUser.Username = userDto.Username;
                existingUser.Email = userDto.Email;
                existingUser.Password = userDto.Password;
                existingUser.Active = userDto.Active;
                existingUser.PersonId = userDto.PersonId;

                // Guardar los cambios en la base de datos
                return await _userData.UpdateAsync(existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario con ID {UserId}", userDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el usuario con ID {userDto.Id}", ex);
            }
        }


        // Método para actualizar campos específicos de un usuario
        public async Task<bool> UpdatePartialUserAsync(int id, UserDto updatedFields)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un usuario con un ID inválido: {UserId}", id);
                throw new ValidationException("id", "El ID del usuario debe ser mayor a 0");
            }

            try
            {
                var existingUser = await _userData.GetByIdAsync(id);
                if (existingUser == null)
                {
                    _logger.LogInformation("No se encontró el usuario con ID {UserId} para actualización parcial", id);
                    throw new EntityNotFoundException("User", id);
                }

                // Actualizar solo los campos proporcionados en el DTO
                if (!string.IsNullOrWhiteSpace(updatedFields.Username))
                {
                    existingUser.Username = updatedFields.Username;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.Email))
                {
                    existingUser.Email = updatedFields.Email;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.Password))
                {
                    existingUser.Password = updatedFields.Password;
                }
                if (updatedFields.Active != existingUser.Active)
                {
                    existingUser.Active = updatedFields.Active;
                }

                return await _userData.UpdateAsync(existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el usuario con ID {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el usuario con ID {id}", ex);
            }
        }

        // Método para realizar una eliminación lógica de un usuario (marcar como inactivo)
        public async Task<bool> SoftDeleteUserAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {UserId}", id);
                throw new ValidationException("id", "El ID del usuario debe ser mayor a 0");
            }

            try
            {
                var user = await _userData.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogInformation("No se encontró el usuario con ID {UserId} para eliminación lógica", id);
                    throw new EntityNotFoundException("User", id);
                }

                user.Active = false;

                return await _userData.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica del usuario con ID {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del usuario con ID {id}", ex);
            }
        }

        // Método para eliminar un usuario por su ID
        public async Task<bool> DeleteUserAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un usuario con un ID inválido: {UserId}", id);
                throw new ValidationException("id", "El ID del usuario debe ser mayor a 0");
            }

            try
            {
                var result = await _userData.DeleteAsync(id);

                if (!result)
                {
                    _logger.LogInformation("No se encontró el usuario con ID {UserId} para eliminar", id);
                    throw new EntityNotFoundException("User", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario con ID {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el usuario con ID {id}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateUser(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new ValidationException("El objeto usuario no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(userDto.Username))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con Username vacío");
                throw new ValidationException("Username", "El Username del usuario es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(userDto.Email))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con Email vacío");
                throw new ValidationException("Email", "El Email del usuario es obligatorio");
            }
        }

        // Método para mapear de User a UserDto
        private UserDto MapToDTO(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Password = user.Password,
                Active = user.Active,
                PersonId = user.PersonId,
            };
        }

        // Método para mapear de UserDto a User
        private User MapToEntity(UserDto userDto)
        {
            return new User
            {
                Id = userDto.Id,
                Username = userDto.Username,
                Email = userDto.Email,
                Password = userDto.Password,
                Active = userDto.Active,
                PersonId = userDto.PersonId,
            };
        }

        // Método para mapear una lista de User a una lista de UserDto
        private IEnumerable<UserDto> MapToDTOList(IEnumerable<User> users)
        {
            var usersDTO = new List<UserDto>();
            foreach (var user in users)
            {
                usersDTO.Add(MapToDTO(user));
            }
            return usersDTO;
        }
    }
}

