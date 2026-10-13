using Fiap.EcoColeta.Api.Data.Contexts;
using Fiap.EcoColeta.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fiap.EcoColeta.Api.Data.Repository
{
    public class PontoColetaRepository : IPontoColetaRepository
    {
        private readonly DatabaseContext _context;

        public PontoColetaRepository(DatabaseContext context)
        {
            _context = context;
        }

        public (IEnumerable<PontoColetaModel> Itens, int Total) GetAll(int pagina, int tamanho)
        {
            var query = _context.PontosColeta.AsNoTracking().OrderBy(p => p.Nome);

            var total = query.Count();
            var itens = query
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToList();

            return (itens, total);
        }

        public IEnumerable<PontoColetaModel> GetCriticos(decimal percentualLimite)
        {
            // Pontos ativos cujo volume atual representa percentual >= limite da capacidade máxima.
            return _context.PontosColeta
                .AsNoTracking()
                .Where(p => p.Ativo && p.CapacidadeMaximaKg > 0)
                .ToList()
                .Where(p => (p.VolumeAtualKg / p.CapacidadeMaximaKg) * 100 >= percentualLimite)
                .OrderByDescending(p => p.VolumeAtualKg / p.CapacidadeMaximaKg)
                .ToList();
        }

        public PontoColetaModel? GetById(int id) =>
            _context.PontosColeta.FirstOrDefault(p => p.PontoColetaId == id);

        public void Add(PontoColetaModel pontoColeta)
        {
            _context.PontosColeta.Add(pontoColeta);
            _context.SaveChanges();
        }

        public void Update(PontoColetaModel pontoColeta)
        {
            _context.PontosColeta.Update(pontoColeta);
            _context.SaveChanges();
        }
    }
}
