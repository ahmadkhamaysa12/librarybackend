
namespace LibraryManagementSystem.Models
{
    public class Payment
    {
        public int Id { get; private set; }
        public int FineId { get; private set; }
        public string Reference { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaidAt { get; private set; }

        internal Payment(int id, int fineId,
                         string reference, decimal amount,
                         DateTime paidAt)
        {
            Id = id;
            FineId = fineId;
            Reference = reference;
            Amount = amount;
            PaidAt = paidAt;
        }
    }
}
