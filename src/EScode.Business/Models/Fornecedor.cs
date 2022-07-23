namespace EScode.Business.Models
{
    public class Fornecedor : EntityBase
    {
        public string Nome { get; set; }

        public string Documento { get; set; }
        public TipoFornecedor TipoFornecedor { get; set; }
        public Endereco Endereco { get; set; }
        public bool Ativo { get; set; }

        /*Relações EF*/
        public IEnumerable<Produto> Produtos { get; set; }
    }
}
