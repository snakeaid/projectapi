using System;
using System.Collections.Generic;
using System.Text;
using ProjectAPI.DataAccess.Primitives;
using ServiceStack;
using ServiceStack.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ProjectAPI.BusinessLogic.Extensions
{
    /// <summary>
    /// Provides extensions for <see cref="UploadRequest"/>.
    /// </summary>
    public static class UploadRequestExtensions
    {
        /// <summary>
        /// Parses the file form the given upload request into a generic list.
        /// </summary>
        /// <param name="request">An instance of <see cref="UploadRequest"/>.</param>
        /// <typeparam name="T">Type of objects in the list.</typeparam>
        /// <returns><see cref="List{T}"/>.</returns>
        public static List<T> ParseIntoList<T>(this UploadRequest request)
        {
            var fileContents = Encoding.UTF8.GetString(Convert.FromBase64String(request.File));
            var list = new List<T>();

            switch (request.FileType)
            {
                case "csv":
                    CsvConfig.ItemSeperatorString = ";"; //optional
                    list = fileContents.FromCsv<List<T>>();
                    break;
                case "json":
                    list = JsonSerializer.Deserialize<List<T>>(fileContents);
                    break;
            }

            return list;
        }
    }
}