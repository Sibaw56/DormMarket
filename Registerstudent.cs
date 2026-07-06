using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DormMarket
{
    public partial class Registerstudent : Form
    {
        public Registerstudent()
        {
            InitializeComponent();
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)// create account button
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string contact = txtContactNumber.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contact) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.");
                return;
            }

            MessageBox.Show("Account created successfully. Please log in.");
            //insert Database code here to save the user information to the database
            var login = new LoginStudent();
            login.Show();
            this.Hide();
        }

        private void btnLogin_Click(object sender, EventArgs e)//log in text
        {
            var login = new LoginStudent();
            login.Show();
            this.Hide();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
        }
    }
}
