
namespace Projects.Models
{
    public class CustomersModel
    {
        private int id;
        private string firstname;
        private string lastname;
        private string address;
        private int phonenumber;
        private string email;

        public int Id { get { return id; } set { id = value; } }
        public string Firstname { get { return firstname; } set { firstname = value; } }
        public string Lastname { get { return lastname; } set { lastname = value; } }
        public string Address { get { return address; } set { address = value; } }
        public int Phonenumber { get { return phonenumber; } set { phonenumber = value; } }
        public string Email { get { return email; } set { email = value; } }

    }
}
