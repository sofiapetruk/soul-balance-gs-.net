namespace soulBalanceGs.Service
{
    public interface IService<TResponse, TDto>
    {
        Task<TResponse> Save(TDto dto);
        Task<IEnumerable<TResponse>> GetAll(int page, int pageSize);
        Task<TResponse> GetById(int id);
        Task<TResponse> Update(int id, TDto dto);
        Task DeleteById(int id);
    }
}
