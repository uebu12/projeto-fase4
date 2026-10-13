using AutoMapper;
using Fiap.EcoColeta.Api.Models;
using Fiap.EcoColeta.Api.ViewModels;

namespace Fiap.EcoColeta.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // TipoResiduo
            CreateMap<TipoResiduoModel, TipoResiduoViewModel>();
            CreateMap<TipoResiduoCreateViewModel, TipoResiduoModel>();

            // PontoColeta
            CreateMap<PontoColetaModel, PontoColetaViewModel>()
                .ForMember(dest => dest.PercentualOcupacao, opt => opt.Ignore());
            CreateMap<PontoColetaCreateViewModel, PontoColetaModel>();
            CreateMap<PontoColetaUpdateViewModel, PontoColetaModel>();

            // Coleta
            CreateMap<ColetaModel, ColetaViewModel>()
                .ForMember(dest => dest.PontoColetaNome, opt => opt.MapFrom(src => src.PontoColeta != null ? src.PontoColeta.Nome : string.Empty))
                .ForMember(dest => dest.TipoResiduoNome, opt => opt.MapFrom(src => src.TipoResiduo != null ? src.TipoResiduo.Nome : string.Empty));
            CreateMap<ColetaCreateViewModel, ColetaModel>();

            // Alerta
            CreateMap<AlertaModel, AlertaViewModel>()
                .ForMember(dest => dest.PontoColetaNome, opt => opt.MapFrom(src => src.PontoColeta != null ? src.PontoColeta.Nome : string.Empty));
        }
    }
}
