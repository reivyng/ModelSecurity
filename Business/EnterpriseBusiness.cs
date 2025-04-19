using Data;
using Entity.DTOautogestion;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las empresas en el sistema.
    /// </summary>
    public class EnterpriseBusiness
    {
        private readonly EnterpriseData _enterpriseData;
        private readonly ILogger<EnterpriseBusiness> _logger;

        public EnterpriseBusiness(EnterpriseData enterpriseData, ILogger<EnterpriseBusiness> logger)
        {
            _enterpriseData = enterpriseData;
            _logger = logger;
        }

        // Método para obtener todas las empresas como DTOs
        public async Task<IEnumerable<EnterpriseDto>> GetAllEnterprisesAsync()
        {
            try
            {
                var enterprises = await _enterpriseData.GetAllAsync();
                return MapToDTOList(enterprises);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las empresas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de empresas", ex);
            }
        }

        // Método para obtener una empresa por ID como DTO
        public async Task<EnterpriseDto> GetEnterpriseByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una empresa con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la empresa debe ser mayor que cero");
            }

            try
            {
                var enterprise = await _enterpriseData.GetByIdAsync(id);
                if (enterprise == null)
                {
                    _logger.LogInformation("No se encontró ninguna empresa con ID: {Id}", id);
                    throw new EntityNotFoundException("enterprise", id);
                }

                return MapToDTO(enterprise);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la empresa con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la empresa con ID {id}", ex);
            }
        }

        // Método para crear una empresa desde un DTO
        public async Task<EnterpriseDto> CreateEnterpriseAsync(EnterpriseDto enterpriseDto)
        {
            try
            {
                ValidateEnterprise(enterpriseDto);

                var enterprise = MapToEntity(enterpriseDto);
                enterprise.CreateDate = DateTime.Now;
                var enterpriseCreado = await _enterpriseData.CreateAsync(enterprise);

                return MapToDTO(enterpriseCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva empresa: {Name}", enterpriseDto?.NameEnterprise ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la empresa", ex);
            }
        }

        // Método para actualizar una empresa existente
        public async Task<bool> UpdateEnterpriseAsync(EnterpriseDto enterpriseDto)
        {
            try
            {
                ValidateEnterprise(enterpriseDto);

                var enterprise = MapToEntity(enterpriseDto);

                var result = await _enterpriseData.UpdateAsync(enterprise);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar la empresa con ID {EnterpriseId}", enterpriseDto.Id);
                    throw new EntityNotFoundException("Enterprise", enterpriseDto.Id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la empresa con ID {EnterpriseId}", enterpriseDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la empresa con ID {enterpriseDto.Id}", ex);
            }
        }

        // Método para actualizar campos específicos de una empresa
        public async Task<bool> UpdatePartialEnterpriseAsync(int id, EnterpriseDto updatedFields)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar una empresa con un ID inválido: {EnterpriseId}", id);
                throw new ValidationException("id", "El ID de la empresa debe ser mayor a 0");
            }

            try
            {
                var existingEnterprise = await _enterpriseData.GetByIdAsync(id);
                if (existingEnterprise == null)
                {
                    _logger.LogInformation("No se encontró la empresa con ID {EnterpriseId} para actualización parcial", id);
                    throw new EntityNotFoundException("Enterprise", id);
                }

                if (!string.IsNullOrWhiteSpace(updatedFields.NameEnterprise))
                {
                    existingEnterprise.NameEnterprise = updatedFields.NameEnterprise;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.NitEnterprise))
                {
                    existingEnterprise.NitEnterprise = updatedFields.NitEnterprise;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.Locate))
                {
                    existingEnterprise.Locate = updatedFields.Locate;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.Observation))
                {
                    existingEnterprise.Observation = updatedFields.Observation;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.PhoneEnterprise))
                {
                    existingEnterprise.PhoneEnterprise = updatedFields.PhoneEnterprise;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.EmailEnterprise))
                {
                    existingEnterprise.EmailEnterprise = updatedFields.EmailEnterprise;
                }
                if (updatedFields.Active != existingEnterprise.Active)
                {
                    existingEnterprise.Active = updatedFields.Active;
                }

                var result = await _enterpriseData.UpdateAsync(existingEnterprise);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar parcialmente la empresa con ID {EnterpriseId}", id);
                    throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la empresa con ID {id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la empresa con ID {EnterpriseId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la empresa con ID {id}", ex);
            }
        }

        // Método para realizar una eliminación lógica de una empresa
        public async Task<bool> SoftDeleteEnterpriseAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {EnterpriseId}", id);
                throw new ValidationException("id", "El ID de la empresa debe ser mayor a 0");
            }

            try
            {
                var enterprise = await _enterpriseData.GetByIdAsync(id);
                if (enterprise == null)
                {
                    _logger.LogInformation("No se encontró la empresa con ID {EnterpriseId} para eliminación lógica", id);
                    throw new EntityNotFoundException("Enterprise", id);
                }

                enterprise.Active = false;

                var result = await _enterpriseData.UpdateAsync(enterprise);

                if (!result)
                {
                    _logger.LogWarning("No se pudo realizar la eliminación lógica de la empresa con ID {EnterpriseId}", id);
                    throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica de la empresa con ID {id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica de la empresa con ID {EnterpriseId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica de la empresa con ID {id}", ex);
            }
        }

        // Método para eliminar una empresa por su ID
        public async Task<bool> DeleteEnterpriseAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una empresa con un ID inválido: {EnterpriseId}", id);
                throw new ValidationException("id", "El ID de la empresa debe ser mayor a 0");
            }

            try
            {
                var result = await _enterpriseData.DeleteAsync(id);

                if (!result)
                {
                    _logger.LogInformation("No se encontró la empresa con ID {EnterpriseId} para eliminar", id);
                    throw new EntityNotFoundException("Enterprise", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la empresa con ID {EnterpriseId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la empresa con ID {id}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateEnterprise(EnterpriseDto enterpriseDto)
        {
            if (enterpriseDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Enterprise no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(enterpriseDto.NameEnterprise))
            {
                _logger.LogWarning("Se intentó crear/actualizar una empresa con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la empresa es obligatorio");
            }
        }

        // Método para mapear de Enterprise a EnterpriseDto
        private EnterpriseDto MapToDTO(Enterprise enterprise)
        {
            return new EnterpriseDto
            {
                Id = enterprise.Id,
                NameEnterprise = enterprise.NameEnterprise,
                NitEnterprise = enterprise.NitEnterprise,
                Locate = enterprise.Locate,
                Observation = enterprise.Observation,
                PhoneEnterprise = enterprise.PhoneEnterprise,
                EmailEnterprise = enterprise.EmailEnterprise,
                Active = enterprise.Active,
            };
        }

        // Método para mapear de EnterpriseDto a Enterprise
        private Enterprise MapToEntity(EnterpriseDto enterpriseDto)
        {
            return new Enterprise
            {
                Id = enterpriseDto.Id,
                NameEnterprise = enterpriseDto.NameEnterprise,
                NitEnterprise = enterpriseDto.NitEnterprise,
                Locate = enterpriseDto.Locate,
                Observation = enterpriseDto.Observation,
                PhoneEnterprise = enterpriseDto.PhoneEnterprise,
                EmailEnterprise = enterpriseDto.EmailEnterprise,
                Active = enterpriseDto.Active,
            };
        }

        // Método para mapear una lista de Enterprise a una lista de EnterpriseDto
        private IEnumerable<EnterpriseDto> MapToDTOList(IEnumerable<Enterprise> enterprises)
        {
            var enterprisesDTO = new List<EnterpriseDto>();
            foreach (var enterprise in enterprises)
            {
                enterprisesDTO.Add(MapToDTO(enterprise));
            }
            return enterprisesDTO;
        }
    }
}
