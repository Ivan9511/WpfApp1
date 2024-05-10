using Microsoft.Data.SqlClient;
using System.Text;
using System.Windows;
using WpfApp1.View.AdminView;
using WpfApp1.View.DirectorView;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Clicked(object sender, RoutedEventArgs e)
        {
            string cmd = "SELECT * FROM Users WHERE username=@login AND password=@password AND role=@role";
            string login = LoginTextBox.Text;
            string password = PwdPasswordBox.Password;
            using(SqlConnection connection = DbConnector.OpenConnection())
            {
                SqlCommand adminCommand = new SqlCommand(cmd, connection);
                adminCommand.Parameters.AddWithValue("login", login);
                adminCommand.Parameters.AddWithValue("password", password);
                adminCommand.Parameters.AddWithValue("role", "Admin");
                using(SqlDataReader adminReader = adminCommand.ExecuteReader())
                {
                    if (adminReader.Read())
                    {
                        // успешный вход Админ
                        AdminWindow adminWindow = new AdminWindow();
                        adminWindow.Show();
                        Close();
                    }
                    else
                    {
                        adminReader.Close();
                        SqlCommand directorCommand = new SqlCommand(cmd, connection);
                        directorCommand.Parameters.AddWithValue("login", login);
                        directorCommand.Parameters.AddWithValue("password", password);
                        directorCommand.Parameters.AddWithValue("role", "Director");
                        using(SqlDataReader directorReader = directorCommand.ExecuteReader())
                        {
                            if (directorReader.Read())
                            {
                                // успешный вход Директор
                                DirectorWindow directorWindow = new DirectorWindow();
                                directorWindow.Show();
                                Close();
                            }
                            else
                            {
                                directorReader.Close();
                                MessageBox.Show("Неверный логин или пароль.");
                            }
                        }
                        
                    }
                }


            }
        }
    }
}