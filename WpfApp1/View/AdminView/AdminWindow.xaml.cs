using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using WpfApp1.Model;

namespace WpfApp1.View.AdminView
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        ObservableCollection<Groups> groupsCollection = new ObservableCollection<Groups>();
        ObservableCollection<Student> studentsCollection = new ObservableCollection<Student>();
        ObservableCollection<Teacher> teacherCollection = new ObservableCollection<Teacher>();

        ObservableCollection<GradeMonitoring> gradeMonitoringCollection = new ObservableCollection<GradeMonitoring>();

        public AdminWindow()
        {
            InitializeComponent();
            GetData();
            StudentGradesMonitoring();

            GroupsDataGrid.ItemsSource = groupsCollection;
            StudentsDataGrid.ItemsSource = studentsCollection;
            TeachersDataGrid.ItemsSource = teacherCollection;

            GradesMonitoringDataGrid.ItemsSource = gradeMonitoringCollection;
        }

        private void GetData()
        {
            
            using (SqlConnection connection = DbConnector.OpenConnection())
            {
                // Группы
                groupsCollection.Clear();
                SqlCommand command = new SqlCommand("SELECT * FROM Groups", connection);
                using(SqlDataReader reader = command.ExecuteReader())
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


                // Студенты
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

                // Преподаватели
                teacherCollection.Clear();
                SqlCommand getTeachers = new SqlCommand("SELECT * FROM Teachers", connection);
                using(SqlDataReader reader = getTeachers.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Teacher teacher = new Teacher();
                        teacher.teacher_id = reader.GetInt32(0);
                        teacher.user_id = reader.GetInt32(1);
                        teacher.first_name = reader.GetString(2);
                        teacher.last_name = reader.GetString(3);
                        teacher.subject = reader.GetString(4);
                        teacherCollection.Add(teacher);
                    }
                }
            }            
        }

        private string GetGroupName(int id)
        {
            using(SqlConnection connection = DbConnector.OpenConnection())
            {
                SqlCommand command = new SqlCommand("SELECT group_name FROM Groups WHERE group_id=@id", connection);
                command.Parameters.AddWithValue("id", id);
                using (SqlDataReader reader =  command.ExecuteReader())
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

        private void StudentGradesMonitoring()
        {
            gradeMonitoringCollection.Clear();
            using (SqlConnection connection = DbConnector.OpenConnection())
            {
                foreach(Student student in studentsCollection)
                {
                    SqlCommand getGrades = new SqlCommand("SELECT grade FROM Grades WHERE student_id=@studentId", connection);
                    getGrades.Parameters.AddWithValue("studentId", student.student_id);
                    using(SqlDataReader reader = getGrades.ExecuteReader())
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
            using(SqlConnection connection = DbConnector.OpenConnection())
            {
                SqlCommand command = new SqlCommand("SELECT first_name, last_name FROM Teachers WHERE teacher_id=@id", connection);
                command.Parameters.AddWithValue("id", curator_id);
                using(SqlDataReader reader = command.ExecuteReader())
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

        private void CreateGroupButton_Clicked(object sender, RoutedEventArgs e)
        {
            CreateGroupModalWindow createGroupModalWindow = new CreateGroupModalWindow();
            createGroupModalWindow.ShowDialog();
        }
        private void CreateStudentButton_Clicked(object sender, RoutedEventArgs e)
        {
            CreateStudentModalWindow createStudentModalWindow = new CreateStudentModalWindow();
            createStudentModalWindow.ShowDialog();
        }
        private void CreateTeacherButton_Clicked(object sender, RoutedEventArgs e)
        {
            CreateTeacherModalWIndow createTeacherModalWIndow = new CreateTeacherModalWIndow();
            createTeacherModalWIndow.ShowDialog();
        }


        private void EditButton_Clicked(object sender, RoutedEventArgs e)
        {
            Groups selectedGroup = (Groups)GroupsDataGrid.SelectedItem;
            if (selectedGroup != null)
            {
                EditGroupModalWindow editGroupModalWindow = new EditGroupModalWindow(selectedGroup.group_id, selectedGroup.group_name, selectedGroup.group_curator_first_name, selectedGroup.group_curator_last_name);
                editGroupModalWindow.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("Выберите группу, данные которой необходимо отредактировать.");
            }
        }
        private void EditStudentButton_Clicked(object sender, RoutedEventArgs e)
        {
            Student selectedStudent = (Student)StudentsDataGrid.SelectedItem;
            if (selectedStudent != null)
            {
                EditStudentModalWindow editStudentModalWindow = new EditStudentModalWindow(selectedStudent.first_name, selectedStudent.last_name, selectedStudent.group_name, selectedStudent.user_id);
                editStudentModalWindow.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("Выберите студента, данные которого необходимо отредактировать.");
            }
        }
        private void EditTeacherButton_Clicked(object sender, RoutedEventArgs e)
        {
            Teacher selectedTeacher = (Teacher)TeachersDataGrid.SelectedItem;
            if (selectedTeacher != null)
            {
                EditTeacherModalWindow editTeacherModalWindow = new EditTeacherModalWindow(selectedTeacher.first_name, selectedTeacher.last_name, selectedTeacher.subject, selectedTeacher.user_id);
                editTeacherModalWindow.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("Выберите учителя, данные которого необходимо отредактировать.");
            }
        }


        private void DeleteGroupButton_Clicked(object sender, RoutedEventArgs e)
        {
            Groups selectedGroup = (Groups)GroupsDataGrid.SelectedItem;

            if (selectedGroup != null)
            {
                using (SqlConnection connection = DbConnector.OpenConnection())
                {
                    SqlCommand deleteGroup = new SqlCommand("DELETE FROM Groups WHERE group_id=@id", connection);
                    deleteGroup.Parameters.AddWithValue("id", selectedGroup.group_id);
                    deleteGroup.ExecuteNonQuery();
                    System.Windows.MessageBox.Show($"Группа с идентификатором {selectedGroup.group_id} успешно удалена");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Выберите группу которую хотите удалить");
            }
        }
        private void DeleteStudentButton_Clicked(object sender, RoutedEventArgs e)
        {
            Student selectedStudent = (Student)StudentsDataGrid.SelectedItem;

            if (selectedStudent != null)
            {
                using (SqlConnection connection = DbConnector.OpenConnection())
                {
                    SqlCommand deleteStudent = new SqlCommand("DELETE FROM Students WHERE user_id=@id", connection);
                    deleteStudent.Parameters.AddWithValue("id", selectedStudent.user_id);
                    deleteStudent.ExecuteNonQuery();

                    SqlCommand deleteUser = new SqlCommand("DELETE FROM Users WHERE user_id=@id", connection);
                    deleteUser.Parameters.AddWithValue("id", selectedStudent.user_id);
                    deleteUser.ExecuteNonQuery();

                    System.Windows.MessageBox.Show($"Пользователь с идентификатором {selectedStudent.user_id} успешно удален");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Выберите пользователя которого хотите удалить");
            }
        }
        private void DeleteTeacherButton_Clicked(object sender, RoutedEventArgs e)
        {
            Teacher selectedTeacher = (Teacher)TeachersDataGrid.SelectedItem;

            if (selectedTeacher != null)
            {
                using (SqlConnection connection = DbConnector.OpenConnection())
                {
                    SqlCommand deleteStudent = new SqlCommand("DELETE FROM Teachers WHERE user_id=@id", connection);
                    deleteStudent.Parameters.AddWithValue("id", selectedTeacher.user_id);
                    deleteStudent.ExecuteNonQuery();

                    SqlCommand deleteUser = new SqlCommand("DELETE FROM Users WHERE user_id=@id", connection);
                    deleteUser.Parameters.AddWithValue("id", selectedTeacher.user_id);
                    deleteUser.ExecuteNonQuery();

                    System.Windows.MessageBox.Show($"Пользователь с идентификатором {selectedTeacher.user_id} успешно удален");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Выберите пользователя которого хотите удалить");
            }
        }


        private void UpdateGridButton_Clicked(object sender, RoutedEventArgs e)
        {
            GetData();
        }
    }
}
