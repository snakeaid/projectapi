namespace ProjectAPI.Primitives
{
    /// <summary>
    /// Model class for the Product entity which is used to receive the product information when deleting it.
    /// </summary>
    public class DeleteProductModel
    {
        /// <summary>
        /// Gets and sets the received unique identifier of the product.
        /// </summary>
        public int Id { get; set; }
    }
}