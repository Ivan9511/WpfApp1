using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows;
using WpfApp1.Model;

namespace WpfApp1.View.DirectorView
{
    /// <summary>
    /// Логика взаимодействия для DirectorWindow.xaml
    /// </summary>
    public partial class DirectorWindow : Window
    {
        ObservableCollection<Student> studentsCollection = new ObservableCollection<Student>();
        ObservableCollection<Groups> groupsCollection = new ObservableCollection<Groups>();

        ObservableCollection<GradeMonitoring> groupGradeMonitoringCollection = new ObservableCollection<GradeMonitoring>();
        ObservableCollection<GradeMonitoring> gradeMonitoringCollection = new ObservableCollection<GradeMonitoring>();

        public DirectorWindow()
        {
            InitializeComponent();

            GetData();
            StudentGradesMonitoring();

            StudentGradesDataGrid.ItemsSource = groupGradeMonitoringCollection;
            GroupsDataGrid.ItemsSource = groupsCollection;
            GradesMonitoringDataGrid.ItemsSource = gradeMonitoringCollection;
        }

        private void GetStatsButton_Clicked(object sender, RoutedEventArgs e)
        {
            groupGradeMonitoringCollection.Clear();
            using (SqlConnection connection = DbConnector.OpenConnection())
            {
                SqlCommand command = new SqlCommand("SELECT group_curator_id FROM Groups WHERE group_name=@groupName", connection);
                command.Parameters.AddWithValue("groupName", GroupNameTextBox.Text);
                using(SqlDataReader groupsReader  = command.ExecuteReader())
                {
                    if (groupsReader.Read())
                    {
                        GroupCuratorName.Content = "Куратор - " + GetCuratorName(groupsReader.GetInt32(0), "FullName");
                        groupsReader.Close();
                        int studentCount = 0;
                        foreach (Student student in studentsCollection)
                        {
                            if (student.group_name == GroupNameTextBox.Text)
                            {
                                studentCount++;
                                SqlCommand getGrades = new SqlCommand("SELECT grade FROM Grades WHERE student_id=@studentId", connection);
                                getGrades.Parameters.AddWithValue("studentId", student.student_id);
                                using (SqlDataReader reader = getGrades.ExecuteReader())
                                {
                                    int studentRating = 0;
                                    while (reader.Read())
                                    {
                                        studentRating = studentRating + reader.GetInt32(0);
                                    }

                                    GradeMonitoring gradeMonitoringModel = new GradeMonitoring();
                                    gradeMonitoringModel.student_name = student.first_name + " " + student.last_name;
                                    gradeMonitoringModel.student_rating = studentRating;
                                    groupGradeMonitoringCollection.Add(gradeMonitoringModel);
                                }
                            }
                        }
                        StudentCountLabel.Content = "Количество студентов - " + studentCount;
                    }
                    else
                    {
                        MessageBox.Show("Группа с указанным названием не найдена.");
                    }
                }


            }
        }

        private void StudentGradesMonitoring()
        {
            gradeMonitoringCollection.Clear();
            using (SqlConnection connection = DbConnector.OpenConnection())
            {
                foreach (Student student in studentsCollection)
                {
                    SqlCommand getGrades = new SqlCommand("SELECT grade FROM Grades WHERE student_id=@studentId", connection);
                    getGrades.Parameters.AddWithValue("studentId", student.student_id);
                    using (SqlDataReader reader = getGrades.ExecuteReader())
                    {
                        int studentRating = 0;
                        while (reader.Read())
                        {
                            studentRating = studentRating + reader.GetInt32(0);
                        }

                        GradeMonitoring gradeMonitoringModel = new GradeMonitoring();
                        gradeMonitoringModel.student_name = student.first_name + " " + student.last_name;
                        gradeMonitoringModel.student_rating = studentRating;
                        gradeMonitoringCollection.Add(gradeMonitoringModel);
                    }
                }
            }
        }

        private string GetCuratorName(int curator_id, string type)
        {
            using (SqlConnection connection = DbConnector.OpenConnection())
            {
                SqlCommand command = new SqlCommand("SELECT first_name, last_name FROM Teachers WHERE teacher_id=@id", connection);
                command.Parameters.AddWithValue("id", curator_id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        switch (type)
                        {
                            case "FirstName":
                                return reader.GetString(0);
                            case "LastName":
                                return reader.GetString(1);
                            case "FullName":
                                return reader.GetString(0) + " " + reader.GetString(1);
                            default:
                                return "Отсутствует";
                        }
                    }
                    else
                    {
                        return "Отсутствует";
                    }
                }
            }
        }
        private void GetData()
        {
            using (SqlConnection connection = DbConnector.OpenConnection())
            {

                groupsCollection.Clear();
                SqlCommand command = new SqlCommand("SELECT * FROM Groups", connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Groups group = new Groups();
                        group.group_id = reader.GetInt32(0);
                        group.group_name = reader.GetString(1);
                        group.group_curator_name = GetCuratorName(reader.GetInt32(2), "FullName");
                        group.group_curator_first_name = GetCuratorName(reader.GetInt32(2), "FirstName");
                        group.group_curator_last_name = GetCuratorName(reader.GetInt32(2), "LastName");
                        groupsCollection.Add(group);
                    }
                }


                studentsCollection.Clear();
                SqlCommand getStudents = new SqlCommand("SELECT * FROM Students", connection);
                using (SqlDataReader reader = getStudents.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Student student = new Student();
                        student.student_id = reader.GetInt32(0);
                        student.user_id = reader.GetInt32(1);
                        student.first_name = reader.GetString(2);
                        student.last_name = reader.GetString(3);
                        student.group_id = reader.GetInt32(4);
                        student.group_name = GetGroupName(reader.GetInt32(4));
                        studentsCollection.Add(student);
                    }
                }
            }
        }

        private string GetGroupName(int id)
        {
            using (SqlConnection connection = DbConnector.OpenConnection())
            {
                SqlCommand command = new SqlCommand("SELECT group_name FROM Groups WHERE group_id=@id", connection);
                command.Parameters.AddWithValue("id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString(0);
                    }
                    else
                    {
                        return "Отсутствует";
                    }
                }
            }
        }
    }
}
