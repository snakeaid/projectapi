using System;
using System.Collections.Generic;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic
{
    public class BatchCategoriesUpload
    {
        public Guid Id { get; set; }
        public List<Category> Categories { get; set; }
    }
}