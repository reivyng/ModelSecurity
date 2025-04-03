using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las sedes.
    /// Implementa la lógica de negocio para la gestión de sedes, incluyendo operaciones CRUD.
    /// </summary>
    internal class SedeBusiness
    {
        // Dependencias inyectadas
        private readonly SedeData _sedeData; // Acceso a la capa de datos
        private readonly ILogger _logger;   // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="sedeData">Servicio de acceso a datos para sedes</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public SedeBusiness(SedeData sedeData, ILogger logger)
        {
            _sedeData = sedeData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las sedes del sistema y las convierte a DTOs
        /// </summary>
        /// <returns>Lista de sedes en formato DTO</returns>
        public async Task<IEnumerable<SedeDTO>> GetAllSedesAsync()
        {
            try
            {
                // Obtener sedes de la capa de datos
                var sedes = await _sedeData.GetAllAsync();
                var sedesDTO = new List<SedeDTO>();

                // Convertir cada sede a DTO
                foreach (var sede in sedes)
                {
                    sedesDTO.Add(new SedeDTO
                    {
                        Id = sede.Id,
                        Name = sede.Name,
                        CodeSede = sede.CodeSede,
                        Addres = sede.Addres,
                        PhoneSede = sede.PhoneSede,
                        EmailContact = sede.EmailContact,
                        CenterId = sede.CenterId
                    });
                }

                return sedesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las sedes");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de sedes", ex);
            }
        }

        /// <summary>
        /// Obtiene una sede específica por su ID
        /// </summary>
        /// <param name="id">Identificador único de la sede</param>
        /// <returns>Sede en formato DTO</returns>
        public async Task<SedeDTO> GetSedeByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una sede con ID inválido: {SedeId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la sede debe ser mayor que cero");
            }

            try
            {
                // Buscar la sede en la base de datos
                var sede = await _sedeData.GetByIdAsync(id);
                if (sede == null)
                {
                    _logger.LogInformation("No se encontró ninguna sede con ID: {SedeId}", id);
                    throw new EntityNotFoundException("Sede", id);
                }

                // Convertir la sede a DTO
                return new SedeDTO
                {
                    Id = sede.Id,
                    Name = sede.Name,
                    CodeSede = sede.CodeSede,
                    Addres = sede.Addres,
                    PhoneSede = sede.PhoneSede,
                    EmailContact = sede.EmailContact,
                    CenterId = sede.CenterId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la sede con ID: {SedeId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la sede con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea una nueva sede en el sistema
        /// </summary>
        /// <param name="sedeDto">Datos de la sede a crear</param>
        /// <returns>Sede creada en formato DTO</returns>
        public async Task<SedeDTO> CreateSedeAsync(SedeDTO sedeDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateSede(sedeDto);

                // Crear la entidad Sede desde el DTO
                var sede = new Sede
                {
                    Name = sedeDto.Name,
                    CodeSede = sedeDto.CodeSede,
                    Addres = sedeDto.Addres,
                    PhoneSede = sedeDto.PhoneSede,
                    EmailContact = sedeDto.EmailContact,
                    CenterId = sedeDto.CenterId
                };

                // Guardar la sede en la base de datos
                var sedeCreada = await _sedeData.CreateAsync(sede);

                // Convertir la sede creada a DTO para la respuesta
                return new SedeDTO
                {
                    Id = sede.Id,
                    Name = sede.Name,
                    CodeSede = sede.CodeSede,
                    Addres = sede.Addres,
                    PhoneSede = sede.PhoneSede,
                    EmailContact = sede.EmailContact,
                    CenterId = sede.CenterId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva sede: {SedeName}", sedeDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la sede", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de sede
        /// </summary>
        /// <param name="sedeDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateSede(SedeDTO sedeDto)
        {
            // Validar que el DTO no sea nulo
            if (sedeDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto sede no puede ser nulo");
            }

            // Validar que el Name no esté vacío
            if (string.IsNullOrWhiteSpace(sedeDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una sede con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la sede es obligatorio");
            }
        }
    }
}





