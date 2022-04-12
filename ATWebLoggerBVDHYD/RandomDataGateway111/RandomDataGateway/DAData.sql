CREATE DATABASE  IF NOT EXISTS `gateway` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `gateway`;
-- MySQL dump 10.13  Distrib 5.6.13, for Win32 (x86)
--
-- Host: localhost    Database: gateway
-- ------------------------------------------------------
-- Server version	5.6.16

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `b827eb593e42_data`
--

DROP TABLE IF EXISTS `b827eb593e42_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `b827eb593e42_data` (
  `IDData` int(11) NOT NULL AUTO_INCREMENT,
  `DateTimeData` datetime DEFAULT NULL,
  `Location_1` float DEFAULT NULL,
  `Low_Level_1` float DEFAULT NULL,
  `High_Level_1` float DEFAULT NULL,
  `Location_2` float DEFAULT NULL,
  `Low_Level_2` float DEFAULT NULL,
  `High_Level_2` float DEFAULT NULL,
  `Location_3` float DEFAULT NULL,
  `Low_Level_3` float DEFAULT NULL,
  `High_Level_3` float DEFAULT NULL,
  `Location_4` float DEFAULT NULL,
  `Low_Level_4` float DEFAULT NULL,
  `High_Level_4` float DEFAULT NULL,
  `Location_5` float DEFAULT NULL,
  `Low_Level_5` float DEFAULT NULL,
  `High_Level_5` float DEFAULT NULL,
  `Location_6` float DEFAULT NULL,
  `Low_Level_6` float DEFAULT NULL,
  `High_Level_6` float DEFAULT NULL,
  `Location_7` float DEFAULT NULL,
  `Low_Level_7` float DEFAULT NULL,
  `High_Level_7` float DEFAULT NULL,
  `Location_8` float DEFAULT NULL,
  `Low_Level_8` float DEFAULT NULL,
  `High_Level_8` float DEFAULT NULL,
  `Location_9` float DEFAULT NULL,
  `Low_Level_9` float DEFAULT NULL,
  `High_Level_9` float DEFAULT NULL,
  `Location_10` float DEFAULT NULL,
  `Low_Level_10` float DEFAULT NULL,
  `High_Level_10` float DEFAULT NULL,
  `Location_11` float DEFAULT NULL,
  `Low_Level_11` float DEFAULT NULL,
  `High_Level_11` float DEFAULT NULL,
  `Location_12` float DEFAULT NULL,
  `Low_Level_12` float DEFAULT NULL,
  `High_Level_12` float DEFAULT NULL,
  `Location_13` float DEFAULT NULL,
  `Low_Level_13` float DEFAULT NULL,
  `High_Level_13` float DEFAULT NULL,
  `Location_14` float DEFAULT NULL,
  `Low_Level_14` float DEFAULT NULL,
  `High_Level_14` float DEFAULT NULL,
  `Location_15` float DEFAULT NULL,
  `Low_Level_15` float DEFAULT NULL,
  `High_Level_15` float DEFAULT NULL,
  `Location_16` float DEFAULT NULL,
  `Low_Level_16` float DEFAULT NULL,
  `High_Level_16` float DEFAULT NULL,
  PRIMARY KEY (`IDData`)
) ENGINE=InnoDB AUTO_INCREMENT=283 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `b827eb593e42_data`
--

LOCK TABLES `b827eb593e42_data` WRITE;
/*!40000 ALTER TABLE `b827eb593e42_data` DISABLE KEYS */;
/*!40000 ALTER TABLE `b827eb593e42_data` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-02-05 10:34:51
