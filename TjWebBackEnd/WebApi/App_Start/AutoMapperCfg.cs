using AutoMapper;
using ErpDb.Entitys.Auth;
using WebApi.ViewModel;

namespace WebApi
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            #region DncMenu

          
            CreateMap<MenuCreateViewModel, Menu>();
            CreateMap<MenuEditViewModel, Menu>();

            #endregion DncMenu
        }
    }
}