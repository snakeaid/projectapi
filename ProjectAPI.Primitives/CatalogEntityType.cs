using System;

namespace ProjectAPI.Primitives
{
    /// <summary>
    /// Enumeration class which defines entity types stored in the catalog.
    /// </summary>
    public enum CatalogEntityType
    {
        Product, 
        Category
    }

    /// <summary>
    /// Extension class for <see cref="CatalogEntityType"/>.
    /// </summary>
    public static class CatalogEntityTypeExtensions
    {
        /// <summary>
        /// Extension method which provides user-friendly type names for <see cref="CatalogEntityType"/>
        /// </summary>
        /// <param name="type">An instance of <see cref="CatalogEntityType"/></param>
        /// <returns>User-friendly type name.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ToFriendlyString(this CatalogEntityType type)
        {
            switch (type)
            {
                case CatalogEntityType.Product:
                    return "Products";
                case CatalogEntityType.Category:
                    return "Categories";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}