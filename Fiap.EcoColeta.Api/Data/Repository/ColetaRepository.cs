using Fiap.EcoColeta.Api.Data.Contexts;
using Fiap.EcoColeta.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fiap.EcoColeta.Api.Data.Repository
{
    public class ColetaRepository : IColetaRepository
    {
        private readonly DatabaseContext _context;

        public ColetaRepository(DatabaseContext context)
        {
            _context = context;
        }

        public (IEnumerable<ColetaModel> Itens, int Total) GetAll(
            int pagina, int tamanho, int? tipoResiduoId, int? pontoColetaId,
            DateTime? dataInicio, DateTime? dataFim)
        {
            var query = _context.Coletas
                .Include(c => c.PontoColeta)
                .Include(c => c.TipoResiduo)
                .AsNoTracking()
                .AsQueryable();

            if (tipoResiduoId.HasValue)
                query = query.Where(c => c.TipoResiduoId == tipoResiduoId.Value);

            if (pontoColetaId.HasValue)
                query = query.Where(c => c.PontoColetaId == pontoColetaId.Value);

            if (dataInicio.HasValue)
                query = query.Where(c => c.DataColeta >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(c => c.DataColeta <= dataFim.Value);

            query = query.OrderByDescending(c => c.DataColeta);

            var total = query.Count();
            var itens = query
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToList();

            return (itens, total);
        }

        public void Add(ColetaModel coleta)
        {
            _context.Coletas.Add(coleta);
            _context.SaveChanges();
        }

        private IQueryable<ColetaModel> FiltrarPorPeriodo(DateTime? dataInicio, DateTime? dataFim)
        {
            var query = _context.Coletas
                .Include(c => c.TipoResiduo)
                .Include(c => c.PontoColeta)
                .AsNoTracking()
                .AsQueryable();

            if (dataInicio.HasValue) query = query.Where(c => c.DataColeta >= dataInicio.Value);
            if (dataFim.HasValue) query = query.Where(c => c.DataColeta <= dataFim.Value);

            return query;
        }

        public Dictionary<string, decimal> GetVolumePorTipoResiduo(DateTime? dataInicio, DateTime? dataFim)
        {
            return FiltrarPorPeriodo(dataInicio, dataFim)
                .GroupBy(c => c.TipoResiduo!.Nome)
                .Select(g => new { Nome = g.Key, Total = g.Sum(c => c.QuantidadeKg) })
                .ToDictionary(x => x.Nome, x => x.Total);
        }

        public Dictionary<string, decimal> GetVolumePorPontoColeta(DateTime? dataInicio, DateTime? dataFim)
        {
            return FiltrarPorPeriodo(dataInicio, dataFim)
                .GroupBy(c => c.PontoColeta!.Nome)
                .Select(g => new { Nome = g.Key, Total = g.Sum(c => c.QuantidadeKg) })
                .ToDictionary(x => x.Nome, x => x.Total);
        }

        public decimal GetVolumeTotal(DateTime? dataInicio, DateTime? dataFim)
        {
            var query = FiltrarPorPeriodo(dataInicio, dataFim);
            return query.Any() ? query.Sum(c => c.QuantidadeKg) : 0;
        }

        public int GetTotalRegistros() => _context.Coletas.Count();
    }
}
