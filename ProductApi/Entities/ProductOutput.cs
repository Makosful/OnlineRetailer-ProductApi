namespace ProductApi.Entities
{
    public class ProductOutput
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public float Price { get; set; }

        public int ItemsInStock { get; set; }

        public int ItemsReserved { get; set; }
    }
}