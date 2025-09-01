using BookShelf.Models.Models;

namespace BookShelf.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        public void Update(Company obj);
        
    }
}
