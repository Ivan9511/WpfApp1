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
    /// Логика взаимодействия для CreateGroupModalWindow.xaml
    /// </summary>
    public partial class CreateGroupModalWindow : Window
    {
        public CreateGroupModalWindow()
        {
            InitializeComponent();
        }

        private void CreateGroupButton_Clicked(object sender, RoutedEventArgs e)
        {
            string groupName = GroupNameTextBox.Text;
            string curatorFirstName = CuratorFirstNameTextBox.Text;
            string curatorLastName = CuratorLastNameTextBox.Text;

            using(SqlConnection connection = DbConnector.OpenConnection())
            {
                SqlCommand cmd = new SqlCommand("SELECT teacher_id FROM Teachers WHERE first_name=@firstName AND last_name=@lastName", connection);
                cmd.Parameters.AddWithValue("firstName", curatorFirstName);
                cmd.Parameters.AddWithValue("lastName", curatorLastName);
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int curatorId = reader.GetInt32(0);

                        reader.Close();
                        SqlCommand createGroup = new SqlCommand("INSERT INTO Groups (group_name, group_curator_id) VALUES (@groupName, @groupCuratorId)", connection);
                        createGroup.Parameters.AddWithValue("groupName", groupName);
                        createGroup.Parameters.AddWithValue("groupCuratorId", curatorId);
                        createGroup.ExecuteNonQuery();
                        MessageBox.Show("Группа успешно создана.");
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Преподаватель не найден. Создание группы не удалось.");
                    }
                }
            }
        }
    }
}
