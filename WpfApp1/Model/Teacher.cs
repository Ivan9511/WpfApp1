using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public class Teacher
    {
        public int teacher_id { get; set; }
        public int user_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string subject { get; set; }
    }
}
