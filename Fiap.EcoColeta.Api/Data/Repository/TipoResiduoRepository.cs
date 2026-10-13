using Fiap.EcoColeta.Api.Data.Contexts;
using Fiap.EcoColeta.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fiap.EcoColeta.Api.Data.Repository
{
    public class TipoResiduoRepository : ITipoResiduoRepository
    {
        private readonly DatabaseContext _context;

        public TipoResiduoRepository(DatabaseContext context)
        {
            _context = context;
        }

        public IEnumerable<TipoResiduoModel> GetAll() =>
            _context.TiposResiduo.AsNoTracking().OrderBy(t => t.Nome).ToList();

        public TipoResiduoModel? GetById(int id) =>
            _context.TiposResiduo.FirstOrDefault(t => t.TipoResiduoId == id);

        public TipoResiduoModel? GetByNome(string nome) =>
            _context.TiposResiduo.AsNoTracking()
                .FirstOrDefault(t => t.Nome.ToLower() == nome.ToLower());

        public void Add(TipoResiduoModel tipoResiduo)
        {
            _context.TiposResiduo.Add(tipoResiduo);
            _context.SaveChanges();
        }
    }
}
