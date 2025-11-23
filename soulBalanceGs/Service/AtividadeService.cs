using Microsoft.EntityFrameworkCore;
using soulBalanceGs.Data;
using soulBalanceGs.DTOs;
using soulBalanceGs.Exceptions;
using soulBalanceGs.Models;
using soulBalanceGs.Repository;

namespace soulBalanceGs.Service
{
    public class AtividadeService : IService<AtividadeResponseDto, AtividadeRequestDto>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IAtividadeRepository _atividadeRepository;

        private const string USUARIO_NOT_FOUND_MESSAGE = "Usuário com o e-mail '{0}' não encontrado.";
        private const string ATIVIDADE_NOT_FOUND_MESSAGE = "Atividade com o ID {0} não encontrada.";
        private const string ATIVIDADE_USER_NOT_FOUND_MESSAGE = "Atividade não encontrada para o usuário e ID especificados.";
        private const string TEMPO_INVALIDO_MESSAGE = "O horário de início deve ser anterior ao horário de fim da atividade.";
        private const string UPDATE_CONFLICT_MESSAGE = "Não é permitido atualizar atividades de outro usuário.";

        public AtividadeService(AppDbContext appDbContext, IAtividadeRepository atividadeRepository)
        {
            _appDbContext = appDbContext;
            _atividadeRepository = atividadeRepository;
        }

        public async Task<AtividadeResponseDto> Save(AtividadeRequestDto dto)
        {
            if (dto.Inicio >= dto.Fim)
            {
                throw new ConflictException(TEMPO_INVALIDO_MESSAGE);
            }

            var usuario = await ValidarUsuarioAsync(dto.Email);

            var duracao = CalcularDuracaoMinutos(dto.Inicio, dto.Fim);

            var atividade = new Atividade
            {
                TipoAtividade = dto.TipoAtividade,
                Descricao = dto.Descricao,
                Inicio = dto.Inicio,
                Fim = dto.Fim,
                DuracaoMinutosAtividade = duracao,
                FkIdUsuario = usuario.IdUsuario
            };

            _appDbContext.Atividades.Add(atividade);
            await _appDbContext.SaveChangesAsync();

            return AtividadeResponseDto.FromEntity(atividade);
        }

        public async Task<IEnumerable<AtividadeResponseDto>> GetAll(int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            return await _appDbContext.Atividades
                .AsNoTracking()
                .OrderBy(a => a.AtividadeId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => AtividadeResponseDto.FromEntity(a))
                .ToListAsync();
        }

        public async Task<AtividadeResponseDto> GetById(int id)
        {
            var atividade = await _appDbContext.Atividades
                .AsNoTracking()
                .Where(a => a.AtividadeId == id)
                .Select(a => AtividadeResponseDto.FromEntity(a))
                .FirstOrDefaultAsync();

            if (atividade == null) throw new KeyNotFoundException(string.Format(ATIVIDADE_NOT_FOUND_MESSAGE, id));

            return atividade;
        }

        public async Task<AtividadeResponseDto> Update(int id, AtividadeRequestDto dto)
        {
            if (dto.Inicio >= dto.Fim)
            {
                throw new ConflictException(TEMPO_INVALIDO_MESSAGE);
            }

            var entity = await _appDbContext.Atividades.FindAsync(id);

            if (entity == null) throw new KeyNotFoundException(string.Format(ATIVIDADE_NOT_FOUND_MESSAGE, id));

            var usuario = await ValidarUsuarioAsync(dto.Email);

            if (entity.FkIdUsuario != usuario.IdUsuario)
            {
                throw new ConflictException(UPDATE_CONFLICT_MESSAGE);
            }

            var novaDuracao = CalcularDuracaoMinutos(dto.Inicio, dto.Fim);

            entity.TipoAtividade = dto.TipoAtividade;
            entity.Descricao = dto.Descricao;
            entity.Inicio = dto.Inicio;
            entity.Fim = dto.Fim;
            entity.DuracaoMinutosAtividade = novaDuracao;

            await _appDbContext.SaveChangesAsync();

            return AtividadeResponseDto.FromEntity(entity);
        }

        public async Task DeleteById(int id)
        {
            var entity = await _appDbContext.Atividades.FindAsync(id);
            if (entity == null) throw new KeyNotFoundException(string.Format(ATIVIDADE_NOT_FOUND_MESSAGE, id));

            _appDbContext.Atividades.Remove(entity);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<AtividadeResponseDto> BuscarHistoricoPorPeriodo(long userId, long atividadeId)
        {
            var atividade = await _atividadeRepository.FindByUsuarioIdAndAtividadeIdAsync(userId, atividadeId);

            if (atividade == null)
            {
                throw new KeyNotFoundException(ATIVIDADE_USER_NOT_FOUND_MESSAGE);
            }

            return AtividadeResponseDto.FromEntity(atividade);
        }

        public async Task<List<AtividadeResponseDto>> GetAllByUsuario(long userId)
        {
            var atividades = await _appDbContext.Atividades
               .AsNoTracking()
               .Where(a => a.FkIdUsuario == userId)
               .OrderByDescending(a => a.Inicio)
               .ToListAsync();

            return atividades.Select(AtividadeResponseDto.FromEntity).ToList();
        }

        public async Task DeleteByUsuarioIdAndAtividadeId(long userId, long atividadeId)
        {
            var atividade = await _atividadeRepository.FindByUsuarioIdAndAtividadeIdAsync(userId, atividadeId);

            if (atividade == null)
            {
                throw new KeyNotFoundException(string.Format(ATIVIDADE_NOT_FOUND_MESSAGE, atividadeId));
            }

            _appDbContext.Atividades.Remove(atividade);
            await _appDbContext.SaveChangesAsync();
        }

        private long CalcularDuracaoMinutos(DateTime inicio, DateTime fim)
        {
            TimeSpan duracao = fim.Subtract(inicio);
            return (long)duracao.TotalMinutes;
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
    }
}