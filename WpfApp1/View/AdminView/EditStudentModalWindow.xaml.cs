using Microsoft.Data.SqlClient;
using System.Windows;

namespace WpfApp1.View.AdminView
{
    /// <summary>
    /// Логика взаимодействия для EditStudentModalWindow.xaml
    /// </summary>
    public partial class EditStudentModalWindow : Window
    {
        private int UserId;
        public EditStudentModalWindow(string firstName, string lastName, string groupName, int userId)
        {
            InitializeComponent();
            FirstNameTextBox.Text = firstName;
            LastNameTextBox.Text = lastName;
            GroupNameTextBox.Text = groupName;
            UserId = userId;
        }

        private void SaveChangesButton_Clicked(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;
            string groupName = GroupNameTextBox.Text;
            int groupId = GetGroupId(groupName);

            if (groupId > 0)
            {
                using (SqlConnection connection = DbConnector.OpenConnection())
                {
                    SqlCommand updateStudent = new SqlCommand("UPDATE Students SET first_name=@firstName, last_name=@lastName WHERE user_id=@userId", connection);
                    updateStudent.Parameters.AddWithValue("userId", UserId);
                    updateStudent.Parameters.AddWithValue("firstName", firstName);
                    updateStudent.Parameters.AddWithValue("lastName", lastName);
                    updateStudent.ExecuteNonQuery();

                    MessageBox.Show("Студент успешно изменён.");
                    Close();                       
                }
            }
            else
            {
                MessageBox.Show("Не найдена группа.");
            }
        }

        private int GetGroupId(string groupName)
        {
            using (SqlConnection connection = DbConnector.OpenConnection())
            {
                SqlCommand getGroupId = new SqlCommand("SELECT group_id FROM Groups WHERE group_name=@groupName", connection);
                getGroupId.Parameters.AddWithValue("groupName", groupName);
                using (SqlDataReader reader = getGroupId.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }
    }
}
