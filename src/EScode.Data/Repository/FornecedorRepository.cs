using EScode.Business.Interfaces;
using EScode.Business.Models;
using EScode.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace EScode.Data.Repository
{
    public class FornecedorRepository : Repository<Fornecedor>, IFornecedorRepository
    {
        public FornecedorRepository(EScodeDbContext context ) : base(context) { }
        public async Task<Fornecedor> ObterFornecedorEndereco(Guid id)
        {
            return await Db.Fornecedores.AsNoTracking()
                .Include(p => p.Produtos)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Fornecedor> ObterFornecedorProdutosEndereco(Guid id)
        {
            return await Db.Fornecedores.AsNoTracking()
                .Include(p => p.Produtos)
                .Include(e => e.Endereco)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
