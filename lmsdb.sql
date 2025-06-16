-- MySQL dump 10.13  Distrib 8.0.42, for Win64 (x86_64)
--
-- Host: localhost    Database: lmsdb
-- ------------------------------------------------------
-- Server version	8.0.42

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `answer`
--

DROP TABLE IF EXISTS `answer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `answer` (
  `AnswerID` int NOT NULL AUTO_INCREMENT,
  `SubmissionID` int DEFAULT NULL,
  `QuestionID` int DEFAULT NULL,
  `OptionID` int DEFAULT NULL,
  `AnswerText` text,
  `MarksAwarded` int DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `updated_by` int DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  PRIMARY KEY (`AnswerID`),
  KEY `SubmissionID` (`SubmissionID`),
  KEY `QuestionID` (`QuestionID`),
  KEY `OptionID` (`OptionID`),
  KEY `fk_answer_created_by` (`created_by`),
  KEY `fk_answer_updated_by` (`updated_by`),
  CONSTRAINT `answer_ibfk_1` FOREIGN KEY (`SubmissionID`) REFERENCES `examsubmission` (`SubmissionID`),
  CONSTRAINT `answer_ibfk_2` FOREIGN KEY (`QuestionID`) REFERENCES `question` (`QuestionID`),
  CONSTRAINT `answer_ibfk_3` FOREIGN KEY (`OptionID`) REFERENCES `optiontable` (`OptionID`),
  CONSTRAINT `fk_answer_created_by` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_answer_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `user` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `answer`
--

LOCK TABLES `answer` WRITE;
/*!40000 ALTER TABLE `answer` DISABLE KEYS */;
INSERT INTO `answer` VALUES (1,1,1,1,NULL,5,3,'2025-06-16 17:16:14',3,'2025-06-16 17:16:14'),(2,1,2,NULL,'Malware is software designed to harm a computer.',9,3,'2025-06-16 17:16:14',3,'2025-06-16 17:16:14');
/*!40000 ALTER TABLE `answer` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `course`
--

DROP TABLE IF EXISTS `course`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `course` (
  `CourseID` int NOT NULL AUTO_INCREMENT,
  `CourseName` varchar(255) DEFAULT NULL,
  `Description` text,
  `created_on` datetime DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `updated_by` int DEFAULT NULL,
  PRIMARY KEY (`CourseID`),
  KEY `fk_course_created_by` (`created_by`),
  KEY `fk_course_updated_by` (`updated_by`),
  CONSTRAINT `fk_course_created_by` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_course_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `user` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `course`
--

LOCK TABLES `course` WRITE;
/*!40000 ALTER TABLE `course` DISABLE KEYS */;
INSERT INTO `course` VALUES (1,'Web Development','Learn HTML, CSS, JS & backend basics','2025-06-16 17:04:51','2025-06-16 17:04:51',1,1),(2,'Cyber Security','Introduction to cyber threats and defense','2025-06-16 17:04:51','2025-06-16 17:04:51',1,1),(3,'Data Science','Python, Data Analysis, and Machine Learning','2025-06-16 17:04:51','2025-06-16 17:04:51',1,1);
/*!40000 ALTER TABLE `course` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `courseenrollments`
--

DROP TABLE IF EXISTS `courseenrollments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `courseenrollments` (
  `Enrollment_id` int NOT NULL AUTO_INCREMENT,
  `course_id` int DEFAULT NULL,
  `user_id` int DEFAULT NULL,
  `completion_status` varchar(50) DEFAULT NULL,
  `enrolled_on` datetime DEFAULT NULL,
  `completed_on` datetime DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `updated_by` int DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  PRIMARY KEY (`Enrollment_id`),
  KEY `course_id` (`course_id`),
  KEY `user_id` (`user_id`),
  KEY `fk_enrollments_created_by` (`created_by`),
  KEY `fk_enrollments_updated_by` (`updated_by`),
  CONSTRAINT `courseenrollments_ibfk_1` FOREIGN KEY (`course_id`) REFERENCES `course` (`CourseID`),
  CONSTRAINT `courseenrollments_ibfk_2` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_enrollments_created_by` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_enrollments_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `user` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `courseenrollments`
--

LOCK TABLES `courseenrollments` WRITE;
/*!40000 ALTER TABLE `courseenrollments` DISABLE KEYS */;
INSERT INTO `courseenrollments` VALUES (1,1,3,'Completed','2025-06-10 00:00:00','2025-06-20 00:00:00',1,1,'2025-06-16 17:15:41','2025-06-16 17:15:41'),(2,2,3,'Ongoing','2025-06-11 00:00:00',NULL,1,1,'2025-06-16 17:15:41','2025-06-16 17:15:41'),(3,3,3,'Not Started','2025-06-12 00:00:00',NULL,1,1,'2025-06-16 17:15:41','2025-06-16 17:15:41');
/*!40000 ALTER TABLE `courseenrollments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `exam`
--

DROP TABLE IF EXISTS `exam`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `exam` (
  `exam_id` int NOT NULL AUTO_INCREMENT,
  `exam_title` varchar(255) DEFAULT NULL,
  `topic_id` int DEFAULT NULL,
  `exam_date` date DEFAULT NULL,
  `duration` int DEFAULT NULL,
  `status` varchar(50) DEFAULT NULL,
  `max_marks` int DEFAULT NULL,
  `passing_marks` int DEFAULT NULL,
  `evaluator_id` int DEFAULT NULL,
  `instructor_id` int DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `updated_by` int DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  PRIMARY KEY (`exam_id`),
  KEY `topic_id` (`topic_id`),
  KEY `evaluator_id` (`evaluator_id`),
  KEY `instructor_id` (`instructor_id`),
  KEY `fk_exam_created_by` (`created_by`),
  KEY `fk_exam_updated_by` (`updated_by`),
  CONSTRAINT `exam_ibfk_1` FOREIGN KEY (`topic_id`) REFERENCES `topic` (`TopicID`),
  CONSTRAINT `exam_ibfk_2` FOREIGN KEY (`evaluator_id`) REFERENCES `user` (`user_id`),
  CONSTRAINT `exam_ibfk_3` FOREIGN KEY (`instructor_id`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_exam_created_by` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_exam_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `user` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `exam`
--

LOCK TABLES `exam` WRITE;
/*!40000 ALTER TABLE `exam` DISABLE KEYS */;
INSERT INTO `exam` VALUES (1,'HTML Basics Test',1,'2025-06-25',60,'Scheduled',100,40,2,2,1,'2025-06-16 17:09:01',1,'2025-06-16 17:09:01'),(2,'Cyber Threats Assessment',2,'2025-06-26',45,'Scheduled',50,20,2,2,1,'2025-06-16 17:09:01',1,'2025-06-16 17:09:01'),(3,'Python Data Handling',3,'2025-06-27',90,'Scheduled',100,50,2,2,1,'2025-06-16 17:09:01',1,'2025-06-16 17:09:01');
/*!40000 ALTER TABLE `exam` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `examresult`
--

DROP TABLE IF EXISTS `examresult`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `examresult` (
  `result_id` int NOT NULL AUTO_INCREMENT,
  `exam_id` int DEFAULT NULL,
  `user_id` int DEFAULT NULL,
  `marks_obtained` int DEFAULT NULL,
  `passed` tinyint(1) DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `updated_by` int DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  PRIMARY KEY (`result_id`),
  KEY `exam_id` (`exam_id`),
  KEY `user_id` (`user_id`),
  KEY `fk_result_created_by` (`created_by`),
  KEY `fk_result_updated_by` (`updated_by`),
  CONSTRAINT `examresult_ibfk_1` FOREIGN KEY (`exam_id`) REFERENCES `exam` (`exam_id`),
  CONSTRAINT `examresult_ibfk_2` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_result_created_by` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_result_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `user` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `examresult`
--

LOCK TABLES `examresult` WRITE;
/*!40000 ALTER TABLE `examresult` DISABLE KEYS */;
INSERT INTO `examresult` VALUES (1,1,3,85,1,3,'2025-06-16 17:16:06',3,'2025-06-16 17:16:06');
/*!40000 ALTER TABLE `examresult` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `examsubmission`
--

DROP TABLE IF EXISTS `examsubmission`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `examsubmission` (
  `SubmissionID` int NOT NULL AUTO_INCREMENT,
  `ExamID` int DEFAULT NULL,
  `user_id` int DEFAULT NULL,
  `SubmissionDate` datetime DEFAULT NULL,
  `TotalMarks` int DEFAULT NULL,
  `submittedBy` int DEFAULT NULL,
  `submittedAt` datetime DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `updated_by` int DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  PRIMARY KEY (`SubmissionID`),
  KEY `ExamID` (`ExamID`),
  KEY `user_id` (`user_id`),
  KEY `fk_submission_created_by` (`created_by`),
  KEY `fk_submission_updated_by` (`updated_by`),
  CONSTRAINT `examsubmission_ibfk_1` FOREIGN KEY (`ExamID`) REFERENCES `exam` (`exam_id`),
  CONSTRAINT `examsubmission_ibfk_2` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_submission_created_by` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_submission_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `user` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `examsubmission`
--

LOCK TABLES `examsubmission` WRITE;
/*!40000 ALTER TABLE `examsubmission` DISABLE KEYS */;
INSERT INTO `examsubmission` VALUES (1,1,3,'2025-06-25 00:00:00',100,3,'2025-06-16 17:15:55',3,'2025-06-16 17:15:55',3,'2025-06-16 17:15:55');
/*!40000 ALTER TABLE `examsubmission` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `optiontable`
--

DROP TABLE IF EXISTS `optiontable`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `optiontable` (
  `OptionID` int NOT NULL AUTO_INCREMENT,
  `QuestionID` int DEFAULT NULL,
  `OptionText` varchar(500) DEFAULT NULL,
  `IsCorrect` tinyint(1) DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `updated_by` int DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  PRIMARY KEY (`OptionID`),
  KEY `QuestionID` (`QuestionID`),
  KEY `fk_option_created_by` (`created_by`),
  KEY `fk_option_updated_by` (`updated_by`),
  CONSTRAINT `fk_option_created_by` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_option_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `optiontable_ibfk_1` FOREIGN KEY (`QuestionID`) REFERENCES `question` (`QuestionID`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `optiontable`
--

LOCK TABLES `optiontable` WRITE;
/*!40000 ALTER TABLE `optiontable` DISABLE KEYS */;
INSERT INTO `optiontable` VALUES (1,1,'Hyper Text Markup Language',1,1,'2025-06-16 17:15:14',1,'2025-06-16 17:15:14'),(2,1,'Home Tool Markup Language',0,1,'2025-06-16 17:15:14',1,'2025-06-16 17:15:14'),(3,1,'Hyperlinks and Text Markup Language',0,1,'2025-06-16 17:15:14',1,'2025-06-16 17:15:14');
/*!40000 ALTER TABLE `optiontable` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `question`
--

DROP TABLE IF EXISTS `question`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `question` (
  `QuestionID` int NOT NULL AUTO_INCREMENT,
  `ExamID` int DEFAULT NULL,
  `QuestionText` text,
  `QuestionType` varchar(50) DEFAULT NULL,
  `Marks` int DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `updated_by` int DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  PRIMARY KEY (`QuestionID`),
  KEY `ExamID` (`ExamID`),
  KEY `fk_question_created_by` (`created_by`),
  KEY `fk_question_updated_by` (`updated_by`),
  CONSTRAINT `fk_question_created_by` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_question_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `question_ibfk_1` FOREIGN KEY (`ExamID`) REFERENCES `exam` (`exam_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `question`
--

LOCK TABLES `question` WRITE;
/*!40000 ALTER TABLE `question` DISABLE KEYS */;
INSERT INTO `question` VALUES (1,1,'What does HTML stand for?','MCQ',5,1,'2025-06-16 17:09:40',1,'2025-06-16 17:09:40'),(2,2,'Define malware.','Descriptive',10,1,'2025-06-16 17:09:40',1,'2025-06-16 17:09:40'),(3,3,'What is a DataFrame in Pandas?','Descriptive',10,1,'2025-06-16 17:09:40',1,'2025-06-16 17:09:40');
/*!40000 ALTER TABLE `question` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rating`
--

DROP TABLE IF EXISTS `rating`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rating` (
  `RatingID` int NOT NULL AUTO_INCREMENT,
  `CourseID` int DEFAULT NULL,
  `user_id` int DEFAULT NULL,
  `Rating_Value` int DEFAULT NULL,
  `Feedback` text,
  `created_by` int DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `updated_by` int DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  PRIMARY KEY (`RatingID`),
  KEY `CourseID` (`CourseID`),
  KEY `user_id` (`user_id`),
  KEY `fk_rating_created_by` (`created_by`),
  KEY `fk_rating_updated_by` (`updated_by`),
  CONSTRAINT `fk_rating_created_by` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_rating_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `rating_ibfk_1` FOREIGN KEY (`CourseID`) REFERENCES `course` (`CourseID`),
  CONSTRAINT `rating_ibfk_2` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rating`
--

LOCK TABLES `rating` WRITE;
/*!40000 ALTER TABLE `rating` DISABLE KEYS */;
INSERT INTO `rating` VALUES (1,1,3,5,'Excellent course!',3,'2025-06-16 17:15:30',3,'2025-06-16 17:15:30'),(2,2,3,4,'Good content, well explained.',3,'2025-06-16 17:15:30',3,'2025-06-16 17:15:30'),(3,3,3,5,'Loved the hands-on exercises.',3,'2025-06-16 17:15:30',3,'2025-06-16 17:15:30');
/*!40000 ALTER TABLE `rating` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `session`
--

DROP TABLE IF EXISTS `session`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `session` (
  `session_id` int NOT NULL AUTO_INCREMENT,
  `course_id` int DEFAULT NULL,
  `topic_id` int DEFAULT NULL,
  `instructor_id` int DEFAULT NULL,
  `session_date` date DEFAULT NULL,
  `start_time` time DEFAULT NULL,
  `end_time` time DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `updated_by` int DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  PRIMARY KEY (`session_id`),
  KEY `course_id` (`course_id`),
  KEY `topic_id` (`topic_id`),
  KEY `instructor_id` (`instructor_id`),
  KEY `fk_session_created_by` (`created_by`),
  KEY `fk_session_updated_by` (`updated_by`),
  CONSTRAINT `fk_session_created_by` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_session_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `session_ibfk_1` FOREIGN KEY (`course_id`) REFERENCES `course` (`CourseID`),
  CONSTRAINT `session_ibfk_2` FOREIGN KEY (`topic_id`) REFERENCES `topic` (`TopicID`),
  CONSTRAINT `session_ibfk_3` FOREIGN KEY (`instructor_id`) REFERENCES `user` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `session`
--

LOCK TABLES `session` WRITE;
/*!40000 ALTER TABLE `session` DISABLE KEYS */;
INSERT INTO `session` VALUES (1,1,1,2,'2025-06-20','10:00:00','12:00:00',1,'2025-06-16 17:06:21',1,'2025-06-16 17:06:21'),(2,2,2,2,'2025-06-21','14:00:00','16:00:00',1,'2025-06-16 17:06:21',1,'2025-06-16 17:06:21'),(3,3,3,2,'2025-06-22','09:00:00','12:00:00',1,'2025-06-16 17:06:21',1,'2025-06-16 17:06:21');
/*!40000 ALTER TABLE `session` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `syllabus`
--

DROP TABLE IF EXISTS `syllabus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `syllabus` (
  `SyllabusID` int NOT NULL AUTO_INCREMENT,
  `Syllabus_name` varchar(255) DEFAULT NULL,
  `CourseID` int DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `updated_by` int DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  PRIMARY KEY (`SyllabusID`),
  KEY `CourseID` (`CourseID`),
  KEY `fk_syllabus_created_by` (`created_by`),
  KEY `fk_syllabus_updated_by` (`updated_by`),
  CONSTRAINT `fk_syllabus_created_by` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_syllabus_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `syllabus_ibfk_1` FOREIGN KEY (`CourseID`) REFERENCES `course` (`CourseID`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `syllabus`
--

LOCK TABLES `syllabus` WRITE;
/*!40000 ALTER TABLE `syllabus` DISABLE KEYS */;
INSERT INTO `syllabus` VALUES (1,'Frontend Basics',1,1,1,'2025-06-16 17:05:16','2025-06-16 17:05:16'),(2,'Cyber Security Fundamentals',2,1,1,'2025-06-16 17:05:16','2025-06-16 17:05:16'),(3,'Python for Data Science',3,1,1,'2025-06-16 17:05:16','2025-06-16 17:05:16');
/*!40000 ALTER TABLE `syllabus` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `topic`
--

DROP TABLE IF EXISTS `topic`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `topic` (
  `TopicID` int NOT NULL AUTO_INCREMENT,
  `TopicName` varchar(255) DEFAULT NULL,
  `Duration` int DEFAULT NULL,
  `Description` text,
  `SyllabusID` int DEFAULT NULL,
  `instructor_id` int DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `updated_by` int DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  PRIMARY KEY (`TopicID`),
  KEY `SyllabusID` (`SyllabusID`),
  KEY `instructor_id` (`instructor_id`),
  KEY `fk_topic_created_by` (`created_by`),
  KEY `fk_topic_updated_by` (`updated_by`),
  CONSTRAINT `fk_topic_created_by` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_topic_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `topic_ibfk_1` FOREIGN KEY (`SyllabusID`) REFERENCES `syllabus` (`SyllabusID`),
  CONSTRAINT `topic_ibfk_2` FOREIGN KEY (`instructor_id`) REFERENCES `user` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `topic`
--

LOCK TABLES `topic` WRITE;
/*!40000 ALTER TABLE `topic` DISABLE KEYS */;
INSERT INTO `topic` VALUES (1,'HTML & CSS',4,'Basic structure and styling of webpages',1,2,1,1,'2025-06-16 17:05:39','2025-06-16 17:05:39'),(2,'Ethical Hacking Basics',3,'Learn basics of ethical hacking and reconnaissance',2,2,1,1,'2025-06-16 17:05:39','2025-06-16 17:05:39'),(3,'NumPy & Pandas',5,'Data manipulation and analysis in Python',3,2,1,1,'2025-06-16 17:05:39','2025-06-16 17:05:39');
/*!40000 ALTER TABLE `topic` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `user_id` int NOT NULL AUTO_INCREMENT,
  `first_name` varchar(100) DEFAULT NULL,
  `last_name` varchar(100) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `phone_number` varchar(15) DEFAULT NULL,
  `date_of_birth` date DEFAULT NULL,
  `gender` varchar(20) DEFAULT NULL,
  `city` varchar(100) DEFAULT NULL,
  `state` varchar(100) DEFAULT NULL,
  `country` varchar(100) DEFAULT NULL,
  `department` varchar(100) DEFAULT NULL,
  `employment_status` varchar(100) DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  `username` varchar(100) DEFAULT NULL,
  `password` varchar(255) DEFAULT NULL,
  `duration` int DEFAULT NULL,
  `last_login` datetime DEFAULT NULL,
  `is_active` tinyint(1) DEFAULT NULL,
  `role_id` int DEFAULT NULL,
  `course_id` int DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `updated_by` int DEFAULT NULL,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `email` (`email`),
  UNIQUE KEY `username` (`username`),
  KEY `role_id` (`role_id`),
  CONSTRAINT `user_ibfk_1` FOREIGN KEY (`role_id`) REFERENCES `userrole` (`role_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,'Ava','Patel','ava@example.com','9876543210','1998-06-15','Female','Coimbatore','TN','India','IT','Active','2025-06-16 16:57:20','2025-06-16 16:57:20','ava123','pass123',0,'2025-06-16 16:57:20',1,1,NULL,NULL,NULL),(2,'Rahul','Kumar','rahul@example.com','9876543211','1995-08-20','Male','Mumbai','MH','India','Training','Active','2025-06-16 16:57:20','2025-06-16 16:57:20','rahulK','pass456',0,'2025-06-16 16:57:20',1,2,NULL,NULL,NULL),(3,'Sara','Ali','sara@example.com','9876543212','1999-12-01','Female','Delhi','DL','India','Learning','Active','2025-06-16 16:57:20','2025-06-16 16:57:20','saraA','pass789',0,'2025-06-16 16:57:20',1,3,NULL,NULL,NULL);
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `userrole`
--

DROP TABLE IF EXISTS `userrole`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `userrole` (
  `role_id` int NOT NULL AUTO_INCREMENT,
  `role_name` varchar(100) DEFAULT NULL,
  `description` text,
  `can_add_courses` tinyint(1) DEFAULT NULL,
  `can_schedule` tinyint(1) DEFAULT NULL,
  `can_evaluate` tinyint(1) DEFAULT NULL,
  `can_rate` tinyint(1) DEFAULT NULL,
  `can_enroll` tinyint(1) DEFAULT NULL,
  `is_admin` tinyint(1) DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `updated_by` int DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  PRIMARY KEY (`role_id`),
  KEY `fk_role_created_by` (`created_by`),
  KEY `fk_role_updated_by` (`updated_by`),
  CONSTRAINT `fk_role_created_by` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`),
  CONSTRAINT `fk_role_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `user` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `userrole`
--

LOCK TABLES `userrole` WRITE;
/*!40000 ALTER TABLE `userrole` DISABLE KEYS */;
INSERT INTO `userrole` VALUES (1,'Admin','Administrator role',1,1,1,1,1,1,1,1,'2025-06-16 16:47:11','2025-06-16 16:47:11'),(2,'Trainer','Trainer role',1,1,1,0,0,0,1,1,'2025-06-16 16:47:11','2025-06-16 16:47:11'),(3,'Learner','Learner role',0,0,0,1,1,0,1,1,'2025-06-16 16:47:11','2025-06-16 16:47:11');
/*!40000 ALTER TABLE `userrole` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'lmsdb'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-06-16 17:25:01
