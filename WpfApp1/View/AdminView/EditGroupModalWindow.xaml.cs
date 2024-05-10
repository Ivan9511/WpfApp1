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
    /// Логика взаимодействия для EditGroupModalWindow.xaml
    /// </summary>
    public partial class EditGroupModalWindow : Window
    {
        private int GroupId;
        public EditGroupModalWindow(int groupId, string groupName, string curatorFirstName, string curatorLastName)
        {
            InitializeComponent();
            GroupNameTextBox.Text = groupName;
            CuratorFirstNameTextBox.Text = curatorFirstName;
            CuratorLastNameTextBox.Text = curatorLastName;
            GroupId = groupId;
        }

        private void EditGroupButton_Clicked(object sender, RoutedEventArgs e)
        {
            string groupName = GroupNameTextBox.Text;
            string curatorFirstName = CuratorFirstNameTextBox.Text;
            string curatorLastName = CuratorLastNameTextBox.Text;

            int curatorId = GetCuratorId(curatorFirstName, curatorLastName);
            if (curatorId > 0)
            {
                using (SqlConnection connection = DbConnector.OpenConnection())
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Groups SET group_name=@groupName, group_curator_id=@curatorId WHERE group_id=@groupId", connection);
                    cmd.Parameters.AddWithValue("groupName", groupName);
                    cmd.Parameters.AddWithValue("curatorId", curatorId);
                    cmd.Parameters.AddWithValue("groupId", GroupId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Данные успешно изменены.");
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Преподаватель не найден. Создание группы не удалось.");
            }

        }

        private int GetCuratorId(string firstName, string lastName)
        {
            using(SqlConnection connection = DbConnector.OpenConnection())            
            {
                SqlCommand cmd = new SqlCommand("SELECT teacher_id FROM Teachers WHERE first_name=@firstName AND last_name=@lastName", connection);
                cmd.Parameters.AddWithValue("firstName", firstName);
                cmd.Parameters.AddWithValue("lastName", lastName);
                using(SqlDataReader reader  = cmd.ExecuteReader())
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
