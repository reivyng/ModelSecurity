﻿using Data;
using Entity.DTOautogestion;
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
        private readonly ILogger _logger;

        public EnterpriseBusiness(EnterpriseData enterpriseData, ILogger logger)
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

                var enterpriseCreado = await _enterpriseData.CreateAsync(enterprise);

                return MapToDTO(enterpriseCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva empresa: {Name}", enterpriseDto?.NameEnterprise ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la empresa", ex);
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
                AprendizProcessInstructorId = enterprise.AprendizProcessInstructorId
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
                AprendizProcessInstructorId = enterpriseDto.AprendizProcessInstructorId,
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
