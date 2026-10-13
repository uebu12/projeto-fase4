using Fiap.EcoColeta.Api.Data.Contexts;
using Fiap.EcoColeta.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fiap.EcoColeta.Api.Data.Repository
{
    public class AlertaRepository : IAlertaRepository
    {
        private readonly DatabaseContext _context;

        public AlertaRepository(DatabaseContext context)
        {
            _context = context;
        }

        public (IEnumerable<AlertaModel> Itens, int Total) GetAll(int pagina, int tamanho, bool? somenteAtivos)
        {
            var query = _context.Alertas
                .Include(a => a.PontoColeta)
                .AsNoTracking()
                .AsQueryable();

            if (somenteAtivos == true)
                query = query.Where(a => !a.Resolvido);

            query = query.OrderByDescending(a => a.DataAlerta);

            var total = query.Count();
            var itens = query.Skip((pagina - 1) * tamanho).Take(tamanho).ToList();

            return (itens, total);
        }

        public AlertaModel? GetById(int id) =>
            _context.Alertas.Include(a => a.PontoColeta).FirstOrDefault(a => a.AlertaId == id);

        public void Add(AlertaModel alerta)
        {
            _context.Alertas.Add(alerta);
            _context.SaveChanges();
        }

        public void Update(AlertaModel alerta)
        {
            _context.Alertas.Update(alerta);
            _context.SaveChanges();
        }

        public bool ExisteAlertaAtivo(int pontoColetaId) =>
            _context.Alertas.Any(a => a.PontoColetaId == pontoColetaId && !a.Resolvido);

        public int GetTotalAtivos() => _context.Alertas.Count(a => !a.Resolvido);
    }
}
