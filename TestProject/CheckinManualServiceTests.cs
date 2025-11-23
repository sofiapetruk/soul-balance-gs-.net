using Microsoft.EntityFrameworkCore;
using soulBalanceGs.Data;
using soulBalanceGs.DTOs;
using soulBalanceGs.Exceptions;
using soulBalanceGs.Models;
using soulBalanceGs.Repository;
using soulBalanceGs.Service;
using Moq;

namespace TestProject
{
    public class CheckinManualServiceTests
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
        public async Task Save_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var mockRepo = new Mock<ICheckinManualRepository>();
            var service = new CheckinManualService(context, mockRepo.Object);

            var requestDto = new CheckinManualRequestDto { Email = "nonexistent@email.com", Humor = "Ruim", Energia = "Baixa", Foco = "Zero" };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.Save(requestDto));
        }


        [Fact]
        public async Task GetById_ShouldThrowKeyNotFoundException_WhenNotFound()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var mockRepo = new Mock<ICheckinManualRepository>();
            var service = new CheckinManualService(context, mockRepo.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetById(999));
        }

        [Fact]
        public async Task DeleteById_ShouldThrowKeyNotFoundException_WhenNotFound()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var mockRepo = new Mock<ICheckinManualRepository>();
            var service = new CheckinManualService(context, mockRepo.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteById(999));
        }

        [Fact]
        public async Task GetAllByUsuario_ShouldReturnCheckinsFromRepo_ForSpecificUser()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var userId = 200;

            var mockRepo = new Mock<ICheckinManualRepository>();
            var checkins = new List<CheckinManual>
            {
                new CheckinManual { ChekinId = 10, FkIdUsuario = userId, Humor = "Certo" },
                new CheckinManual { ChekinId = 11, FkIdUsuario = userId, Humor = "Errado" }
            };

            mockRepo.Setup(r => r.FindByUsuarioIdAsync(userId))
                    .ReturnsAsync(checkins);

            var service = new CheckinManualService(context, mockRepo.Object);

            var result = await service.GetAllByUsuario(userId);

            Assert.Equal(2, result.Count);
            Assert.True(result.All(c => c.Usuario == userId));
            mockRepo.Verify(r => r.FindByUsuarioIdAsync(userId), Times.Once());
        }
    }
}