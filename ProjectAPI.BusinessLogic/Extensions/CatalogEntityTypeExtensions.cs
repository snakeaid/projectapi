using System;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Extensions
{
    /// <summary>
    /// Provides extensions for <see cref="CatalogEntityType"/>.
    /// </summary>
    public static class CatalogEntityTypeExtensions //TODO move to business logic
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