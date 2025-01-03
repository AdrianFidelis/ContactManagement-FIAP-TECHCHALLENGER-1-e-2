namespace ContactManagement.Domain.Entities
{
    public class Phone
    {
        public int CountryCode { get; set; }  // Código do País (Ex: 55)
        public int RegionalCode { get; set; } // DDD (Ex: 11, 21, 31)
        public int NumberPhone { get; set; } // Número de telefone (Ex: 987654321)

        public override string ToString()
        {
            return $"+{CountryCode} ({RegionalCode}) {NumberPhone}";
        }
    }
}
