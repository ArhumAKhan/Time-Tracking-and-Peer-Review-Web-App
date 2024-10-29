-- set tracker schema as default
-- drop table tracker.time_logs;
-- drop table tracker.courses;
-- drop table tracker.users; 

-- create tables
 CREATE TABLE tracker.users (
    utd_id INT PRIMARY KEY,
    net_id VARCHAR(50) UNIQUE,
    password VARCHAR(50) not null,
    last_name VARCHAR(50) not null,
    first_name VARCHAR(50) not null,
	user_type VARCHAR(1) not null
);

 CREATE TABLE tracker.courses ( 
 course_id varchar(10) PRIMARY KEY, 
 course_name VARCHAR(100),
 professor_id INT,
 FOREIGN KEY (professor_id) REFERENCES users(utd_id)
 ); 
 
CREATE TABLE tracker.time_logs ( 
log_id INT PRIMARY KEY AUTO_INCREMENT, 
utd_id INT, 
course_id varchar(10), 
log_date DATE, 
hours_logged int, 
minutes_logged  int, 
work_desc VARCHAR(150),
UNIQUE KEY unique_log (utd_id, log_date),
FOREIGN KEY (utd_id) REFERENCES users(utd_id), 
FOREIGN KEY (course_id) REFERENCES courses(course_id) );

INSERT INTO users (utd_id, net_id, password, last_name, first_name, user_type) 
VALUES (1, 'john_doe', 'pass123doe', 'Doe', 'John', 'S');

INSERT INTO users (utd_id, net_id, password, last_name, first_name, user_type) 
VALUES (2021504000, 'axa190000', 'pass123axa190000', 'Acharya', 'Prakash', 'S');

INSERT INTO users (utd_id, net_id, password, last_name, first_name, user_type) 
VALUES (2021482111, 'dxa190111', 'pass123dxa190111', 'Priya', 'Dhanushu', 'S');

INSERT INTO users (utd_id, net_id, password, last_name, first_name, user_type) 
VALUES (2021393333, 'sib170121', 'pass123sib170121', 'Barrameda', 'David', 'S');

INSERT INTO users (utd_id, net_id, password, last_name, first_name, user_type) 
VALUES (2021542222, 'nxb200088', 'pass123nxb200088', 'Bollepalli', 'Darwin', 'S');

INSERT INTO users (utd_id, net_id, password, last_name, first_name, user_type) 
VALUES (2021308444, 'cab160444', 'pass123cab160444', 'Burrell', 'Chase', 'S');

INSERT INTO users (utd_id, net_id, password, last_name, first_name, user_type) 
VALUES (2021345555, 'nkc160199', 'pass123nkc160199', 'Chen', 'Kevin', 'S');

INSERT INTO users (utd_id, net_id, password, last_name, first_name, user_type) 
VALUES (2021188666, 'zjd130000', 'pass123zjd130000', 'Dewey', 'Zach', 'S');

INSERT INTO users (utd_id, net_id, password, last_name, first_name, user_type) 
VALUES (2024102700, 'prof130000', 'pass123zprof', 'Professor', 'One', 'P');

INSERT INTO users (utd_id, net_id, password, last_name, first_name, user_type) 
VALUES (2024102800, 'prof120000', 'pass456zprof', 'Professor', 'Two', 'P');

--- courses 
 insert into courses (course_id, course_name) values ('cs4485', 'Senior Design', 2024102700);
 insert into courses (course_id, course_name) values ('cs9999', 'Senior Design Advanced', 2024102800);
 insert into courses (course_id, course_name) values ('cs1111', 'Project Test', 2024102700);
 
 -- time logs
insert into time_logs (utd_id, course_id, log_date, hours_logged, minutes_logged, work_desc)
values( '2021504000', 'cs4485', '2024-10-18', '3', '20', 'Description of Work last week 3');

insert into time_logs (utd_id, course_id, log_date, hours_logged, minutes_logged, work_desc)
values( '2021504000', 'cs4485', '2024-10-10', '3', '50', 'Description of Work old entry');

insert into time_logs (utd_id, course_id, log_date, hours_logged, minutes_logged, work_desc)
values( '2021504000', 'cs4485', '2024-10-11', '2', '15', 'Description of Work old entry 2');
