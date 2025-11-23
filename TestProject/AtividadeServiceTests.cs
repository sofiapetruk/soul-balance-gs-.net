using Microsoft.EntityFrameworkCore;
using soulBalanceGs.Data;
using soulBalanceGs.Models;
using soulBalanceGs.Repository;
using soulBalanceGs.Service;
using Moq;
using soulBalanceGs.Enuns;

namespace TestProject
{
    public class AtividadeServiceTests
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new AppDbContext(options);
            dbContext.Database.EnsureCreated();
            return dbContext;
        }

        [Fact]
        public async Task GetById_ShouldReturnAtividade_WhenFound()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var mockRepo = new Mock<IAtividadeRepository>();
            var atividadeId = 5;
            context.Atividades.Add(new Atividade
            {
                AtividadeId = atividadeId,
                TipoAtividade = TipoAtividade.TRABALHO_FOCO,
                FkIdUsuario = 101,
                Inicio = DateTime.Now.AddHours(-1),
                Fim = DateTime.Now,
                DuracaoMinutosAtividade = 60
            });
            await context.SaveChangesAsync();

            var service = new AtividadeService(context, mockRepo.Object);

            var resultDto = await service.GetById(atividadeId);

            Assert.NotNull(resultDto);
            Assert.Equal(atividadeId, resultDto.AtividadeId);
            Assert.Equal(TipoAtividade.TRABALHO_FOCO, resultDto.TipoAtividade);
        }

        [Fact]
        public async Task GetAll_ShouldReturnPaginatedList()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var mockRepo = new Mock<IAtividadeRepository>();

            for (int i = 1; i <= 5; i++)
            {
                context.Atividades.Add(new Atividade { AtividadeId = i, TipoAtividade = TipoAtividade.LAZER_SOCIAL, FkIdUsuario = 101, Inicio = DateTime.Now.AddDays(-i), Fim = DateTime.Now.AddDays(-i).AddHours(1), DuracaoMinutosAtividade = 60 });
            }
            await context.SaveChangesAsync();

            var service = new AtividadeService(context, mockRepo.Object);
            var page = 2;
            var pageSize = 2;

            var result = await service.GetAll(page, pageSize);

            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Equal(3, resultList[0].AtividadeId);
            Assert.Equal(4, resultList[1].AtividadeId);
        }

        [Fact]
        public async Task GetAllByUsuario_ShouldReturnOnlyUserActivitiesOrderedByInicio()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var mockRepo = new Mock<IAtividadeRepository>();
            var targetUserId = 200;
            var otherUserId = 201;

            context.Atividades.Add(new Atividade { AtividadeId = 1, FkIdUsuario = targetUserId, TipoAtividade = TipoAtividade.ESTUDO_APRENDIZADO, Inicio = new DateTime(2025, 1, 10), Fim = new DateTime(2025, 1, 10).AddHours(1), DuracaoMinutosAtividade = 60 });
            context.Atividades.Add(new Atividade { AtividadeId = 3, FkIdUsuario = targetUserId, TipoAtividade = TipoAtividade.TRABALHO_FOCO, Inicio = new DateTime(2025, 1, 12), Fim = new DateTime(2025, 1, 12).AddHours(1), DuracaoMinutosAtividade = 60 });

            context.Atividades.Add(new Atividade { AtividadeId = 2, FkIdUsuario = otherUserId, TipoAtividade = TipoAtividade.DESCANSO_PASSIVO, Inicio = new DateTime(2025, 1, 11), Fim = new DateTime(2025, 1, 11).AddHours(1), DuracaoMinutosAtividade = 60 });

            await context.SaveChangesAsync();

            var service = new AtividadeService(context, mockRepo.Object);

            var result = await service.GetAllByUsuario(targetUserId);

            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);

            Assert.Equal(3, resultList[0].AtividadeId);
            Assert.Equal(1, resultList[1].AtividadeId);
        }

        [Fact]
        public async Task DeleteById_ShouldRemoveActivity_WhenFound()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var mockRepo = new Mock<IAtividadeRepository>();
            var atividadeId = 7;
            context.Atividades.Add(new Atividade
            {
                AtividadeId = atividadeId,
                TipoAtividade = TipoAtividade.EXERCICIO_FISICO,
                FkIdUsuario = 101,
                Inicio = DateTime.Now,
                Fim = DateTime.Now.AddMinutes(1)
            });
            await context.SaveChangesAsync();

            var service = new AtividadeService(context, mockRepo.Object);

            await service.DeleteById(atividadeId);

            var deletedEntity = await context.Atividades.FindAsync(atividadeId);
            Assert.Null(deletedEntity);
        }

        [Fact]
        public async Task DeleteByUsuarioIdAndAtividadeId_ShouldDeleteActivity_WhenFound()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var userId = 101L;
            var atividadeId = 6;
            var activityToDelete = new Atividade
            {
                AtividadeId = atividadeId,
                FkIdUsuario = (int)userId,
                TipoAtividade = TipoAtividade.PAUSA_ATIVA,
                Inicio = DateTime.Now,
                Fim = DateTime.Now.AddMinutes(1)
            };

            var mockRepo = new Mock<IAtividadeRepository>();
            mockRepo.Setup(r => r.FindByUsuarioIdAndAtividadeIdAsync(userId, atividadeId))
                    .ReturnsAsync(activityToDelete);

            context.Atividades.Add(activityToDelete);
            await context.SaveChangesAsync();

            var service = new AtividadeService(context, mockRepo.Object);

            await service.DeleteByUsuarioIdAndAtividadeId(userId, atividadeId);

            var deletedEntity = await context.Atividades.FindAsync((int)atividadeId);
            Assert.Null(deletedEntity);
            mockRepo.Verify(r => r.FindByUsuarioIdAndAtividadeIdAsync(userId, atividadeId), Times.Once());
        }
    }
}