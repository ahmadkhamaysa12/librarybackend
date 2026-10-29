
namespace LibraryManagementSystem.Models
{
    public class Fine
    {
        public int Id { get; private set; }
        public Loan Loan { get; private set; }
        public decimal Amount { get; private set; }

        private List<Payment> payments;

        internal Fine(int id, Loan loan, decimal amount)
        {
            Id = id;
            Loan = loan;
            Amount = amount;

            payments = new List<Payment>();
        }

        public decimal GetPaidAmount()
        {
            decimal total = 0;

            foreach (Payment payment in payments)
            {
                total += payment.Amount;
            }

            return total;
        }

        public decimal GetRemainingAmount()
        {
            return Amount - GetPaidAmount();
        }

        public bool IsFullyPaid()
        {
            return GetRemainingAmount() == 0;
        }

        public List<Payment> GetPayments()
        {
            return new List<Payment>(payments);
        }

        internal bool AddPayment(Payment payment)
        {
            if (payment.FineId != Id)
                return false;

            if (payment.Amount <= 0)
                return false;

            if (payment.Amount > GetRemainingAmount())
                return false;

            foreach (Payment p in payments)
            {
                if (p.Id == payment.Id ||
                    p.Reference.Equals(
                        payment.Reference,
                        StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            payments.Add(payment);
            return true;
        }
    }
}
