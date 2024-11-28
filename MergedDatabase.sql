-- Users Table as a Central Connection Table
CREATE TABLE users (
    user_id INT PRIMARY KEY AUTO_INCREMENT,
    utd_id CHAR(10) UNIQUE NOT NULL,
    net_id CHAR(9) UNIQUE NOT NULL,
    password VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    first_name VARCHAR(50) NOT NULL,
    user_role ENUM('Student', 'Professor') NOT NULL,
    CHECK (user_role IN ('Student', 'Professor'))
);

-- Professors Table referencing Users Table
CREATE TABLE professors (
    professor_id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE,
    UNIQUE (user_id)
);

-- Students Table referencing Users Table
CREATE TABLE students (
    student_id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE,
    UNIQUE (user_id)
);

-- Courses Table with Foreign Key for Professors
CREATE TABLE courses (
    course_id INT PRIMARY KEY AUTO_INCREMENT,
    course_code VARCHAR(10) UNIQUE NOT NULL,
    course_name VARCHAR(100) NOT NULL,
    professor_id INT,
    FOREIGN KEY (professor_id) REFERENCES professors(professor_id)
);

-- Course Enrollments for Associating Students with Courses
CREATE TABLE course_enrollments (
    enrollment_id INT PRIMARY KEY AUTO_INCREMENT,
    student_id INT NOT NULL,
    course_id INT NOT NULL,
    FOREIGN KEY (student_id) REFERENCES students(student_id),
    FOREIGN KEY (course_id) REFERENCES courses(course_id)
);

-- Time Logs Table linked to Students
CREATE TABLE time_logs (
    log_id INT PRIMARY KEY AUTO_INCREMENT,
    student_id INT NOT NULL,
    course_id INT NOT NULL,
    log_date DATE,
    minutes_logged INT,
    work_desc VARCHAR(150),
    UNIQUE KEY unique_log (student_id, log_date),
    FOREIGN KEY (student_id) REFERENCES students(student_id),
    FOREIGN KEY (course_id) REFERENCES courses(course_id)
);

-- Team Members Table with Course ID and Student ID
CREATE TABLE team_members (
    team_number INT,
    course_id INT,
    student_id INT,
    PRIMARY KEY (team_number, student_id),
    FOREIGN KEY (course_id) REFERENCES courses(course_id),
    FOREIGN KEY (student_id) REFERENCES students(student_id)
);

-- Peer Review System with Student-Specific Submissions and Ratings
CREATE TABLE peer_review (
    pr_id INT PRIMARY KEY AUTO_INCREMENT,
    start_date DATE,
    end_date DATE
);

CREATE TABLE pr_criteria (
    criteria_id INT PRIMARY KEY AUTO_INCREMENT,
    pr_id INT,
    criteria_desc VARCHAR(150),
    FOREIGN KEY (pr_id) REFERENCES peer_review(pr_id)
);

CREATE TABLE pr_submissions (
    submission_id INT PRIMARY KEY AUTO_INCREMENT,
    pr_id INT,
    submission_date DATE,
    submitter_id INT,
    FOREIGN KEY (pr_id) REFERENCES peer_review(pr_id),
    FOREIGN KEY (submitter_id) REFERENCES students(student_id)
);

CREATE TABLE pr_ratings (
    rating_id INT PRIMARY KEY AUTO_INCREMENT,
    submission_id INT,
    pr_id INT,
    criteria_id INT,
    from_student_id INT,
    for_student_id INT,
    rating INT,
    FOREIGN KEY (submission_id) REFERENCES pr_submissions(submission_id),
    FOREIGN KEY (pr_id) REFERENCES peer_review(pr_id),
    FOREIGN KEY (criteria_id) REFERENCES pr_criteria(criteria_id),
    FOREIGN KEY (from_student_id) REFERENCES students(student_id),
    FOREIGN KEY (for_student_id) REFERENCES students(student_id)
);