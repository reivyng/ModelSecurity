using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class PersonData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PersonData> _logger;

        public PersonData(ApplicationDbContext context, ILogger<PersonData> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _context.Set<Person>().ToListAsync();
        }

        public async Task<Person?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Person>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener persona con ID {id}");
                throw;
            }
        }

        public async Task<Person> CreateAsync(Person person)
        {
            try
            {
                await _context.Set<Person>().AddAsync(person);
                await _context.SaveChangesAsync();
                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la persona {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Person person)
        {
            var existingPerson = await _context.Person.FindAsync(person.Id);
            if (existingPerson == null)
            {
                _logger.LogWarning("No se encontró la persona con ID {PersonId} para actualizar", person.Id);
                return false;
            }

            // Preservar la fecha de creación
            person.CreateDate = existingPerson.CreateDate;

            // Establecer la fecha de actualización
            person.UpdateDate = DateTime.Now;

            // Actualizar los valores de la entidad
            _context.Entry(existingPerson).CurrentValues.SetValues(person);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var person = await _context.Set<Person>().FindAsync(id);
                if (person == null)
                    return false;

                _context.Set<Person>().Remove(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la persona {ex.Message}");
                return false;
            }
        }
    }
}

