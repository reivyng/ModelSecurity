﻿using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los centros en el sistema.
    /// </summary>
    public class CenterBusiness
    {
        private readonly CenterData _centerData;
        private readonly ILogger<CenterBusiness> _logger;

        public CenterBusiness(CenterData centerData, ILogger<CenterBusiness> logger)
        {
            _centerData = centerData;
            _logger = logger;
        }

        // Método para obtener todos los centros como DTOs
        public async Task<IEnumerable<CenterDto>> GetAllCentersAsync()
        {
            try
            {
                var centers = await _centerData.GetAllAsync();
                return MapToDTOList(centers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los centros");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de centros", ex);
            }
        }

        // Método para obtener un centro por ID como DTO
        public async Task<CenterDto> GetCenterByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un centro con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del centro debe ser mayor que cero");
            }

            try
            {
                var center = await _centerData.GetByIdAsync(id);
                if (center == null)
                {
                    _logger.LogInformation("No se encontró ningún centro con ID: {Id}", id);
                    throw new EntityNotFoundException("center", id);
                }

                return MapToDTO(center);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el centro con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el centro con ID {id}", ex);
            }
        }

        // Método para crear un centro desde un DTO
        public async Task<CenterDto> CreateCenterAsync(CenterDto centerDto)
        {
            try
            {
                ValidateCenter(centerDto);

                var center = MapToEntity(centerDto);
                center.CreateDate = DateTime.Now;
                var centerCreado = await _centerData.CreateAsync(center);

                return MapToDTO(centerCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo centro: {Name}", centerDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el centro", ex);
            }
        }

        // Método para actualizar un centro existente
        public async Task<bool> UpdateCenterAsync(CenterDto centerDto)
        {
            try
            {
                ValidateCenter(centerDto);

                var center = MapToEntity(centerDto);

                var result = await _centerData.UpdateAsync(center);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar el centro con ID {CenterId}", centerDto.Id);
                    throw new EntityNotFoundException("Center", centerDto.Id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el centro con ID {CenterId}", centerDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el centro con ID {centerDto.Id}", ex);
            }
        }

        // Método para actualizar campos específicos de un centro
        public async Task<bool> UpdatePartialCenterAsync(int id, CenterDto updatedFields)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un centro con un ID inválido: {CenterId}", id);
                throw new ValidationException("id", "El ID del centro debe ser mayor a 0");
            }

            try
            {
                var existingCenter = await _centerData.GetByIdAsync(id);
                if (existingCenter == null)
                {
                    _logger.LogInformation("No se encontró el centro con ID {CenterId} para actualización parcial", id);
                    throw new EntityNotFoundException("Center", id);
                }

                if (!string.IsNullOrWhiteSpace(updatedFields.Name))
                {
                    existingCenter.Name = updatedFields.Name;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.CodeCenter))
                {
                    existingCenter.CodeCenter = updatedFields.CodeCenter;
                }
                if (updatedFields.Active != existingCenter.Active)
                {
                    existingCenter.Active = updatedFields.Active;
                }
                if (updatedFields.RegionalId > 0 && updatedFields.RegionalId != existingCenter.RegionalId)
                {
                    existingCenter.RegionalId = updatedFields.RegionalId;
                }

                var result = await _centerData.UpdateAsync(existingCenter);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar parcialmente el centro con ID {CenterId}", id);
                    throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el centro con ID {id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el centro con ID {CenterId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el centro con ID {id}", ex);
            }
        }

        // Método para realizar una eliminación lógica de un centro
        public async Task<bool> SoftDeleteCenterAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {CenterId}", id);
                throw new ValidationException("id", "El ID del centro debe ser mayor a 0");
            }

            try
            {
                var center = await _centerData.GetByIdAsync(id);
                if (center == null)
                {
                    _logger.LogInformation("No se encontró el centro con ID {CenterId} para eliminación lógica", id);
                    throw new EntityNotFoundException("Center", id);
                }

                center.Active = false;

                var result = await _centerData.UpdateAsync(center);

                if (!result)
                {
                    _logger.LogWarning("No se pudo realizar la eliminación lógica del centro con ID {CenterId}", id);
                    throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del centro con ID {id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica del centro con ID {CenterId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del centro con ID {id}", ex);
            }
        }

        // Método para eliminar un centro por su ID
        public async Task<bool> DeleteCenterAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un centro con un ID inválido: {CenterId}", id);
                throw new ValidationException("id", "El ID del centro debe ser mayor a 0");
            }

            try
            {
                var result = await _centerData.DeleteAsync(id);

                if (!result)
                {
                    _logger.LogInformation("No se encontró el centro con ID {CenterId} para eliminar", id);
                    throw new EntityNotFoundException("Center", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el centro con ID {CenterId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el centro con ID {id}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateCenter(CenterDto centerDto)
        {
            if (centerDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Center no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(centerDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un centro con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del centro es obligatorio");
            }
        }

        // Método para mapear de Center a CenterDto
        private CenterDto MapToDTO(Center center)
        {
            return new CenterDto
            {
                Id = center.Id,
                Name = center.Name,
                CodeCenter = center.CodeCenter,
                Active = center.Active,
                RegionalId = center.RegionalId,
                Address = center.Address,
            };
        }

        // Método para mapear de CenterDto a Center
        private Center MapToEntity(CenterDto centerDto)
        {
            return new Center
            {
                Id = centerDto.Id,
                Name = centerDto.Name,
                CodeCenter = centerDto.CodeCenter,
                Active = centerDto.Active,
                RegionalId = centerDto.RegionalId,
                Address = centerDto.Address,
            };
        }

        // Método para mapear una lista de Center a una lista de CenterDto
        private IEnumerable<CenterDto> MapToDTOList(IEnumerable<Center> centers)
        {
            var centersDTO = new List<CenterDto>();
            foreach (var center in centers)
            {
                centersDTO.Add(MapToDTO(center));
            }
            return centersDTO;
        }
    }
}
