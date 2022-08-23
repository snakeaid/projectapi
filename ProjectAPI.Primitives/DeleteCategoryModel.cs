namespace ProjectAPI.Primitives
{
    /// <summary>
    /// Model class for the Product entity which is used to receive the category information when deleting it.
    /// </summary>
    public class DeleteCategoryModel
    {
        /// <summary>
        /// Gets and sets the received unique identifier of the category.
        /// </summary>
        public int Id { get; set; }
    }
}