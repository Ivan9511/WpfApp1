using Microsoft.Data.SqlClient;
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
    /// Логика взаимодействия для CreateStudentModalWindow.xaml
    /// </summary>
    public partial class CreateStudentModalWindow : Window
    {
        public CreateStudentModalWindow()
        {
            InitializeComponent();
        }

        private void RegisterStudentButton_Clicked(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Text;
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;
            string groupName = GroupNameTextBox.Text;
            int groupId = GetGroupId(groupName);

            if (groupId > 0)
            {
                using (SqlConnection connection = DbConnector.OpenConnection())
                {
                    SqlCommand createUser = new SqlCommand("INSERT INTO Users (username, password, role) VALUES (@userName, @password, @role)", connection);
                    createUser.Parameters.AddWithValue("userName", login);
                    createUser.Parameters.AddWithValue("password", password);
                    createUser.Parameters.AddWithValue("role", "Student");
                    createUser.ExecuteNonQuery();

                    // забыл команду для получения последнего id с таблицы :(
                    SqlCommand getLastUserId = new SqlCommand("SELECT user_id FROM Users WHERE username=@userName", connection);
                    getLastUserId.Parameters.AddWithValue("userName", login);
                    using (SqlDataReader reader = getLastUserId.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = reader.GetInt32(0);

                            reader.Close();
                            SqlCommand createStudent = new SqlCommand("INSERT INTO Students (user_id, first_name, last_name, group_id) VALUES (@userId, @firstName, @lastName, @groupId)", connection);
                            createStudent.Parameters.AddWithValue("userId", userId);
                            createStudent.Parameters.AddWithValue("firstName", firstName);
                            createStudent.Parameters.AddWithValue("lastName", lastName);
                            createStudent.Parameters.AddWithValue("groupId", groupId);
                            createStudent.ExecuteNonQuery();

                            MessageBox.Show("Студент успешно добавлен.");
                            Close();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Не найдена группа.");
            }             

        }

        private int GetGroupId(string groupName)
        {
            using(SqlConnection connection = DbConnector.OpenConnection())
            {
                SqlCommand getGroupId = new SqlCommand("SELECT group_id FROM Groups WHERE group_name=@groupName", connection);
                getGroupId.Parameters.AddWithValue("groupName", groupName);
                using(SqlDataReader reader =  getGroupId.ExecuteReader()) 
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
