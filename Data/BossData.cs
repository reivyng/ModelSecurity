using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class BossData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BossData> _logger;

        public BossData(ApplicationDbContext context, ILogger<BossData> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Boss>> GetAllAsync()
        {
            return await _context.Set<Boss>().ToListAsync();
        }

        public async Task<Boss?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Boss>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener jefe con ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<Boss> CreateAsync(Boss boss)
        {
            try
            {
                await _context.Set<Boss>().AddAsync(boss);
                await _context.SaveChangesAsync();
                return boss;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el jefe: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Boss boss)
        {
            try
            {
                _context.Set<Boss>().Update(boss);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el jefe: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var boss = await _context.Set<Boss>().FindAsync(id);
                if (boss == null)
                    return false;

                _context.Set<Boss>().Remove(boss);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el jefe: {ex.Message}");
                return false;
            }
        }
    }
}
