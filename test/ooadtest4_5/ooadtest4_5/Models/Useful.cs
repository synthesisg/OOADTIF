using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace ooadtest4_5.Models
{
    public class Useful
    {
    }

    //Models
    public class admin
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string admin_name { get; set; }
        public string admin_pwd { get; set; }
    }
    public class userinfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string academic_id { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public int? interval { get; set; }
        public bool? is_student { get; set; }
        public bool? is_valid { get; set; }
    }
    public class course
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int teacher_id { get; set; }
        public int? report_percentage { get; set; }
        public int? presentation_percentage { get; set; }
        public int? question_percentage { get; set; }
        public bool? is_group { get; set; }
        public DateTime? start_datetime { get; set; }
        public DateTime? end_datetime { get; set; }
        public int? max_member_limit { get; set; }
        public int? min_member_limit { get; set; }
    }
    public class class1
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int course_id { get; set; }
        public string name { get; set; }
        public string site { get; set; }
        public string time { get; set; }
    }
    public class select_course
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int class_id { get; set; }
        public int student_id { get; set; }
        public bool? hasteam { get; set; }
    }
    public class team_member
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int team_id { get; set; }
        public int student_id { get; set; }
    }
    public class team
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int class_id { get; set; }
        public string inclass_id { get; set; }
        public string name { get; set; }
        public int leader_id { get; set; }
        public bool? is_valid { get; set; }
    }
    public class round
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int course_id { get; set; }
        public int number { get; set; }
        public bool is_pre_avg { get; set; }
        public bool is_rep_avg { get; set; }
        public bool is_que_avg { get; set; }
    }
    public class class_round
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int round_id { get; set; }
        public int class_id { get; set; }
        public int times { get; set; }
    }
    public class round_seminar
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int class_round_id { get; set; }
        public int seminar_id { get; set; }
        public DateTime? reportddl { get; set; }
    }
    public class seminar
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int course_id { get; set; }
        public string theme { get; set; }
        public string description { get; set; }
        public int order { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public bool? is_visible { get; set; }
        public int max_team_limit { get; set; }
        public bool? is_sequence { get; set; }
        public bool? state { get; set; }
    }
    public class presentation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int round_seminar_id { get; set; }
        public int team_id { get; set; }
        public int queue_no { get; set; }
        public string presentation_file { get; set; }
        public decimal? presentation_score { get; set; }
        public string report_file { get; set; }
        public decimal? report_score { get; set; }
        public DateTime timer { get; set; }
    }
    public class question
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int student_id { get; set; }
        public int presentateion_id { get; set; }
        public bool? is_finished { get; set; }
        public decimal? question_score { get; set; }
    }
    public class team_share
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int from_course_id { get; set; }
        public bool? is_src_agree { get; set; }
        public int to_course_id { get; set; }
        public bool? is_dst_agree { get; set; }
        public bool? state { get; set; }
    }
    public class team_submit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int team_id { get; set; }
        public bool? state { get; set; }
    }
    public class seminar_share
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int from_course_id { get; set; }
        public bool? is_src_agree { get; set; }
        public int to_course_id { get; set; }
        public bool? is_dst_agree { get; set; }
        public bool? state { get; set; }
    }
    public class verification_code
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_id { get; set; }
        public string code { get; set; }
        public DateTime time { get; set; }
    }
    public class notice
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_id { get; set; }
        public string content { get; set; }
        public DateTime time { get; set; }
        public bool? is_read { get; set; }
    }


    //Context
    public class adminDB : DbContext
    {
        public adminDB() : base("SQLConnectionString") { }
        public DbSet<admin> data { get; set; }
    }
    public class userinfoDB : DbContext
    {
        public userinfoDB() : base("SQLConnectionString") { }
        public DbSet<userinfo> data { get; set; }
    }
    public class courseDB : DbContext
    {
        public courseDB() : base("SQLConnectionString") { }
        public DbSet<course> data { get; set; }
    }
    public class class1DB : DbContext
    {
        public class1DB() : base("SQLConnectionString") { }
        public DbSet<class1> data { get; set; }
    }
    public class select_courseDB : DbContext
    {
        public select_courseDB() : base("SQLConnectionString") { }
        public DbSet<select_course> data { get; set; }
    }
    public class team_memberDB : DbContext
    {
        public team_memberDB() : base("SQLConnectionString") { }
        public DbSet<team_member> data { get; set; }
    }
    public class teamDB : DbContext
    {
        public teamDB() : base("SQLConnectionString") { }
        public DbSet<team> data { get; set; }
    }
    public class roundDB : DbContext
    {
        public roundDB() : base("SQLConnectionString") { }
        public DbSet<round> data { get; set; }
    }
    public class class_roundDB : DbContext
    {
        public class_roundDB() : base("SQLConnectionString") { }
        public DbSet<class_round> data { get; set; }
    }
    public class round_seminarDB : DbContext
    {
        public round_seminarDB() : base("SQLConnectionString") { }
        public DbSet<round_seminar> data { get; set; }
    }
    public class seminarDB : DbContext
    {
        public seminarDB() : base("SQLConnectionString") { }
        public DbSet<seminar> data { get; set; }
    }
    public class presentationDB : DbContext
    {
        public presentationDB() : base("SQLConnectionString") { }
        public DbSet<presentation> data { get; set; }
    }
    public class questionDB : DbContext
    {
        public questionDB() : base("SQLConnectionString") { }
        public DbSet<question> data { get; set; }
    }
    public class team_shareDB : DbContext
    {
        public team_shareDB() : base("SQLConnectionString") { }
        public DbSet<team_share> data { get; set; }
    }
    public class team_submitDB : DbContext
    {
        public team_submitDB() : base("SQLConnectionString") { }
        public DbSet<team_submit> data { get; set; }
    }
    public class seminar_shareDB : DbContext
    {
        public seminar_shareDB() : base("SQLConnectionString") { }
        public DbSet<seminar_share> data { get; set; }
    }
    public class verification_codeDB : DbContext
    {
        public verification_codeDB() : base("SQLConnectionString") { }
        public DbSet<verification_code> data { get; set; }
    }
    public class noticeDB:DbContext
    {
        public noticeDB() : base("SQLConnectionString") { }
        public DbSet<notice> data { get; set; }
    }
}