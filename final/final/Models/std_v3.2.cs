using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace final.Models
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
        public byte is_active { get; set; }
        public string student_name { get; set; }
        public string email { get; set; }
    }
    public class teacher
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string account { get; set; }
        public string password { get; set; }
        public string teacher_name { get; set; }
        public byte is_active { get; set; }
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
        public byte team_serial { get; set; }
        public byte? klass_serial { get; set; }
        public byte status { get; set; }
    }
    public class course
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int teacher_id { get; set; }
        public string course_name { get; set; }
        public string introduction { get; set; }
        public byte presentation_percentage { get; set; }
        public byte question_percentage { get; set; }
        public byte report_percentage { get; set; }
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
        public byte max_team { get; set; }
        public byte is_visible { get; set; }
        public byte seminar_serial { get; set; }
        public Nullable<DateTime> enroll_start_time { get; set; }
        public Nullable<DateTime> enroll_end_time { get; set; }
    }
    public class attendance
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int klass_seminar_id { get; set; }
        public int team_id { get; set; }
        public byte team_order { get; set; }
        public byte is_present { get; set; }
        public string report_name { get; set; }
        public string report_url { get; set; }
        public string ppt_name { get; set; }
        public string ppt_url { get; set; }
    }
    public class round
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int course_id { get; set; }
        public byte round_serial { get; set; }
        public byte presentation_score_method { get; set; }
        public byte report_score_method { get; set; }
        public byte question_score_method { get; set; }
    }
    public class klass
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int course_id { get; set; }
        public long grade { get; set; }
        public byte klass_serial { get; set; }
        public string klass_time { get; set; }
        public string klass_location { get; set; }
    }
    public class question
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int klass_seminar_id { get; set; }
        public int attendance_id { get; set; }
        public int team_id { get; set; }
        public int student_id { get; set; }
        public byte is_selected { get; set; }
        public decimal? score { get; set; }
    }
    public class klass_seminar
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int klass_id { get; set; }
        public int seminar_id { get; set; }
        public DateTime? report_ddl { get; set; }
        public short status { get; set; }
    }
    //*
	public class klass_student
    {
        [Key, Column(Order = 0)]
        public int klass_id { get; set; }
        [Key, Column(Order = 1)]
        public int student_id { get; set; }
        public int course_id { get; set; }
        public int? team_id { get; set; }//===================================================================================================
    }
	//*/
    public class klass_round
    {
        [Key, Column(Order = 0)]
        public int klass_id { get; set; }
        [Key, Column(Order = 1)]
        public int round_id { get; set; }
        public byte enroll_number { get; set; }
    }
    public class seminar_score
    {
        [Key, Column(Order = 0)]
        public int klass_seminar_id { get; set; }
        [Key, Column(Order = 1)]
        public int team_id { get; set; }
        public decimal? total_score { get; set; }
        public decimal? presentation_score { get; set; }
        public decimal? question_score { get; set; }
        public decimal? report_score { get; set; }
    }
    public class round_score
    {
        [Key, Column(Order = 0)]
        public int round_id { get; set; }
        [Key, Column(Order = 1)]
        public int team_id { get; set; }
        public decimal? total_score { get; set; }
        public decimal? presentation_score { get; set; }
        public decimal? question_score { get; set; }
        public decimal? report_score { get; set; }
    }

    public class share_seminar_application
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int main_course_id { get; set; }
        public int sub_course_id { get; set; }
        public int sub_course_teacher_id { get; set; }
        public byte? status { get; set; }
    }
    public class share_team_application
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int main_course_id { get; set; }
        public int sub_course_id { get; set; }
        public int sub_course_teacher_id { get; set; }
        public byte? status { get; set; }
    }
    public class team_valid_application
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int team_id { get; set; }
        public int teacher_id { get; set; }
        public string reason { get; set; }
        public byte? status { get; set; }
    }

    public class conflict_course_strategy//0
    {
        [Key, Column(Order = 0)]
        public int id { get; set; }
        [Key, Column(Order = 1)]
        public int course_id { get; set; }
    }
    public class course_member_limit_strategy//1
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int course_id { get; set; }
        public byte? min_member { get; set; }
        public byte? max_member { get; set; }
    }
    public class member_limit_strategy//1
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int course_id { get; set; }
        public byte? min_member { get; set; }
        public byte? max_member { get; set; }
    }
    public class team_and_strategy//0
    {
        [Key, Column(Order = 0)]
        public int id { get; set; }
        [Key, Column(Order = 1)]
        public string strategy_name { get; set; }
        [Key, Column(Order = 2)]
        public int strategy_id { get; set; }
    }
    public class team_or_strategy//0
    {
        [Key, Column(Order = 0)]
        public int id { get; set; }
        [Key, Column(Order = 1)]
        public string strategy_name { get; set; }
        [Key, Column(Order = 2)]
        public int strategy_id { get; set; }
    }
    public class team_strategy//0
    {
        [Key, Column(Order = 0)]
        public int course_id { get; set; }
        [Key, Column(Order = 1)]
        public int strategy_serial { get; set; }
        public int strategy_id { get; set; }
        public string strategy_name { get; set; }
    }


    public class klass_team
    {
        [Key, Column(Order = 0)]
        public int klass_id { get; set; }
        [Key, Column(Order = 1)]
        public int team_id { get; set; }
    }
    public class team_student
    {
        [Key, Column(Order = 0)]
        public int team_id { get; set; }
        [Key, Column(Order = 1)]
        public int student_id { get; set; }
    }


    //Context
    public class MSSQLContext : DbContext
    {
        public MSSQLContext()
            : base("SQLServerContext")
        {
        }

        public DbSet<admin> admin { get; set; }
        public DbSet<attendance> attendance { get; set; }
        public DbSet<conflict_course_strategy> conflict_course_strategy { get; set; }
        public DbSet<course> course { get; set; }
        public DbSet<course_member_limit_strategy> course_member_limit_strategy { get; set; }
        public DbSet<klass> klass { get; set; }
        public DbSet<klass_round> klass_round { get; set; }
        public DbSet<klass_seminar> klass_seminar { get; set; }
        public DbSet<klass_student> klass_student { get; set; }
        public DbSet<member_limit_strategy> member_limit_strategy { get; set; }
        public DbSet<question> question { get; set; }
        public DbSet<round> round { get; set; }
        public DbSet<round_score> round_score { get; set; }
        public DbSet<seminar> seminar { get; set; }
        public DbSet<seminar_score> seminar_score { get; set; }
        public DbSet<share_seminar_application> share_seminar_application { get; set; }
        public DbSet<share_team_application> share_team_application { get; set; }
        public DbSet<student> student { get; set; }
        public DbSet<teacher> teacher { get; set; }
        public DbSet<team> team { get; set; }
        public DbSet<team_and_strategy> team_and_strategy { get; set; }
        public DbSet<team_or_strategy> team_or_strategy { get; set; }
        public DbSet<team_strategy> team_strategy { get; set; }
        public DbSet<team_valid_application> team_valid_application { get; set; }


        public DbSet<klass_team> klass_team { get; set; }
        public DbSet<team_student> team_student { get; set; }
    }
}