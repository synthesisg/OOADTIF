/*
SQLyog Ultimate v13.1.1 (64 bit)
MySQL - 5.7.24-0ubuntu0.16.04.1 : Database - standard_seminar_system
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`standard_seminar_system` /*!40100 DEFAULT CHARACTER SET utf8 */;

USE `standard_seminar_system`;

/*Table structure for table `admin` */

DROP TABLE IF EXISTS `admin`;

CREATE TABLE `admin` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `account` varchar(20) NOT NULL COMMENT '账号',
  `password` varchar(20) NOT NULL COMMENT '密码',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `admin` */

/*Table structure for table `attendance` */

DROP TABLE IF EXISTS `attendance`;

CREATE TABLE `attendance` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `klass_seminar_id` bigint(20) unsigned NOT NULL COMMENT '讨论课（某班级）id',
  `team_id` bigint(20) unsigned NOT NULL COMMENT '队伍id',
  `team_order` tinyint(4) unsigned NOT NULL COMMENT '该队伍顺序',
  `is_present` tinyint(4) unsigned NOT NULL COMMENT '是否正在进行',
  `report_name` varchar(30) DEFAULT NULL COMMENT '提交的报告文件名',
  `report_url` varchar(50) DEFAULT NULL COMMENT '提交的报告文件位置',
  `ppt_name` varchar(30) DEFAULT NULL COMMENT '提交的PPT文件名',
  `ppt_url` varchar(50) DEFAULT NULL COMMENT '提交的PPT文件位置',
  PRIMARY KEY (`id`),
  KEY `idx_team_id` (`team_id`),
  KEY `idx_klass_seminar_id` (`klass_seminar_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `attendance` */

/*Table structure for table `conflict_course_strategy` */

DROP TABLE IF EXISTS `conflict_course_strategy`;

CREATE TABLE `conflict_course_strategy` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `course_1_id` bigint(20) unsigned NOT NULL COMMENT '冲突课程1',
  `course_2_id` bigint(20) unsigned NOT NULL COMMENT '冲突课程2',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `conflict_course_strategy` */

/*Table structure for table `course` */

DROP TABLE IF EXISTS `course`;

CREATE TABLE `course` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `teacher_id` bigint(20) unsigned NOT NULL COMMENT '教师id',
  `course_name` varchar(30) NOT NULL COMMENT '课程名称',
  `introduction` varchar(500) DEFAULT NULL COMMENT '课程介绍',
  `presentation_percentage` tinyint(4) unsigned NOT NULL COMMENT '展示分数占比',
  `question_percentage` tinyint(4) unsigned NOT NULL COMMENT '提问分数占比',
  `report_percentage` tinyint(4) unsigned NOT NULL COMMENT '报告分数占比',
  `team_start_time` datetime NOT NULL COMMENT '开始组队时间',
  `team_end_time` datetime NOT NULL COMMENT '截止组队时间',
  `team_main_course_id` bigint(20) unsigned DEFAULT NULL COMMENT '共享分组，主课程id',
  `seminar_main_course_id` bigint(20) unsigned DEFAULT NULL COMMENT '共享讨论课，主课程id',
  PRIMARY KEY (`id`),
  KEY `idx_teacher_id` (`teacher_id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8;

/*Data for the table `course` */

/*Table structure for table `course_member_limit_strategy` */

DROP TABLE IF EXISTS `course_member_limit_strategy`;

CREATE TABLE `course_member_limit_strategy` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `course_id` tinyint(4) unsigned NOT NULL COMMENT '课程id',
  `min_member` tinyint(4) unsigned DEFAULT NULL COMMENT '队伍中选该课程最少人数',
  `max_member` tinyint(4) unsigned DEFAULT NULL COMMENT '队伍中选该课程最多人数',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `course_member_limit_strategy` */

/*Table structure for table `klass` */

DROP TABLE IF EXISTS `klass`;

CREATE TABLE `klass` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `course_id` bigint(20) unsigned NOT NULL COMMENT '课程id',
  `grade` int(10) unsigned NOT NULL COMMENT '年级',
  `klass_serial` tinyint(4) unsigned NOT NULL COMMENT '班级序号',
  `klass_time` varchar(20) NOT NULL COMMENT '上课时间',
  `klass_location` varchar(20) NOT NULL COMMENT '上课地点',
  PRIMARY KEY (`id`),
  KEY `idx_course_id` (`course_id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8;

/*Data for the table `klass` */

/*Table structure for table `klass_round` */

DROP TABLE IF EXISTS `klass_round`;

CREATE TABLE `klass_round` (
  `klass_id` bigint(20) unsigned NOT NULL COMMENT '课程id',
  `round_id` bigint(20) unsigned NOT NULL COMMENT '轮次id',
  `enroll_number` tinyint(3) unsigned DEFAULT NULL COMMENT '某班级，某轮次队伍报名次数限制',
  PRIMARY KEY (`klass_id`,`round_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `klass_round` */

/*Table structure for table `klass_seminar` */

DROP TABLE IF EXISTS `klass_seminar`;

CREATE TABLE `klass_seminar` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `klass_id` bigint(20) unsigned NOT NULL COMMENT '班级id',
  `seminar_id` bigint(20) unsigned NOT NULL COMMENT '讨论课id\n',
  `report_ddl` datetime DEFAULT NULL COMMENT '报告截止时间',
  `status` tinyint(4) NOT NULL COMMENT '讨论课所处状态，未开始0，正在进行1，已结束2，暂停3',
  PRIMARY KEY (`id`),
  KEY `idx_klass_id` (`klass_id`),
  KEY `idx_seminar_id` (`seminar_id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8;

/*Data for the table `klass_seminar` */

/*Table structure for table `klass_student` */

DROP TABLE IF EXISTS `klass_student`;

CREATE TABLE `klass_student` (
  `klass_id` bigint(20) unsigned NOT NULL COMMENT '班级id',
  `student_id` bigint(20) unsigned NOT NULL COMMENT '学生id',
  `course_id` bigint(20) unsigned NOT NULL COMMENT '课程id',
  `team_id` bigint(20) unsigned DEFAULT NULL COMMENT '学生所在小组id',
  PRIMARY KEY (`klass_id`,`student_id`),
  KEY `idx_team_id` (`team_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `klass_student` */

/*Table structure for table `member_limit_strategy` */

DROP TABLE IF EXISTS `member_limit_strategy`;

CREATE TABLE `member_limit_strategy` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `min_member` tinyint(4) unsigned DEFAULT NULL COMMENT '最少人数',
  `max_member` tinyint(4) unsigned DEFAULT NULL COMMENT '最多人数',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `member_limit_strategy` */

/*Table structure for table `question` */

DROP TABLE IF EXISTS `question`;

CREATE TABLE `question` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `klass_seminar_id` bigint(20) unsigned NOT NULL,
  `attendance_id` bigint(20) unsigned NOT NULL COMMENT '问题所针对的发言id',
  `team_id` bigint(20) unsigned NOT NULL COMMENT '提问小组的id',
  `student_id` bigint(20) unsigned NOT NULL COMMENT '提问学生的id',
  `is_selected` tinyint(4) unsigned NOT NULL COMMENT '是否被选中',
  `score` decimal(4,1) DEFAULT NULL COMMENT '提问分数',
  PRIMARY KEY (`id`),
  KEY `idx_team_id` (`team_id`),
  KEY `idx_klass_seminar_id` (`klass_seminar_id`),
  KEY `idx_attendance_id` (`attendance_id`),
  KEY `idx_student_id` (`student_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `question` */

/*Table structure for table `round` */

DROP TABLE IF EXISTS `round`;

CREATE TABLE `round` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `course_id` bigint(20) unsigned NOT NULL COMMENT '课程id',
  `round_serial` tinyint(4) unsigned NOT NULL COMMENT '轮次序号',
  `presentation_score_method` tinyint(4) unsigned NOT NULL COMMENT '本轮展示分数计算方法',
  `report_score_method` tinyint(4) unsigned NOT NULL COMMENT '本轮报告分数计算方法',
  `question_score_method` tinyint(4) unsigned NOT NULL COMMENT '本轮提问分数计算方法',
  PRIMARY KEY (`id`),
  KEY `idx_course_id` (`course_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

/*Data for the table `round` */

/*Table structure for table `round_score` */

DROP TABLE IF EXISTS `round_score`;

CREATE TABLE `round_score` (
  `round_id` bigint(20) unsigned NOT NULL COMMENT '轮次id',
  `team_id` bigint(20) unsigned NOT NULL COMMENT '小组id',
  `total_score` decimal(4,1) DEFAULT NULL COMMENT '总成绩',
  `presentation_score` decimal(4,1) DEFAULT NULL COMMENT '展示成绩',
  `question_score` decimal(4,1) DEFAULT NULL COMMENT '提问成绩',
  `report_score` decimal(4,1) DEFAULT NULL COMMENT '报告成绩',
  PRIMARY KEY (`round_id`,`team_id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `round_score` */

/*Table structure for table `seminar` */

DROP TABLE IF EXISTS `seminar`;

CREATE TABLE `seminar` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `course_id` bigint(20) unsigned NOT NULL COMMENT '课程id',
  `round_id` bigint(20) unsigned NOT NULL COMMENT '轮次id',
  `seminar_name` varchar(30) NOT NULL COMMENT '讨论课名称',
  `introduction` varchar(500) DEFAULT NULL COMMENT '讨论课介绍',
  `max_team` tinyint(4) unsigned NOT NULL COMMENT '报名讨论课最多组数',
  `is_visible` tinyint(4) unsigned NOT NULL COMMENT '是否可见',
  `seminar_serial` tinyint(4) unsigned NOT NULL COMMENT '讨论课序号',
  `enroll_start_time` datetime DEFAULT NULL COMMENT '讨论课报名开始时间',
  `enroll_end_time` datetime DEFAULT NULL COMMENT '讨论课报名截止时间',
  PRIMARY KEY (`id`),
  KEY `idx_course_id` (`course_id`),
  KEY `idx_round_id` (`round_id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

/*Data for the table `seminar` */

/*Table structure for table `seminar_score` */

DROP TABLE IF EXISTS `seminar_score`;

CREATE TABLE `seminar_score` (
  `klass_seminar_id` bigint(20) unsigned NOT NULL COMMENT '班级讨论课id',
  `team_id` bigint(20) unsigned NOT NULL COMMENT '小组id',
  `total_score` decimal(4,1) DEFAULT NULL COMMENT '总成绩',
  `presentaton_score` decimal(4,1) DEFAULT NULL COMMENT '展示成绩',
  `question_score` decimal(4,1) DEFAULT NULL COMMENT '提问成绩',
  `report_score` decimal(4,1) DEFAULT NULL COMMENT '报告成绩',
  PRIMARY KEY (`klass_seminar_id`,`team_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `seminar_score` */

/*Table structure for table `share_seminar_application` */

DROP TABLE IF EXISTS `share_seminar_application`;

CREATE TABLE `share_seminar_application` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `main_course_id` bigint(20) unsigned NOT NULL COMMENT '主课程id',
  `sub_course_id` bigint(20) unsigned NOT NULL COMMENT '从课程id',
  `sub_course_teacher_id` bigint(20) unsigned NOT NULL COMMENT '从课程的教师id',
  `status` tinyint(4) unsigned DEFAULT NULL COMMENT '请求状态，同意1、不同意0、未处理null',
  PRIMARY KEY (`id`),
  KEY `idx_main_course_id` (`main_course_id`),
  KEY `idx_sub_course_id` (`sub_course_id`),
  KEY `idx_sub_course_teacher_id` (`sub_course_teacher_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `share_seminar_application` */

/*Table structure for table `share_team_application` */

DROP TABLE IF EXISTS `share_team_application`;

CREATE TABLE `share_team_application` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `main_course_id` bigint(20) unsigned NOT NULL COMMENT '主课程id',
  `sub_course_id` bigint(20) unsigned NOT NULL COMMENT '从课程id',
  `sub_course_teacher_id` bigint(20) unsigned NOT NULL COMMENT '从课程老师id',
  `status` tinyint(4) unsigned DEFAULT NULL COMMENT '请求状态，同意1、不同意0、未处理null',
  PRIMARY KEY (`id`),
  KEY `idx_main_course_id` (`main_course_id`),
  KEY `idx_sub_course_id` (`sub_course_id`),
  KEY `idx_sub_course_teacher_id` (`sub_course_teacher_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `share_team_application` */

/*Table structure for table `student` */

DROP TABLE IF EXISTS `student`;

CREATE TABLE `student` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `account` varchar(20) NOT NULL COMMENT '学生账户',
  `password` varchar(20) NOT NULL COMMENT '账户密码',
  `is_active` tinyint(4) unsigned NOT NULL COMMENT '账号是否激活',
  `student_name` varchar(10) NOT NULL COMMENT '学生姓名',
  `email` varchar(30) DEFAULT NULL COMMENT '邮箱地址',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_account` (`account`),
  KEY `idx_password` (`password`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

/*Data for the table `student` */

/*Table structure for table `teacher` */

DROP TABLE IF EXISTS `teacher`;

CREATE TABLE `teacher` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `account` varchar(20) NOT NULL COMMENT '教师账户',
  `password` varchar(20) NOT NULL COMMENT '账户密码',
  `teacher_name` varchar(10) NOT NULL COMMENT '教师姓名',
  `is_active` tinyint(4) unsigned NOT NULL COMMENT '账号是否激活',
  `email` varchar(30) NOT NULL COMMENT '邮箱地址',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_account` (`account`),
  KEY `idx_password` (`password`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

/*Data for the table `teacher` */

/*Table structure for table `team` */

DROP TABLE IF EXISTS `team`;

CREATE TABLE `team` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `klass_id` bigint(20) unsigned NOT NULL COMMENT '班级序号',
  `course_id` bigint(20) unsigned NOT NULL COMMENT '课程序号',
  `leader_id` bigint(20) unsigned NOT NULL COMMENT '队长的学生id',
  `team_name` varchar(30) NOT NULL COMMENT '队伍名称',
  `team_serial` tinyint(4) unsigned NOT NULL COMMENT '队伍序号',
  `status` tinyint(4) unsigned NOT NULL COMMENT '队伍状态，不合法0、合法1、审核中2',
  PRIMARY KEY (`id`),
  KEY `idx_course_id` (`course_id`),
  KEY `idx_leader_id` (`leader_id`),
  KEY `idx_klass_id` (`klass_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

/*Data for the table `team` */

/*Table structure for table `team_and_strategy` */

DROP TABLE IF EXISTS `team_and_strategy`;

CREATE TABLE `team_and_strategy` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `strategy_1_name` varchar(50) NOT NULL COMMENT '“与”组队策略1实现类名称',
  `strategy_1_id` bigint(20) unsigned NOT NULL COMMENT '“与”组队策略1_id',
  `strategy_2_name` varchar(50) NOT NULL COMMENT '“与”组队策略2实现类名称',
  `strategy_2_id` bigint(20) unsigned NOT NULL COMMENT '“与”组队策略2_id',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `team_and_strategy` */

/*Table structure for table `team_or_strategy` */

DROP TABLE IF EXISTS `team_or_strategy`;

CREATE TABLE `team_or_strategy` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `strategy_1_name` varchar(50) NOT NULL COMMENT '“或”组队策略1实现类名称',
  `strategy_1_id` bigint(20) unsigned NOT NULL COMMENT '“或”组队策略1_id',
  `strategy_2_name` varchar(50) NOT NULL COMMENT '“或”组队策略2实现类名称',
  `strategy_2_id` bigint(20) unsigned NOT NULL COMMENT '“或”组队策略2_id',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `team_or_strategy` */

/*Table structure for table `team_strategy` */

DROP TABLE IF EXISTS `team_strategy`;

CREATE TABLE `team_strategy` (
  `course_id` bigint(20) unsigned NOT NULL AUTO_INCREMENT COMMENT '课程id',
  `strategy_id` bigint(20) unsigned NOT NULL COMMENT '策略id',
  `strategy_name` varchar(50) NOT NULL COMMENT '组队策略实现类名称',
  PRIMARY KEY (`course_id`,`strategy_id`,`strategy_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `team_strategy` */

/*Table structure for table `team_valid_application` */

DROP TABLE IF EXISTS `team_valid_application`;

CREATE TABLE `team_valid_application` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `team_id` bigint(20) unsigned NOT NULL COMMENT '小组id',
  `teacher_id` bigint(20) unsigned NOT NULL COMMENT '教师id',
  `reason` varchar(500) DEFAULT NULL COMMENT '申请理由',
  `status` tinyint(4) unsigned DEFAULT NULL COMMENT '请求状态，同意1、不同意0、未处理null',
  PRIMARY KEY (`id`),
  KEY `idx_team_id` (`team_id`),
  KEY `idx_teacher_id` (`teacher_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `team_valid_application` */

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
