﻿using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las sedes en el sistema.
    /// </summary>
    public class SedeBusiness
    {
        private readonly SedeData _sedeData;
        private readonly ILogger<SedeBusiness> _logger;

        public SedeBusiness(SedeData sedeData, ILogger<SedeBusiness> logger)
        {
            _sedeData = sedeData;
            _logger = logger;
        }

        // Método para obtener todas las sedes como DTOs
        public async Task<IEnumerable<SedeDto>> GetAllSedesAsync()
        {
            try
            {
                var sedes = await _sedeData.GetAllAsync();
                return MapToDTOList(sedes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las sedes");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de sedes", ex);
            }
        }

        // Método para obtener una sede por ID como DTO
        public async Task<SedeDto> GetSedeByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una sede con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la sede debe ser mayor que cero");
            }

            try
            {
                var sede = await _sedeData.GetByIdAsync(id);
                if (sede == null)
                {
                    _logger.LogInformation("No se encontró ninguna sede con ID: {Id}", id);
                    throw new EntityNotFoundException("sede", id);
                }

                return MapToDTO(sede);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la sede con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la sede con ID {id}", ex);
            }
        }

        // Método para crear una sede desde un DTO
        public async Task<SedeDto> CreateSedeAsync(SedeDto sedeDto)
        {
            try
            {
                ValidateSede(sedeDto);

                var sede = MapToEntity(sedeDto);

                var sedeCreada = await _sedeData.CreateAsync(sede);

                return MapToDTO(sedeCreada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva sede: {Name}", sedeDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la sede", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateSede(SedeDto sedeDto)
        {
            if (sedeDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Sede no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(sedeDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una sede con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la sede es obligatorio");
            }
        }

        // Método para mapear de Sede a SedeDto
        private SedeDto MapToDTO(Sede sede)
        {
            return new SedeDto
            {
                Id = sede.Id,
                Name = sede.Name,
                CodeSede = sede.CodeSede,
                Address = sede.Address,
                PhoneSede = sede.PhoneSede,
                EmailContact = sede.EmailContact,
                CenterId = sede.CenterId,
                Active = sede.Active,
            };
        }

        // Método para mapear de SedeDto a Sede
        private Sede MapToEntity(SedeDto sedeDto)
        {
            return new Sede
            {
                Id = sedeDto.Id,
                Name = sedeDto.Name,
                CodeSede = sedeDto.CodeSede,
                Address = sedeDto.Address,
                PhoneSede = sedeDto.PhoneSede,
                EmailContact = sedeDto.EmailContact,
                CenterId = sedeDto.CenterId,
                Active = sedeDto.Active,
            };
        }

        // Método para mapear una lista de Sede a una lista de SedeDto
        private IEnumerable<SedeDto> MapToDTOList(IEnumerable<Sede> sedes)
        {
            var sedesDTO = new List<SedeDto>();
            foreach (var sede in sedes)
            {
                sedesDTO.Add(MapToDTO(sede));
            }
            return sedesDTO;
        }
    }
}






 