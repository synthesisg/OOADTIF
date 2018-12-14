using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace ooadtest4_5.Models
{
    //Models
    public class admin
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string account { get; set; }
        public string password { get; set; }
    }
	public class student
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string account { get; set; }
        public string password { get; set; }
        public bool is_active { get; set; }
        public string studnet_name { get; set; }
        public string email { get; set; }
    }
	public class teacher
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string account { get; set; }
        public string password { get; set; }
        public string teacher_name { get; set; }
        public bool is_active { get; set; }
        public string email { get; set; }
    }
    public class team
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int klass_id { get; set; }
        public int course_id { get; set; }
        public int leader_id { get; set; }
        public string team_name { get; set; }
		public int team_serial { get; set; }
        public int status { get; set; }
    }
    public class course
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int teacher_id { get; set; }
        public string course_name { get; set; }
        public string introduction { get; set; }
        public int report_percentage { get; set; }
        public int presentation_percentage { get; set; }
        public int question_percentage { get; set; }
        public DateTime team_start_time { get; set; }
        public DateTime team_end_time { get; set; }
        public int? team_main_course_id { get; set; }
        public int? seminar_main_course_id { get; set; }
    }
	
    public class seminar
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int course_id { get; set; }
        public int round_id { get; set; }
        public string seminar_name { get; set; }
        public string introduction { get; set; }
        public int max_team { get; set; }
        public bool? is_visible { get; set; }
        public int seminar_serial { get; set; }
        public DateTime? enroll_start_time { get; set; }
        public DateTime? enroll_end_time { get; set; }
    }
	public class attendance
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int klass_seminar_id { get; set; }
        public int team_id { get; set; }
        public int team_order { get; set; }
        public bool is_present { get; set; }
        public string ppt_name { get; set; }
        public string report_name { get; set; }
        public string ppt_url { get; set; }
        public string report_url { get; set; }
    }
	public class round
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int course_id { get; set; }
        public int round_serial { get; set; }
        public bool presentation_score_method { get; set; }
        public bool report_score_method { get; set; }
        public bool question_score_method { get; set; }
    }
    public class klass
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int course_id { get; set; }
        public int grade { get; set; }
        public string klass_serial { get; set; }
        public string klass_location { get; set; }
        public string klass_time { get; set; }
    }
	public class question
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int klass_seminar_id { get; set; }
        public int attendance_id { get; set; }
        public int team_id { get; set; }
        public int student_id { get; set; }
        public bool is_selected { get; set; }
        public decimal? score { get; set; }
    }
	public class klass_seminar
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int klass_id { get; set; }
        public int seminar_id { get; set; }
        public DateTime? report_ddl { get; set; }
        public string seminar_status { get; set; }
    }
	public class klass_student
    {
        [Key, Column(Order = 0)]
        public int klass_id { get; set; }
        [Key, Column(Order = 1)]
        public int student_id { get; set; }
        public int course_id { get; set; }
        public int? team_id { get; set; }
    }
    public class klass_round
    {
        [Key, Column(Order = 0)]
        public int klass_id { get; set; }
        [Key, Column(Order = 1)]
        public int round_id { get; set; }
        public int? enroll_number { get; set; }
    }
	public class seminar_score
	{
        [Key, Column(Order = 0)]
        public int klass_seminar_id { get; set; }
        [Key, Column(Order = 1)]
        public int team_id { get; set; }
		public int? total_score { get; set; }
		public int? presentation_score { get; set; }
		public int? question_score { get; set; }
		public int? report_score { get; set; }
	}
	public class round_score
	{
        [Key, Column(Order = 0)]
        public int round_id { get; set; }
        [Key, Column(Order = 1)]
        public int team_id { get; set; }
		public int? total_score { get; set; }
		public int? presentation_score { get; set; }
		public int? question_score { get; set; }
		public int? report_score { get; set; }
	}
	public class conflict_course_strategy
	{
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int course_1_id { get; set; }
        public int course_2_id { get; set; }
	}
	public class member_limit_strategy
	{
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int? min_member { get; set; }
        public int? max_member { get; set; }
	}
	public class share_seminar_application
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int main_course_id { get; set; }
        public int sub_course_id { get; set; }
        public int sub_course_teacher_id { get; set; }
        public bool? status { get; set; }
    }
	
    public class share_team_application
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int main_course_id { get; set; }
        public int sub_course_id { get; set; }
        public int sub_course_teacher_id { get; set; }
        public bool? status { get; set; }
    }
    public class team_valid_application
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int team_id { get; set; }
        public int teacher_id { get; set; }
        public string reasoon { get; set; }
        public bool? status { get; set; }
    }
    public class team_strategy
	{
        [Key, Column(Order = 0)]
        public int course_id { get; set; }
        [Key, Column(Order = 1)]
        public int strategy_id { get; set; }
		public string strategy_name { get; set; }
	}
    public class course_member_limit_strategy
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int course_id { get; set; }
        public int min_member { get; set; }
        public int max_member { get; set; }
    }
    public class team_and_strategy
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string strategy_1_name { get; set; }
        public int strategy_1_id { get; set; }
        public string strategy_2_name { get; set; }
        public int strategy_2_id { get; set; }
    }
	public class team_or_strategy
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string strategy_1_name { get; set; }
        public int strategy_1_id { get; set; }
        public string strategy_2_name { get; set; }
        public int strategy_2_id { get; set; }
    }


    //Context
    public class adminDB : DbContext
    {
        public adminDB() : base("MySQLContext") { }
        public DbSet<admin> data { get; set; }
    }
    public class studentDB : DbContext
    {
        public studentDB() : base("MySQLContext") { }
        public DbSet<student> data { get; set; }
    }
	public class teacherDB : DbContext
    {
        public teacherDB() : base("MySQLContext") { }
        public DbSet<teacher> data { get; set; }
    }
	public class teamDB : DbContext
    {
        public teamDB() : base("MySQLContext") { }
        public DbSet<team> data { get; set; }
    }
    public class courseDB : DbContext
    {
        public courseDB() : base("MySQLContext") { }
        public DbSet<course> data { get; set; }
    }
	public class seminarDB : DbContext
    {
        public seminarDB() : base("MySQLContext") { }
        public DbSet<seminar> data { get; set; }
    }
    public class attendanceDB : DbContext
    {
        public attendanceDB() : base("MySQLContext") { }
        public DbSet<attendance> data { get; set; }
    }
	public class roundDB : DbContext
    {
        public roundDB() : base("MySQLContext") { }
        public DbSet<round> data { get; set; }
    }
    public class klassDB : DbContext
    {
        public klassDB() : base("MySQLContext") { }
        public DbSet<klass> data { get; set; }
    }
	public class questionDB : DbContext
    {
        public questionDB() : base("MySQLContext") { }
        public DbSet<question> data { get; set; }
    }
    public class klass_seminarDB : DbContext
    {
        public klass_seminarDB() : base("MySQLContext") { }
        public DbSet<klass_seminar> data { get; set; }
    }
    public class klass_studentDB : DbContext
    {
        public klass_studentDB() : base("MySQLContext") { }
        public DbSet<klass_student> data { get; set; }
    }
    public class klass_roundDB : DbContext
    {
        public klass_roundDB() : base("MySQLContext") { }
        public DbSet<klass_round> data { get; set; }
    }
	public class seminar_scoreDB : DbContext
    {
        public seminar_scoreDB() : base("MySQLContext") { }
        public DbSet<seminar_score> data { get; set; }
    }
	public class round_scoreDB : DbContext
    {
        public round_scoreDB() : base("MySQLContext") { }
        public DbSet<round_score> data { get; set; }
    }
	public class conflict_course_strategyDB : DbContext
    {
        public conflict_course_strategyDB() : base("MySQLContext") { }
        public DbSet<conflict_course_strategy> data { get; set; }
    }
	public class member_limit_strategyDB : DbContext
    {
        public member_limit_strategyDB() : base("MySQLContext") { }
        public DbSet<member_limit_strategy> data { get; set; }
    }
    public class share_team_applicationDB : DbContext
    {
        public share_team_applicationDB() : base("MySQLContext") { }
        public DbSet<share_team_application> data { get; set; }
    }
    public class team_valid_applicationDB : DbContext
    {
        public team_valid_applicationDB() : base("MySQLContext") { }
        public DbSet<team_valid_application> data { get; set; }
    }
    public class share_seminar_applicationDB : DbContext
    {
        public share_seminar_applicationDB() : base("MySQLContext") { }
        public DbSet<share_seminar_application> data { get; set; }
    }
	public class team_strategyDB : DbContext
    {
        public team_strategyDB() : base("MySQLContext") { }
        public DbSet<team_strategy> data { get; set; }
    }
	public class course_member_limit_strategyDB : DbContext
    {
        public course_member_limit_strategyDB() : base("MySQLContext") { }
        public DbSet<course_member_limit_strategy> data { get; set; }
    }
    public class team_and_strategyDB : DbContext
    {
        public team_and_strategyDB() : base("MySQLContext") { }
        public DbSet<team_and_strategy> data { get; set; }
    }
    public class team_or_strategyDB:DbContext
    {
        public team_or_strategyDB() : base("MySQLContext") { }
        public DbSet<team_or_strategy> data { get; set; }
    }
}