using AutoMapper;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Primitives;

namespace ProjectAPI.Mapping
{
	public class AllMappersProfile : Profile
	{
		public AllMappersProfile()
		{
			CreateMap<Product, ProductModel>();
			CreateMap<Category, CategoryModel>();

			CreateMap<ProductModel, Product>().ForMember(p => p.DateCreated, opts => opts.Ignore())
											.ForMember(p => p.DateUpdated, opts => opts.Ignore())
											.ForMember(p => p.Id, opts => opts.Ignore());
			//								.ForMember(p => p.Category, opts => opts.Ignore());

			CreateMap<CategoryModel, Category>().ForMember(c => c.DateCreated, opts => opts.Ignore())
											  .ForMember(c => c.DateUpdated, opts => opts.Ignore())
											  .ForMember(c => c.Id, opts => opts.Ignore());
			//								  .ForMember(c => c.Products, opts => opts.Ignore());
		}
	}
}