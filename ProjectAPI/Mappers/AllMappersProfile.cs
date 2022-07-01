using System;
using AutoMapper;
using ProjectAPI.Models;

namespace ProjectAPI.Mappers
{
	public class AllMappersProfile : Profile
	{
		public AllMappersProfile()
		{
			CreateMap<Product, ProductDTO>();
			CreateMap<Category, CategoryDTO>();

			CreateMap<ProductDTO, Product>().ForMember(p => p.DateCreated, opts => opts.Ignore())
											.ForMember(p => p.DateUpdated, opts => opts.Ignore());
			//								.ForMember(p => p.Category, opts => opts.Ignore());

			CreateMap<CategoryDTO, Category>().ForMember(c => c.DateCreated, opts => opts.Ignore())
											  .ForMember(c => c.DateUpdated, opts => opts.Ignore());
			//								  .ForMember(c => c.Products, opts => opts.Ignore());
		}
	}
}

