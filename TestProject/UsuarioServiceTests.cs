using Microsoft.EntityFrameworkCore;
using soulBalanceGs.Data;
using soulBalanceGs.DTOs;
using soulBalanceGs.Models;
using soulBalanceGs.Service;

namespace TestProject
{
    public class UsuarioServiceTests
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        public UsuarioServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestUsuarioDB" + Guid.NewGuid())
                .Options;
        }

        private UsuarioRequestDto GetBaseRequestDto() =>
            new UsuarioRequestDto { Nome = "Alice Santos", Email = "alice.santos@dominio.com", Senha = "SenhaSegura123" };

        private Usuario CreateBaseUsuario(AppDbContext context)
        {
            var entity = new Usuario { Nome = "João Base", Email = "joao@dominio.com", Senha = "senha456" };
            context.Usuarios.Add(entity);
            context.SaveChanges();
            return entity;
        }

        [Fact]
        public async Task GetById_ShouldReturnUser_WhenUserExists()
        {
            using var context = new AppDbContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var baseUser = CreateBaseUsuario(context);

            var service = new UsuarioService(context);

            var result = await service.GetById(baseUser.IdUsuario);

            Assert.NotNull(result);
            Assert.Equal(baseUser.Email, result.Email);
        }

        [Fact]
        public async Task GetById_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            using var context = new AppDbContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var service = new UsuarioService(context);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetById(999));
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllUsers()
        {
            const int totalUsers = 3;

            using var context = new AppDbContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            for (int i = 1; i <= totalUsers; i++)
            {
                context.Usuarios.Add(new Usuario { Nome = $"User {i}", Email = $"user{i}@test.com", Senha = "pass" });
            }
            await context.SaveChangesAsync();

            var service = new UsuarioService(context);

            var result = await service.GetAll(page: 1, pageSize: 50);

            Assert.NotNull(result);
            Assert.Equal(totalUsers, result.Count());
            Assert.Contains(result, u => u.Nome == "User 3");
        }

        [Fact]
        public async Task Update_ShouldModifyUserAndReturnUpdatedDto()
        {
            using var context = new AppDbContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var baseUser = CreateBaseUsuario(context);
            var service = new UsuarioService(context);

            var updateDto = new UsuarioRequestDto
            {
                Nome = "Alice Atualizada",
                Email = baseUser.Email,
                Senha = "newpassword"
            };

            var result = await service.Update(baseUser.IdUsuario, updateDto);

            Assert.NotNull(result);
            Assert.Equal("Alice Atualizada", result.Nome);

            var updatedEntity = await context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == baseUser.IdUsuario);
            Assert.Equal("Alice Atualizada", updatedEntity.Nome);
            Assert.Equal("newpassword", updatedEntity.Senha);
        }

        [Fact]
        public async Task DeleteById_ShouldRemoveUser()
        {
            using var context = new AppDbContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var baseUser = CreateBaseUsuario(context);
            var service = new UsuarioService(context);
            var userId = baseUser.IdUsuario;

            await service.DeleteById(userId);

            var deletedEntity = await context.Usuarios.FindAsync(userId);
            Assert.Null(deletedEntity);
        }
    }
}
