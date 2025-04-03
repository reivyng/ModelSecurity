using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las empresas.
    /// Implementa la lógica de negocio para la gestión de empresas, incluyendo operaciones CRUD.
    /// </summary>
    public class EnterpriseBusiness
    {
        // Dependencias inyectadas
        private readonly EnterpriseData _enterpriseData; // Acceso a la capa de datos
        private readonly ILogger _logger;               // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="enterpriseData">Servicio de acceso a datos para empresas</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public EnterpriseBusiness(EnterpriseData enterpriseData, ILogger logger)
        {
            _enterpriseData = enterpriseData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las empresas del sistema y las convierte a DTOs
        /// </summary>
        /// <returns>Lista de empresas en formato DTO</returns>
        public async Task<IEnumerable<EnterpriseDTO>> GetAllEnterprisesAsync()
        {
            try
            {
                // Obtener empresas de la capa de datos
                var enterprises = await _enterpriseData.GetAllAsync();
                var enterprisesDTO = new List<EnterpriseDTO>();

                // Convertir cada empresa a DTO
                foreach (var enterprise in enterprises)
                {
                    enterprisesDTO.Add(new EnterpriseDTO
                    {
                        Id = enterprise.Id,
                        Observation = enterprise.Observation,
                        NameBoss = enterprise.NameBoss,
                        NameEnterprise = enterprise.NameEnterprise,
                        PhoneEnterprise = enterprise.PhoneEnterprise,
                        Locate = enterprise.Locate,
                        EmailBoss = enterprise.EmailBoss,
                        NitEnterprise = enterprise.NitEnterprise,
                        EmailEnterprise = enterprise.EmailEnterprise
                    });
                }

                return enterprisesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las empresas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de empresas", ex);
            }
        }

        /// <summary>
        /// Obtiene una empresa específica por su ID
        /// </summary>
        /// <param name="id">Identificador único de la empresa</param>
        /// <returns>Empresa en formato DTO</returns>
        public async Task<EnterpriseDTO> GetEnterpriseByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una empresa con ID inválido: {EnterpriseId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la empresa debe ser mayor que cero");
            }

            try
            {
                // Buscar la empresa en la base de datos
                var enterprise = await _enterpriseData.GetByIdAsync(id);
                if (enterprise == null)
                {
                    _logger.LogInformation("No se encontró ninguna empresa con ID: {EnterpriseId}", id);
                    throw new EntityNotFoundException("Enterprise", id);
                }

                // Convertir la empresa a DTO
                return new EnterpriseDTO
                {
                    Id = enterprise.Id,
                    Observation = enterprise.Observation,
                    NameBoss = enterprise.NameBoss,
                    NameEnterprise = enterprise.NameEnterprise,
                    PhoneEnterprise = enterprise.PhoneEnterprise,
                    Locate = enterprise.Locate,
                    EmailBoss = enterprise.EmailBoss,
                    NitEnterprise = enterprise.NitEnterprise,
                    EmailEnterprise = enterprise.EmailEnterprise
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la empresa con ID: {EnterpriseId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la empresa con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea una nueva empresa en el sistema
        /// </summary>
        /// <param name="enterpriseDto">Datos de la empresa a crear</param>
        /// <returns>Empresa creada en formato DTO</returns>
        public async Task<EnterpriseDTO> CreateEnterpriseAsync(EnterpriseDTO enterpriseDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateEnterprise(enterpriseDto);

                // Crear la entidad Enterprise desde el DTO
                var enterprise = new Enterprise
                {
                    Observation = enterpriseDto.Observation,
                    NameBoss = enterpriseDto.NameBoss,
                    NameEnterprise = enterpriseDto.NameEnterprise,
                    PhoneEnterprise = enterpriseDto.PhoneEnterprise,
                    Locate = enterpriseDto.Locate,
                    EmailBoss = enterpriseDto.EmailBoss,
                    NitEnterprise = enterpriseDto.NitEnterprise,
                    EmailEnterprise = enterpriseDto.EmailEnterprise
                };

                // Guardar la empresa en la base de datos
                var enterpriseCreado = await _enterpriseData.CreateAsync(enterprise);

                // Convertir la empresa creada a DTO para la respuesta
                return new EnterpriseDTO
                {
                    Id = enterprise.Id,
                    Observation = enterprise.Observation,
                    NameBoss = enterprise.NameBoss,
                    NameEnterprise = enterprise.NameEnterprise,
                    PhoneEnterprise = enterprise.PhoneEnterprise,
                    Locate = enterprise.Locate,
                    EmailBoss = enterprise.EmailBoss,
                    NitEnterprise = enterprise.NitEnterprise,
                    EmailEnterprise = enterprise.EmailEnterprise
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva empresa: {EnterpriseName}", enterpriseDto?.NameEnterprise ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la empresa", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de empresa
        /// </summary>
        /// <param name="enterpriseDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateEnterprise(EnterpriseDTO enterpriseDto)
        {
            // Validar que el DTO no sea nulo
            if (enterpriseDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto empresa no puede ser nulo");
            }

            // Validar que el NameEnterprise no esté vacío
            if (string.IsNullOrWhiteSpace(enterpriseDto.NameEnterprise))
            {
                _logger.LogWarning("Se intentó crear/actualizar una empresa con NameEnterprise vacío");
                throw new Utilities.Exceptions.ValidationException("NameEnterprise", "El NameEnterprise de la empresa es obligatorio");
            }
        }
    }
}

