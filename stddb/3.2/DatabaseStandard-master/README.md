# XMU'18 OOAD Database Working Group
This repo contains Public Releases and Offical Resources from XMU'18 OOAD Database Working Group

## Resources
 * [sql脚本](https://github.com/Black-W/DatabaseStandard/blob/master/sql/init.sql)
 * [数据字典](https://github.com/Black-W/DatabaseStandard/blob/master/%E6%95%B0%E6%8D%AE%E5%AD%97%E5%85%B8.md)
 * [架构设计图](https://raw.githubusercontent.com/Black-W/DatabaseStandard/master/%E6%9E%B6%E6%9E%84%E8%AE%BE%E8%AE%A1%E5%9B%BE.bmp)
 * [外键图](https://github.com/Black-W/DatabaseStandard/blob/master/%E5%A4%96%E9%94%AE%E5%9B%BE.png)
 * [策略表说明](https://github.com/Black-W/DatabaseStandard/blob/master/%E7%AD%96%E7%95%A5%E8%A1%A8%E8%AF%B4%E6%98%8E.md)

 ## 支持软件
 * MySQL 5.7

 ## 修订记录
| 版本 | 出版日期 | 修订内容 | 作者 | 
| --- | --- | --- | --- |
| 1.0 | 2018.12.12 | 初稿 (表中暂未插入数据)| 数据库标准组  |
| 2.0 | 2018.12.15 | 新增表：<br>team_and_strategy <br> team_or_strategy <br>course_member_limit_strategy <br> <br>删除表：<br>member_sex_strategy<br> <br>修改表：<br> - student.sex <br> * team team.is_valid -> team.status <br>  * student.email -> 允许为空 <br> * klass_seminar.status 类型改为 tinyint unsigned<br>* 纠正 attendance.report_url 拼写错误<br>* 删除所有表名中的 _table 后缀 <br> * 修改所有 decimal 字段为无符号 <br>* course、team、seminar的name扩展为varchar(30) <br> <br>新增文件：<br>策略表说明 .md <br>外键图.png| 数据库标准组  |
| 3.0 | 2018.12.25 | 新增表：<br>klass_team <br> team_student <br> <br>修改表：<br> strategy相关的表，详见策略说明 <br> <br> 录入数据:<br>OOAD、J2EE、.NET、SE四门课程(SE实际上是2门)<br>OOAD和J2EE的所有讨论课<br>OOAD、J2EE、SE的所有班级和学生名单<br> OOAD作为主课程，创建所有同学的分组信息（用l.xmu.edu.cn网上的信息创建，有的小组没写队名，用3-2这种代替队名）<br>OOAD和J2EE共享分组<br>讨论课分数，提问分数，报告分数 <br>OOAD前3条组队策略（策略一改为王、林课程冲突）<br>学生、教师、管理员账号<br>其他相关数据 <br> | 数据库标准组  |
| 3.1 | 2018.12.25 | 修改表：<br> team表新增klass_serial字段<br> <br> 录入数据:<br>OOAD和SE(王、林两门课)共享分组<br>team.klass_serial<br> <br> | 数据库标准组  |
| 3.2 | 2018.12.25 | 修改表：<br> team.klass_serial字段允许为空，各组自己决定是否使用<br>attendance表的report和ppt的name和url长度分别改为50和500<br><br>修改数据：<br> team_strategy的第三条strategy_id改为2<br> team_student表增加了1-12小组的成员(新增team_id为14的记录)<br>course_member_limit_strategy.max_member改为5<br>删除策略表中2个字段共同作为主键的自增id| 数据库标准组  |