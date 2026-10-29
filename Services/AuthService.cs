
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public class AuthService
    {
        private Library library;
        private List<LoginAccount> accounts;

        public AuthService(Library library)
        {
            this.library = library;
            accounts = new List<LoginAccount>();
        }

        private bool UsernameExists(string username)
        {
            foreach (LoginAccount account in accounts)
            {
                if (account.User.Username.Equals(
                    username,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public Member? RegisterMember(
            string name,
            string username,
            string password)
        {
            name = name.Trim();
            username = username.Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
                return null;

            if (password.Length < 8)
                return null;

            if (UsernameExists(username))
                return null;

            int nextId = 101;

            foreach (Member member in library.GetAllMembers())
            {
                if (member.Id >= nextId)
                    nextId = member.Id + 1;
            }

            Member newMember = new Member(
                nextId, name, username, 3
            );

            if (!library.AddMember(newMember))
                return null;

            accounts.Add(
                new LoginAccount(newMember, password)
            );

            return newMember;
        }

        public bool RegisterLibrarian(
            Librarian librarian,
            string password)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                password.Length < 8)
                return false;

            if (UsernameExists(librarian.Username))
                return false;

            accounts.Add(
                new LoginAccount(librarian, password)
            );

            return true;
        }

        public Person? Login(string username, string password)
        {
            foreach (LoginAccount account in accounts)
            {
                if (account.User.Username.Equals(
                    username.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    if (account.VerifyPassword(password))
                        return account.User;

                    return null;
                }
            }

            return null;
        }
    }
}
