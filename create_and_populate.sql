-- Disable foreign key checks to drop tables safely
SET FOREIGN_KEY_CHECKS = 0;

-- Drop tables if they exist
DROP TABLE IF EXISTS time_logs;
DROP TABLE IF EXISTS courses;
DROP TABLE IF EXISTS students;
DROP TABLE IF EXISTS professors;
DROP TABLE IF EXISTS users;

-- Re-enable foreign key checks
SET FOREIGN_KEY_CHECKS = 1;

-- Create the professors table
CREATE TABLE `professors` (
    `professor_id` int NOT NULL AUTO_INCREMENT,
    `first_name` varchar(50),
    `last_name` varchar(50),
    `email` varchar(100),
    `department` varchar(100),
    PRIMARY KEY (`professor_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Create the courses table
CREATE TABLE `courses` (
    `course_id` int NOT NULL AUTO_INCREMENT,
    `course_name` varchar(100) DEFAULT NULL,
    `professor_id` int DEFAULT NULL,
    PRIMARY KEY (`course_id`),
    KEY `professor_id` (`professor_id`),
    CONSTRAINT `courses_ibfk_1` FOREIGN KEY (`professor_id`) REFERENCES `professors` (`professor_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Create the students table
CREATE TABLE `students` (
    `student_id` int NOT NULL AUTO_INCREMENT,
    `first_name` varchar(50),
    `last_name` varchar(50),
    `email` varchar(100),
    `major` varchar(100),
    PRIMARY KEY (`student_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Create the time_logs table
CREATE TABLE `time_logs` (
    `log_id` int NOT NULL AUTO_INCREMENT,
    `student_id` int,
    `course_id` int,
    `log_date` date,
    `hours_logged` decimal(5,2),
    PRIMARY KEY (`log_id`),
    FOREIGN KEY (`student_id`) REFERENCES `students` (`student_id`),
    FOREIGN KEY (`course_id`) REFERENCES `courses` (`course_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Create the users table
CREATE TABLE `users` (
    `user_id` int NOT NULL AUTO_INCREMENT,
    `netID` varchar(9) NOT NULL UNIQUE,
    `password` varchar(255) NOT NULL,
    `role` enum('student', 'professor', 'admin') NOT NULL,
    PRIMARY KEY (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Insert sample data into the professors table
INSERT INTO professors (first_name, last_name, email, department) VALUES
('John', 'Doe', 'jdoe@university.com', 'Computer Science'),
('Jane', 'Smith', 'jsmith@university.com', 'Mathematics'),
('Michael', 'Brown', 'mbrown@university.com', 'Physics'),
('Emily', 'Johnson', 'ejohnson@university.com', 'Chemistry');

-- Insert sample data into the courses table
INSERT INTO courses (course_name, professor_id) VALUES
('Introduction to Programming', 1),
('Calculus I', 2),
('Classical Mechanics', 3),
('Organic Chemistry', 4);

-- Insert sample data into the students table
INSERT INTO students (first_name, last_name, email, major) VALUES
('Alice', 'Green', 'aliceg@student.com', 'Computer Science'),
('Bob', 'White', 'bobw@student.com', 'Mathematics'),
('Charlie', 'Black', 'charlieb@student.com', 'Physics'),
('Diana', 'Blue', 'dianab@student.com', 'Chemistry');

-- Insert sample data into the time_logs table
INSERT INTO time_logs (student_id, course_id, log_date, hours_logged) VALUES
(1, 1, '2024-10-01', 4.5),
(1, 1, '2024-10-02', 3.0),
(2, 2, '2024-10-01', 2.0),
(2, 2, '2024-10-03', 2.5),
(3, 3, '2024-10-01', 3.0),
(4, 4, '2024-10-01', 4.0);

-- Insert sample data into the users table
INSERT INTO users (netID, password, role) VALUES
('aliceg01', 'password123', 'student'),
('bobw02', 'password123', 'student'),
('char03', 'password123', 'student'),
('dianab04', 'password123', 'student'),
('profjd', 'profpass123', 'professor'),
('admin01', 'adminpass123', 'admin');
