-- MySQL dump 10.13  Distrib 8.0.34, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: joartdb
-- ------------------------------------------------------
-- Server version	8.0.34

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
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20250318215448_update02','8.0.10');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `admins`
--

DROP TABLE IF EXISTS `Admins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Admins` (
  `AdminId` int NOT NULL AUTO_INCREMENT,
  `Email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `HashedPassword` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`AdminId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `admins`
--

LOCK TABLES `Admins` WRITE;
/*!40000 ALTER TABLE `Admins` DISABLE KEYS */;
INSERT INTO `Admins` VALUES (1,'Admin','Admin','$2a$11$E3H1OJ/1HMc5JskhDBOAYeEmuKXztbCyRG3TJ7J4HEF8d4W9rnC1W');
/*!40000 ALTER TABLE `Admins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `artworkimages`
--

DROP TABLE IF EXISTS `ArtworkImages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ArtworkImages` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ObjectKey` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsWallPreview` tinyint(1) NOT NULL,
  `ArtworkId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ArtworkImages_ArtworkId` (`ArtworkId`),
  CONSTRAINT `FK_ArtworkImages_Artworks_ArtworkId` FOREIGN KEY (`ArtworkId`) REFERENCES `artworks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=65 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `artworkimages`
--

LOCK TABLES `Artworkimages` WRITE;
/*!40000 ALTER TABLE `Artworkimages` DISABLE KEYS */;
INSERT INTO `ArtworkImages` VALUES (29,'756d517d-a702-41ae-99c6-059e72bfb5b5.jpg',0,13),(30,'ed7eb0a7-4f6d-4a9a-85ed-c1d3cff34376.jpeg',1,13),(35,'7c337b7a-66e7-4e1f-aabe-c479e720d859.jpeg',0,16),(36,'ca598449-4551-4efd-a2f0-9a335e5d20e8.jpeg',1,16),(37,'329a05f2-79a4-4c43-91f4-ce3af9ca10f3.jpeg',0,17),(40,'35078a68-c112-42d2-921e-937efc894bf0.jpeg',0,18),(41,'7b2bebb3-c691-40c7-88a8-2d04991015bf.jpeg',1,18),(42,'8c237475-c04b-412d-a9dd-c0a1ff9dff99.jpeg',0,19),(43,'b7f2e75a-0e89-4433-983e-3149fb1da82b.jpeg',1,19),(44,'d6a5fe99-af0f-4910-8c00-432427bff625.jpeg',0,20),(45,'e69e30c4-0dc8-4f6d-a8cd-716222ce9879.jpeg',1,20),(46,'1fccfdeb-02c1-4519-bcbb-a521f3256548.jpeg',0,21),(47,'05d2cf58-10ed-4cc4-b3ce-ec1f257ba5e1.jpeg',1,21),(48,'aa60ec6c-3de6-46fc-8a8c-39b94c9cda45.jpeg',0,22),(49,'5d6c1786-a459-4182-b33c-161a77ec7966.jpeg',0,23),(50,'f01da2a8-0a54-41b1-b2b6-bd8d99bbd384.jpeg',1,23),(51,'4256ee0c-069a-47f2-b3d4-5cd9c18dbfc9.jpeg',0,24),(52,'7c666b23-5c65-463b-9c8f-5566927da43e.jpeg',1,24),(53,'5f16db27-c7a2-4bf0-aed8-42255554e310.jpeg',0,25),(54,'cb11c680-41cd-42e5-8865-8a9442ed740d.jpeg',1,25),(55,'61ad703b-c205-4acb-b043-4d79d8630109.jpeg',0,26),(56,'5718c98e-d4f0-4fd2-abb7-cd47f745982e.jpeg',1,26),(57,'e80596e6-2169-4e38-85bf-2e628e337f65.jpeg',0,27),(58,'abb724d0-838e-4937-861b-9d719ec81849.jpeg',1,27),(59,'bd7bc223-fc9f-4303-b950-ff5a42093131.jpeg',0,28),(60,'134ae0fa-6bb4-41f2-9d6a-de5981531771.jpeg',1,28),(61,'1f01487c-fbe9-468a-a307-447cea349ae9.jpeg',0,29),(62,'476aad88-dc6e-45c9-9da5-92ff05ecd0ff.jpeg',1,29),(63,'08594391-a7e1-466d-8494-a351be221a60.jpeg',0,30),(64,'21689851-3a0d-4091-b303-2a5ba5b98787.jpeg',1,30);
/*!40000 ALTER TABLE `Artworkimages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `artworks`
--

DROP TABLE IF EXISTS `Artworks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Artworks` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Artist` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Price` decimal(65,30) DEFAULT NULL,
  `HeightDimension` double DEFAULT NULL,
  `WidthDimension` double DEFAULT NULL,
  `ForSale` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `artworks`
--

LOCK TABLES `Artworks` WRITE;
/*!40000 ALTER TABLE `Artworks` DISABLE KEYS */;
INSERT INTO `Artworks` VALUES (13,'Sunset Beach','Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer feugiat lectus sit amet massa pretium, ut vestibulum lectus ultrices. Nullam hendrerit ornare condimentum. Mauris finibus lorem nulla, tristique maximus sem mattis at. Interdum et malesuada fames ac ante ipsum primis in faucibus. Vivamus ac sem eros. Morbi non mi ut odio fringilla imperdiet. Morbi mauris nisi, elementum a neque quis, finibus egestas est. Quisque quam massa, consequat ut risus at, molestie tincidunt nulla. Nunc et velit lorem. In in bibendum sem. Sed dignissim nulla vel tellus convallis pellentesque. Ut ex tellus, mollis hendrerit rhoncus vitae, elementum a orci. Aenean ornare ac dui vel rutrum. Quisque cursus ac massa non aliquet.','Bjarne Johnsen',9999.000000000000000000000000000000,234,345,1),(16,'Home','A place where I can go\r\nTo take this off my shoulders\r\nSomeone take me home\r\nHome\r\nA place where I can go\r\nTo take this off my shoulders\r\nSomeone take me home (let\'s go)\r\nSomeone take me','Bjarne Johnsen',NULL,234,345,0),(17,'Waterfall','Oh, when I\'m lonely and when I come undone\r\nRemember what you told me, \"Life\'s like a waterfall\"\r\nI know the only way to get along\r\nIs goin\' with the flow \'cause life\'s like a waterfall','Bjarne Johnsen',NULL,234,345,0),(18,'Crashing Waves','My heart is beating loud, in perfect rhythm There\'s beauty all around, that once was hidden! Waves crash all around me! Broken is how You found me','Bjarne Johnsen',3000.000000000000000000000000000000,153,435,1),(19,'Cold Sunset',' I\'m watching this sunset, colors bleed. Wishing you were here, right next to me. The sky\'s on fire, and I\'m feeling cold','Bjarne Johnsen',NULL,234,345,0),(20,'Three Little Trees','Three little trees,\r\nWhat do you see,\r\nAs you look down at the world from a mountain.\r\n\r\nThree little trees,\r\nWith three little dreams,\r\nAs they grow tall up on top of the mountain.\r\nOne hoped for beauty, one hoped for strength,\r\nOne hoped to point up to God in the heavens.\r\n\r\nThree little trees,\r\nAll loved by God,\r\nAs they grow tall up on top of the mountain.\r\n\r\nThree little trees,\r\nLike you and like me,\r\nAs we look down at the world from a mountaintop.','Bjarne Johnsen',2999.000000000000000000000000000000,234,345,1),(21,'The Lonely Tree','Promise we can meet. Underneath the Lonely Tree. When I had no one. You cared about me. When we were far apart. I always knew. You\'ll be there for me.','Bjarne Johnsen',NULL,234,345,0),(22,'Swans','The trees are in their autumn beauty,\r\nThe woodland paths are dry,\r\nUnder the October twilight the water\r\nMirrors a still sky;\r\nUpon the brimming water among the stones\r\nAre nine-and-fifty swans.\r\n\r\nThe nineteenth autumn has come upon me\r\nSince I first made my count;\r\nI saw, before I had well finished,\r\nAll suddenly mount\r\nAnd scatter wheeling in great broken rings\r\nUpon their clamorous wings.\r\n\r\nI have looked upon those brilliant creatures,\r\nAnd now my heart is sore.\r\nAll\'s changed since I, hearing at twilight,\r\nThe first time on this shore,\r\nThe bell-beat of their wings above my head,\r\nTrod with a lighter tread.\r\n \r\nUnwearied still, lover by lover,\r\nThey paddle in the cold\r\nCompanionable streams or climb the air;\r\nTheir hearts have not grown old;\r\nPassion or conquest, wander where they will,\r\nAttend upon them still.\r\n\r\nBut now they drift on the still water,\r\nMysterious, beautiful;\r\nAmong what rushes will they build,\r\nBy what lake\'s edge or pool\r\nDelight men\'s eyes when I awake some day\r\nTo find they have flown away?[','Bjarne Johnsen',NULL,234,345,0),(23,'Horse','His bridle hung around the post.\r\nThe sun and the leaves made spots come down;\r\nI looked close at him through the fence;\r\nThe post was drab and he was brown.','Bjarne Johnsen',1699.000000000000000000000000000000,234,345,1),(24,'Deer in the woods','Out of the mid-wood’s twilight\r\nInto the meadow’s dawn,\r\nIvory limbed and brown-eyed,\r\nFlashes my Faun!','Bjarne Johnsen',4000.000000000000000000000000000000,234,345,1),(25,'Colours Of Nature','The fine art of nature,\r\nColours that soothe,\r\nThe eye and the soul,\r\nThat changes the mood.\r\n\r\nThe soft green leaves,\r\nThe golden sand,\r\nHere for the taking,\r\nAs we walk hand in hand.\r\n\r\nThe bright blue sky,\r\nThe clouds of white,\r\nThe rich red rose,\r\nThat holds my heart tight.\r\n\r\nThe rust brown soil,\r\nBeneath my feet,\r\nNature so fine,\r\nColours so neat.\r\n\r\nA splash of colour,\r\nTo brighten one\'s day,\r\nThrough changing seasons,\r\nAlong the way.','Bjarne Johnsen',3999.000000000000000000000000000000,234,345,1),(26,'The Rose Bush','The roses grow by the old wooden fence,\r\nThey’ve been there for many years.\r\nWho took the time to plant the roses,\r\nWhose beauty lies so near.\r\n\r\nThe sun shines down upon the roses,\r\nCreating the perfect crimson hue.\r\nWho took the time to care for the roses,\r\nWhose beauty is so true.\r\n\r\nThe water pours down upon the roses,\r\nNourishing their every need.\r\nWho took the time to feed the roses,\r\nWhose beauty grew from a seed.\r\n\r\nIt was my mom who planted the roses.\r\nAnd gave them loving care.\r\nAnd now it’s my turn to watch over the roses,\r\nWhose beauty is so rare.','Bjarne Johnsen',4999.000000000000000000000000000000,234,345,1),(27,'The First Snow','The snow\r\nbegan here\r\nthis morning and all day\r\ncontinued, its white\r\nrhetoric everywhere\r\ncalling us back to why, how,\r\nwhence such beauty and what\r\nthe meaning; such\r\nan oracular fever! flowing\r\npast windows, an energy it seemed\r\nwould never ebb, never settle\r\nless than lovely! and only now,\r\ndeep into night,\r\nit has finally ended.\r\nThe silence\r\nis immense,\r\nand the heavens still hold\r\na million candles; nowhere\r\nthe familiar things:\r\nstars, the moon,\r\nthe darkness we expect\r\nand nightly turn from. Trees\r\nglitter like castles\r\nof ribbons, the broad fields\r\nsmolder with light, a passing\r\ncreekbed lies\r\nheaped with shining hills;\r\nand though the questions\r\nthat have assailed us all day\r\nremain - not a single\r\nanswer has been found -\r\nwalking out now\r\ninto the silence and the light\r\nunder the trees,\r\nand through the fields,\r\nfeels like one','Bjarne Johnsen',NULL,234,345,0),(28,'Tulips In A Glass Of Water','It\'s the yellow dust inside the tulips. It\'s the shape of a tulip. It\'s the water in the drinking glass the tulips are in. It\'s a day like any other.','Bjarne Johnsen',2569.000000000000000000000000000000,234,345,1),(29,'Sailing On The Ocean','Hvem kan seile foruten vind\r\nHvem kan ro uten årer\r\nHvem kan skilles fra vennen sin\r\nuten å felle tårer?\r\n\r\nJeg kan seile foruten vind\r\nJeg kan ro uten årer\r\nmen ei skilles fra vennen min\r\nuten å felle tårer!\r\n\r\nFuglen seiler foruten vind\r\nFisken ror uten årer\r\nMen ingen kan skilles fra vennen sin\r\nuten å felle tårer!','Bjarne Johnsen',5999.000000000000000000000000000000,234,345,1),(30,'Light in the Sky','You and I, with outstretched arms\r\ndared to reach the highest sky,\r\nwhere the sun concealed its light\r\n\r\nfrom our many dreams behind\r\nclouds of ebony steeped in\r\nMurphy\'s Law of misery.\r\n\r\nWe shook rainbows by their tails,\r\ngathered up the crumbs that fell,\r\ntill the storm came for your health.\r\n\r\nThe darkest of clouds set down\r\non your bones, claiming its home\r\nin your blood. So many times\r\n\r\nI\'ve awakened late at night\r\nto find you sitting bedside,\r\nhands on face, deftly wrapping\r\nthe nightly pain for my sake.\r\n\r\nYet, your smile touches me still,\r\nas real as these worn-out jeans\r\nthat you call my second skin,\r\nhand-woven from weathered dreams.\r\n\r\nSo much time has been wasted\r\nreaching for the sun and sky,\r\nwhile the brightest of all lights\r\nhas always shown in your eyes.','Bjarne Johnsen',8999.000000000000000000000000000000,234,345,1);

/*!40000 ALTER TABLE `Artworks` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-03-24 11:41:56
