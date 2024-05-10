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
    /// Логика взаимодействия для CreateTeacherModalWIndow.xaml
    /// </summary>
    public partial class CreateTeacherModalWIndow : Window
    {
        public CreateTeacherModalWIndow()
        {
            InitializeComponent();
        }

        private void RegisterTeacherButton_Clicked(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Text;
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;
            string subjectName = SubjectNameTextBox.Text;
            using (SqlConnection connection = DbConnector.OpenConnection())
            {
                SqlCommand createUser = new SqlCommand("INSERT INTO Users (username, password, role) VALUES (@userName, @password, @role)", connection);
                createUser.Parameters.AddWithValue("userName", login);
                createUser.Parameters.AddWithValue("password", password);
                createUser.Parameters.AddWithValue("role", "Teacher");
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
                        SqlCommand createStudent = new SqlCommand("INSERT INTO Teachers (user_id, first_name, last_name, subject) VALUES (@userId, @firstName, @lastName, @subject)", connection);
                        createStudent.Parameters.AddWithValue("userId", userId);
                        createStudent.Parameters.AddWithValue("firstName", firstName);
                        createStudent.Parameters.AddWithValue("lastName", lastName);
                        createStudent.Parameters.AddWithValue("subject", subjectName);
                        createStudent.ExecuteNonQuery();

                        MessageBox.Show("Учитель успешно добавлен.");
                        Close();
                    }
                }
            }
        }

        private int GetSubjectId(string subjectName)
        {
            using (SqlConnection connection = DbConnector.OpenConnection())
            {
                SqlCommand getSubjectId = new SqlCommand("SELECT subject_id FROM Subjects WHERE subject_name=@subjectName", connection);
                getSubjectId.Parameters.AddWithValue("subjectName", subjectName);
                using (SqlDataReader reader = getSubjectId.ExecuteReader())
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
