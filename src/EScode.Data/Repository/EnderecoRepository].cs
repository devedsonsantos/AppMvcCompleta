using EScode.Business.Interfaces;
using EScode.Business.Models;
using EScode.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace EScode.Data.Repository
{
    public class EnderecoRepository : Repository<Endereco>, IEnderecoRepository
    {
        public EnderecoRepository(EScodeDbContext context) : base(context) { }

        public async Task<Endereco> ObterEnderecoPorFornecedor(Guid fornecedorId)
        {
            return await Db.Enderecos.AsNoTracking()
                .Include(f => f.Fornecedor)
                .FirstOrDefaultAsync(e => e.Fornecedor.Id == fornecedorId);
        }
    }
}
