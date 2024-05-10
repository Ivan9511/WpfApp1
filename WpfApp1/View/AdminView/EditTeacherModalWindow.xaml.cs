using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1.View.AdminView
{
    /// <summary>
    /// Логика взаимодействия для EditTeacherModalWindow.xaml
    /// </summary>
    public partial class EditTeacherModalWindow : Window
    {
        private int UserId;
        public EditTeacherModalWindow(string firstName, string lastName, string subject, int userId)
        {
            InitializeComponent();
            FirstNameTextBox.Text = firstName;
            LastNameTextBox.Text = lastName;
            SubjectNameTextBox.Text = subject;
            UserId = userId;
        }

        private void SaveChangesButton_Clicked(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;
            string subject = SubjectNameTextBox.Text;

            using (SqlConnection connection = DbConnector.OpenConnection())
            {
                SqlCommand updateTeacher = new SqlCommand("UPDATE Teachers SET first_name=@firstName, last_name=@lastName, subject=@subject WHERE user_id=@userId", connection);
                updateTeacher.Parameters.AddWithValue("userId", UserId);
                updateTeacher.Parameters.AddWithValue("firstName", firstName);
                updateTeacher.Parameters.AddWithValue("lastName", lastName);
                updateTeacher.Parameters.AddWithValue("subject", subject);
                updateTeacher.ExecuteNonQuery();

                MessageBox.Show("Учитель успешно изменён.");
                Close();
            }
        }
    }
}
