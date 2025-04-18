﻿using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los aprendices del sistema.
    /// </summary>
    public class AprendizBusiness
    {
        private readonly AprendizData _aprendizData;
        private readonly ILogger<AprendizBusiness> _logger;

        public AprendizBusiness(AprendizData aprendizData, ILogger<AprendizBusiness> logger)
        {
            _aprendizData = aprendizData;
            _logger = logger;
        }

        // Método para obtener todos los aprendices como DTOs
        public async Task<IEnumerable<AprendizDto>> GetAllAprendizAsync()
        {
            try
            {
                var aprendices = await _aprendizData.GetAllAsync();
                return MapToDTOList(aprendices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los aprendices");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de aprendices", ex);
            }
        }

        // Método para obtener un aprendiz por ID como DTO
        public async Task<AprendizDto> GetAprendizByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un aprendiz con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del aprendiz debe ser mayor que cero");
            }

            try
            {
                var aprendiz = await _aprendizData.GetByIdAsync(id);
                if (aprendiz == null)
                {
                    _logger.LogInformation("No se encontró ningún aprendiz con ID: {Id}", id);
                    throw new EntityNotFoundException("Aprendiz", id);
                }
                return MapToDTO(aprendiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el aprendiz con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el aprendiz con ID {id}", ex);
            }
        }

        // Método para crear un usuario desde un DTO
        public async Task<AprendizDto> CreateAprendizAsync(AprendizDto aprendizDto)
        {
            try
            {
                ValidateAprendiz(aprendizDto);

                var aprendiz = MapToEntity(aprendizDto);

                var aprendizCreado = await _aprendizData.CreateAsync(aprendiz);

                return MapToDTO(aprendizCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo aprendiz: {Name}", aprendizDto?.PreviuosProgram ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el aprendiz", ex);
            }
        }

        // Método para actualizar un aprendiz existente
        public async Task<bool> UpdateAprendizAsync(AprendizDto aprendizDto)
        {
            try
            {
                ValidateAprendiz(aprendizDto);

                var aprendiz = MapToEntity(aprendizDto);

                var result = await _aprendizData.UpdateAsync(aprendiz);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar el aprendiz con ID {AprendizId}", aprendizDto.Id);
                    throw new EntityNotFoundException("Aprendiz", aprendizDto.Id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el aprendiz con ID {AprendizId}", aprendizDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el aprendiz con ID {aprendizDto.Id}", ex);
            }
        }

        // Método para actualizar campos específicos de un aprendiz
        public async Task<bool> UpdatePartialAprendizAsync(int id, AprendizDto updatedFields)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un aprendiz con un ID inválido: {AprendizId}", id);
                throw new ValidationException("id", "El ID del aprendiz debe ser mayor a 0");
            }

            try
            {
                // Obtener el aprendiz existente
                var existingAprendiz = await _aprendizData.GetByIdAsync(id);
                if (existingAprendiz == null)
                {
                    _logger.LogInformation("No se encontró el aprendiz con ID {AprendizId} para actualización parcial", id);
                    throw new EntityNotFoundException("Aprendiz", id);
                }

                // Actualizar solo los campos proporcionados en el DTO
                if (!string.IsNullOrWhiteSpace(updatedFields.PreviuosProgram))
                {
                    existingAprendiz.PreviuosProgram = updatedFields.PreviuosProgram;
                }
                if (updatedFields.Active != existingAprendiz.Active) // Si Active es un booleano, siempre se envía un valor
                {
                    existingAprendiz.Active = updatedFields.Active;
                }
                if (updatedFields.UserId > 0 && updatedFields.UserId != existingAprendiz.UserId)
                {
                    existingAprendiz.UserId = updatedFields.UserId;
                }

                // Guardar los cambios
                var result = await _aprendizData.UpdateAsync(existingAprendiz);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar parcialmente el aprendiz con ID {AprendizId}", id);
                    throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el aprendiz con ID {id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el aprendiz con ID {AprendizId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el aprendiz con ID {id}", ex);
            }
        }

        // Método para realizar una eliminación lógica de un aprendiz (marcar como inactivo)
        public async Task<bool> SoftDeleteAprendizAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {AprendizId}", id);
                throw new ValidationException("id", "El ID del aprendiz debe ser mayor a 0");
            }

            try
            {
                // Obtener el aprendiz por ID
                var aprendiz = await _aprendizData.GetByIdAsync(id);
                if (aprendiz == null)
                {
                    _logger.LogInformation("No se encontró el aprendiz con ID {AprendizId} para eliminación lógica", id);
                    throw new EntityNotFoundException("Aprendiz", id);
                }

                // Marcar el aprendiz como inactivo
                aprendiz.Active = false;

                // Actualizar el aprendiz en la base de datos
                var result = await _aprendizData.UpdateAsync(aprendiz);

                if (!result)
                {
                    _logger.LogWarning("No se pudo realizar la eliminación lógica del aprendiz con ID {AprendizId}", id);
                    throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del aprendiz con ID {id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica del aprendiz con ID {AprendizId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del aprendiz con ID {id}", ex);
            }
        }

        // Método para eliminar un aprendiz por su ID
        public async Task<bool> DeleteAprendizAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un aprendiz con un ID inválido: {AprendizId}", id);
                throw new ValidationException("id", "El ID del aprendiz debe ser mayor a 0");
            }

            try
            {
                var result = await _aprendizData.DeleteAsync(id);

                if (!result)
                {
                    _logger.LogInformation("No se encontró el aprendiz con ID {AprendizId} para eliminar", id);
                    throw new EntityNotFoundException("Aprendiz", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el aprendiz con ID {AprendizId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el aprendiz con ID {id}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateAprendiz(AprendizDto aprendizDto)
        {
            if (aprendizDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto aprendiz no puede ser nulo");
            }
        }

        // Método para mapear de Aprendiz a AprendizDto
        private AprendizDto MapToDTO(Aprendiz aprendiz)
        {
            return new AprendizDto
            {
                Id = aprendiz.Id,
                PreviuosProgram = aprendiz.PreviuosProgram,
                UserId = aprendiz.UserId,
                Active = aprendiz.Active,    
            };
        }

        // Método para mapear de AprendizDto a Aprendiz
        private Aprendiz MapToEntity(AprendizDto aprendizDto)
        {
            return new Aprendiz
            {
                Id = aprendizDto.Id,
                PreviuosProgram = aprendizDto.PreviuosProgram,
                UserId = aprendizDto.UserId,
                Active = aprendizDto.Active,
            };
        }

        // Método para mapear una lista de Rol a una lista de RolDTO
        private IEnumerable<AprendizDto> MapToDTOList(IEnumerable<Aprendiz> aprendiz)
        {
            var aprendizDTO = new List<AprendizDto>();
            foreach (var rol in aprendiz)
            {
                aprendizDTO.Add(MapToDTO(rol));
            }
            return aprendizDTO;
        }
    }
}
