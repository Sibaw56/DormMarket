using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DormMarket
{
    public partial class BrowseDetailStudent : Form
    {
        public BrowseDetailStudent()
        {
            InitializeComponent();
        }
        private void loadDormDetails()//Load dorm details from database
        {
            //load dorm details from database a the button
        }
        private void btnMessageOwner_Click(object sender, EventArgs e)//message button
        {
            var message = new Registerstudent();
            message.Show();
            this.Hide();
        }
        private void ManangeProfileStudent_Load(object sender, EventArgs e)//Load profile infos
        {
            //Load the profile infos
        }

        private void btnChangePhoto_Click(object sender, EventArgs e) //change photo button
        {
            
        }

        private void btnSaveChanges_Click(object sender, EventArgs e) //save changes button
        {
            if(string.IsNullOrEmpty(txtName.Text))
            {
                txtName.Text = //database value Name;
                return;
            }
            if (string.IsNullOrEmpty(txtPhone.Text))
            {
                txtPhone.Text = //database value Phone;
                return;
            }
            if (string.IsNullOrEmpty(txtEmail.Text))
            {
                txtEmail.Text = //database value Phone;
                return;
            }

            if (string.IsNullOrEmpty(txtAddress.Text))
            {
                txtAddress.Text = //database value Phone;
                return;
            }
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                txtPassword.Text = //database value Phone;
                return;
            }
            //edit a row of student profile database with the new values from the textboxes
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            var role = new Role();
            role.Show();
            this.Hide();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var browse = new BrowseDormStudent();
            browse.Show();
            this.Hide();
        }
    }
}
