using AutoMapper;
using ProjectAPI.BusinessLogic.Extensions;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Primitives;

namespace ProjectAPI.Mapping
{
	/// <summary>
    /// Represents entity to model and model to entity mapping profile.
    /// </summary>
	public class AllMappersProfile : Profile
	{
		/// <summary>
        /// Constructs an instance of <see cref="AllMappersProfile"/> class.
        /// </summary>
		public AllMappersProfile()
		{
			CreateMap<Product, ProductModel>();
			CreateMap<CreateProductModel, Product>();
			CreateMap<UpdateProductModel, Product>();
			
			CreateMap<Category, CategoryModel>();
			CreateMap<CreateCategoryModel, Category>();
			CreateMap<UpdateCategoryModel, Category>();
			
			CreateMap<UploadRequest, UploadRequestModel>()
					.ForMember(dest => dest.Type, 
						opt => opt.MapFrom(src => src.Type.ToFriendlyString()));
		}
	}
}