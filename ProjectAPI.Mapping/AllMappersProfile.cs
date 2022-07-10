using AutoMapper;
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
			CreateMap<Category, CategoryModel>();

			CreateMap<CreateProductModel, Product>();
			CreateMap<CreateCategoryModel, Category>();

			CreateMap<UpdateProductModel, Product>();
			CreateMap<UpdateCategoryModel, Category>();
		}
	}
}