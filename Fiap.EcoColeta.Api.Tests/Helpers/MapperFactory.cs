using AutoMapper;
using Fiap.EcoColeta.Api.Mapping;

namespace Fiap.EcoColeta.Api.Tests.Helpers
{
    /// <summary>
    /// Cria uma instância real do AutoMapper (com o MappingProfile da
    /// aplicação) para ser usada nos testes, evitando a necessidade de
    /// mockar a interface genérica IMapper.
    /// </summary>
    public static class MapperFactory
    {
        public static IMapper Create()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = true;
                cfg.AddProfile<MappingProfile>();
            });
            return config.CreateMapper();
        }
    }
}
