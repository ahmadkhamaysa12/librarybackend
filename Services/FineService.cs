
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public class FineService
    {
        private List<Fine> fines;
        private List<Payment> payments;

        private int nextFineId;
        private int nextPaymentId;

        public FineService()
        {
            fines = new List<Fine>();
            payments = new List<Payment>();

            nextFineId = 1;
            nextPaymentId = 1;
        }

        // Register fine after returning an item
        internal void RegisterFine(Loan loan)
        {
            if (!loan.ReturnDate.HasValue)
                return;

            foreach (Fine fine in fines)
            {
                if (fine.Loan.Id == loan.Id)
                    return;
            }

            decimal amount = loan.GetLateFee(
                loan.ReturnDate.Value
            );

            if (amount <= 0)
                return;

            Fine newFine = new Fine(
                nextFineId, loan, amount
            );

            fines.Add(newFine);
            nextFineId++;
        }

        // Search for a fine
        public Fine? SearchFine(int fineId)
        {
            foreach (Fine fine in fines)
            {
                if (fine.Id == fineId)
                    return fine;
            }

            return null;
        }

        // Pay a fine
        public bool PayFine(int fineId, decimal amount,
                            string reference)
        {
            Fine? fine = SearchFine(fineId);

            if (fine == null)
                return false;

            if (amount <= 0 ||
                amount > fine.GetRemainingAmount())
                return false;

            if (string.IsNullOrWhiteSpace(reference))
                return false;

            reference = reference.Trim();

            // Prevent duplicate payment references
            foreach (Payment payment in payments)
            {
                if (payment.Reference.Equals(
                    reference,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            Payment newPayment = new Payment(
                nextPaymentId,
                fineId,
                reference,
                amount,
                DateTime.Now
            );

            if (!fine.AddPayment(newPayment))
                return false;

            payments.Add(newPayment);
            nextPaymentId++;

            return true;
        }

        public List<Fine> GetMemberFines(int memberId)
        {
            List<Fine> result = new List<Fine>();

            foreach (Fine fine in fines)
            {
                if (fine.Loan.Member.Id == memberId)
                    result.Add(fine);
            }

            return result;
        }

        public decimal GetMemberUnpaidTotal(int memberId)
        {
            decimal total = 0;

            foreach (Fine fine in fines)
            {
                if (fine.Loan.Member.Id == memberId)
                {
                    total += fine.GetRemainingAmount();
                }
            }

            return total;
        }

        public decimal GetMemberPaidTotal(int memberId)
        {
            decimal total = 0;

            foreach (Fine fine in fines)
            {
                if (fine.Loan.Member.Id == memberId)
                {
                    total += fine.GetPaidAmount();
                }
            }

            return total;
        }

        public bool HasUnpaidFines(int memberId)
        {
            return GetMemberUnpaidTotal(memberId) > 0;
        }

        public decimal GetTotalCollected()
        {
            decimal total = 0;

            foreach (Payment payment in payments)
            {
                total += payment.Amount;
            }

            return total;
        }

        public List<Fine> GetAllFines()
        {
            return new List<Fine>(fines);
        }

        public List<Payment> GetAllPayments()
        {
            return new List<Payment>(payments);
        }
    }
}
