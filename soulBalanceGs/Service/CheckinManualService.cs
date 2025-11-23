using Microsoft.EntityFrameworkCore;
using soulBalanceGs.Data;
using soulBalanceGs.DTOs;
using soulBalanceGs.Exceptions;
using soulBalanceGs.Models;
using soulBalanceGs.Repository;

namespace soulBalanceGs.Service
{
    public class CheckinManualService : IService<CheckinManualResponseDto, CheckinManualRequestDto>
    {
        private readonly AppDbContext _appDbContext;
        private readonly ICheckinManualRepository _checkinManualRepository;

        private const string USUARIO_NOT_FOUND_MESSAGE = "Usuário com o e-mail '{0}' não encontrado.";
        private const string CHECKIN_NOT_FOUND_MESSAGE = "Check-in com o ID {0} não encontrado.";

        public CheckinManualService(AppDbContext appDbContext, ICheckinManualRepository checkinManualRepository)
        {
            _appDbContext = appDbContext;
            _checkinManualRepository = checkinManualRepository;
        }

        public async Task<CheckinManualResponseDto> Save(CheckinManualRequestDto dto)
        {
            var usuario = await ValidarUsuarioAsync(dto.Email);

            var checkin = new CheckinManual
            {
                Humor = dto.Humor,
                Energia = dto.Energia,
                Foco = dto.Foco,
                Time = DateTime.Now,
                FkIdUsuario = usuario.IdUsuario
            };

            _appDbContext.CheckinManuais.Add(checkin);
            await _appDbContext.SaveChangesAsync();

            return CheckinManualResponseDto.FromEntity(checkin);
        }

        public async Task<IEnumerable<CheckinManualResponseDto>> GetAllPagination(int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            return await _appDbContext.CheckinManuais
                .AsNoTracking()
                .OrderBy(c => c.ChekinId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => CheckinManualResponseDto.FromEntity(c))
                .ToListAsync();
        }

        public async Task<CheckinManualResponseDto> GetById(int id)
        {
            var checkin = await _appDbContext.CheckinManuais
                .AsNoTracking()
                .Where(c => c.ChekinId == id)
                .Select(c => CheckinManualResponseDto.FromEntity(c))
                .FirstOrDefaultAsync();

            if (checkin == null) throw new KeyNotFoundException(string.Format(CHECKIN_NOT_FOUND_MESSAGE, id));

            return checkin;
        }

        public async Task<CheckinManualResponseDto> Update(int id, CheckinManualRequestDto dto)
        {
            var entity = await _appDbContext.CheckinManuais.FindAsync(id);

            if (entity == null) throw new KeyNotFoundException(string.Format(CHECKIN_NOT_FOUND_MESSAGE, id));

            var usuario = await ValidarUsuarioAsync(dto.Email);
            if (entity.FkIdUsuario != usuario.IdUsuario)
            {
                throw new ConflictException("Não é permitido atualizar check-ins de outro usuário.");
            }

            entity.Humor = dto.Humor;
            entity.Energia = dto.Energia;
            entity.Foco = dto.Foco;
            entity.Time = DateTime.Now;

            await _appDbContext.SaveChangesAsync();

            return CheckinManualResponseDto.FromEntity(entity);
        }

        public async Task DeleteById(int id)
        {
            var entity = await _appDbContext.CheckinManuais.FindAsync(id);
            if (entity == null) throw new KeyNotFoundException(string.Format(CHECKIN_NOT_FOUND_MESSAGE, id));

            _appDbContext.CheckinManuais.Remove(entity);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<List<CheckinManualResponseDto>> GetAllByUsuario(int userId)
        {
            var historico = await _checkinManualRepository.FindByUsuarioIdAsync(userId);

            return historico.Select(c => CheckinManualResponseDto.FromEntity(c)).ToList();
        }

        public async Task<List<CheckinManualResponseDto>> GetAll()
        {
            return await _appDbContext.CheckinManuais
                .AsNoTracking()
                .Select(c => CheckinManualResponseDto.FromEntity(c))
                .ToListAsync();
        }

        public async Task<int> DeleteUserChekin(int userId, int chekinId)
        {
            var usuario = await _appDbContext.Usuarios.FindAsync((int)userId);
            if (usuario == null)
            {
                throw new KeyNotFoundException(string.Format(USUARIO_NOT_FOUND_MESSAGE, userId));
            }

            var checkin = await _appDbContext.CheckinManuais.FindAsync((int)chekinId);
            if (checkin == null)
            {
                throw new KeyNotFoundException(string.Format(CHECKIN_NOT_FOUND_MESSAGE, chekinId));
            }

            return await _checkinManualRepository.DeleteByUsuarioIdAndChekinIdAsync(userId, chekinId);
        }

        private async Task<Usuario> ValidarUsuarioAsync(string email)
        {
            var usuario = await _appDbContext.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null)
            {
                throw new KeyNotFoundException(string.Format(USUARIO_NOT_FOUND_MESSAGE, email));
            }

            return usuario;
        }

        public Task<IEnumerable<CheckinManualResponseDto>> GetAll(int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}