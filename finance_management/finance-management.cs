using System;
using System.Collections.Generic;

namespace finance_management
{
    // a. Define the Transaction record
    public record Transaction(int ID, DateTime Date, decimal Amount, string Category);

    // b. Define the interface
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // c. Implement the processors
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"\n[Bank Transfer] Processed {transaction.Amount:F2} for {transaction.Category}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"\n[Mobile Money] Sent {transaction.Amount:F2} for {transaction.Category}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"\n[Crypto Wallet] Received {transaction.Amount:F2} for {transaction.Category}");
        }
    }

    // d. Base Account class
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. New balance: {Balance:F2}");
        }
    }

    // e. Sealed SavingsAccount class
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
            }
            else
            {
                Balance -= transaction.Amount;
                Console.WriteLine($"Transaction successful. Updated balance: {Balance:F2}");
            }
        }
    }

    // f. FinanceApp class
    public class FinanceApp
    {
        private List<Transaction> _transactions = new List<Transaction>();

        public void Run()
        {
            // i. Create SavingsAccount
            var savingsAccount = new SavingsAccount("ACC1001", 1000m);

            // ii. Create 3 Transactions
            var t1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 200m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 100m, "Entertainment");

            // iii. Process each transaction
            ITransactionProcessor mobileMoney = new MobileMoneyProcessor();
            ITransactionProcessor bankTransfer = new BankTransferProcessor();
            ITransactionProcessor cryptoWallet = new CryptoWalletProcessor();

            mobileMoney.Process(t1);
            bankTransfer.Process(t2);
            cryptoWallet.Process(t3);

            // iv. Apply each transaction to account
            savingsAccount.ApplyTransaction(t1);
            savingsAccount.ApplyTransaction(t2);
            savingsAccount.ApplyTransaction(t3);

            // v. Add transactions to the list
            _transactions.Add(t1);
            _transactions.Add(t2);
            _transactions.Add(t3);
        }

        // Main method
        public static void Main(string[] args)
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}
