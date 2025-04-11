using Data;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
using Entity.DTOautogestion;
using Entity.DTOs;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los jefes del sistema.
    /// </summary>
    public class BossBusiness
    {
        private readonly BossData _bossData;
        private readonly ILogger<BossBusiness> _logger;

        public BossBusiness(BossData bossData, ILogger<BossBusiness> logger)
        {
            _bossData = bossData;
            _logger = logger;
        }

        // Método para obtener todos los jefes como DTOs
        public async Task<IEnumerable<BossDTO>> GetAllBossesAsync()
        {
            try
            {
                var bosses = await _bossData.GetAllAsync();
                return MapToDTOList(bosses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los jefes");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de jefes", ex);
            }
        }

        // Método para obtener un jefe por su ID como DTO
        public async Task<BossDTO> GetBossByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un jefe con un ID inválido: {BossId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del jefe debe ser mayor a 0");
            }
            try
            {
                var boss = await _bossData.GetByIdAsync(id);
                if (boss == null)
                {
                    _logger.LogInformation("No se encontró el jefe con ID {BossId}", id);
                    throw new EntityNotFoundException("Boss", id);
                }
                return MapToDTO(boss);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el jefe con ID {BossId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el jefe con ID {id}", ex);
            }
        }

        // Método para crear un jefe desde un DTO
        public async Task<BossDTO> CreateBossAsync(BossDTO bossDto)
        {
            try
            {
                ValidateBoss(bossDto);

                var boss = MapToEntity(bossDto);

                var bossCreado = await _bossData.CreateAsync(boss);

                return MapToDTO(bossCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo jefe: {BossName}", bossDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el jefe", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateBoss(BossDTO bossDto)
        {
            if (bossDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto jefe no puede ser nulo");
            }
            if (string.IsNullOrWhiteSpace(bossDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un jefe con nombre vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El nombre del jefe es obligatorio");
            }
        }

        // Método para mapear de Boss a BossDTO
        private BossDTO MapToDTO(Boss boss)
        {
            return new BossDTO
            {
                Id = boss.Id,
                Name = boss.Name,
                EmailBoss = boss.EmailBoss,
                PhoneNumberBoss = boss.PhoneNumberBoss
            };
        }

        // Método para mapear de BossDTO a Boss
        private Boss MapToEntity(BossDTO bossDto)
        {
            return new Boss
            {
                Id = bossDto.Id,
                Name = bossDto.Name,
                EmailBoss = bossDto.EmailBoss,
                PhoneNumberBoss = bossDto.PhoneNumberBoss
            };
        }

        // Método para mapear una lista de Boss a una lista de BossDTO
        private IEnumerable<BossDTO> MapToDTOList(IEnumerable<Boss> bosses)
        {
            var bossesDTO = new List<BossDTO>();
            foreach (var boss in bosses)
            {
                bossesDTO.Add(MapToDTO(boss));
            }
            return bossesDTO;
        }
    }
}
