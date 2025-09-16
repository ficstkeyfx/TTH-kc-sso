using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace api
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
        }
    }
    //public class GenericMappingProfile : Profile
    //{
    //    public GenericMappingProfile()
    //    {
    //        CreateMap(typeof(TItem<>), typeof(TItem<>));
    //        CreateMap(typeof(TItem<>), typeof(TItem<>))
    //        .ForMember("Value", opt => opt.ConvertUsing(typeof(GenericValueConverter<,>), "Value"));
    //    }
    //}
   
}