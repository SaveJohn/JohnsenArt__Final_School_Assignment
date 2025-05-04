-- MySQL dump 10.13  Distrib 8.0.35, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: joartdb
-- ------------------------------------------------------
-- Server version	8.0.35

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
INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES ('20250408115125_update03','8.0.10'),('20250413084926_update05','8.0.10'),('20250421214810_RemoveIsWallPreview','8.0.10'),('20250421221234_AddMaterials','8.0.10'),('20250422073446_AddImagePreviewSize','8.0.10'),('20250425081758_update06','8.0.10'),('20250430200235_ObjectKeyNameMatch','8.0.10'),('20250501110338_RoleToAdmin','8.0.10');
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
  `Role` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`AdminId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `admins`
--

LOCK TABLES `Admins` WRITE;
INSERT INTO `Admins` (`AdminId`, `Email`, `Name`, `HashedPassword`, `Role`) VALUES (1,'admin@email.com','Admin','$2a$11$E3H1OJ/1HMc5JskhDBOAYeEmuKXztbCyRG3TJ7J4HEF8d4W9rnC1W','Admin'),(2,'test@mail.com','Tester','$2y$10$.4l4Sd4Et647qIgEhw6TW.O0Hg1qvrGCT/aWMWAyxa0TaFIvjKT4S','Test');
UNLOCK TABLES;

--
-- Table structure for table `artworkimages`
--

DROP TABLE IF EXISTS `ArtworkImages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ArtworkImages` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FullViewKey` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ThumbnailKey` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ArtworkId` int NOT NULL,
  `PreviewKey` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ArtworkImages_ArtworkId` (`ArtworkId`),
  CONSTRAINT `FK_ArtworkImages_Artworks_ArtworkId` FOREIGN KEY (`ArtworkId`) REFERENCES `artworks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=285 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `artworkimages`
--

LOCK TABLES `ArtworkImages` WRITE;
INSERT INTO `ArtworkImages` (`Id`, `FullViewKey`, `ThumbnailKey`, `ArtworkId`, `PreviewKey`) VALUES (252,'fullView/18420db4-f9b3-48d6-a283-5425d4d6312d.jpg','thumbs/181265ef-d594-4fa8-b031-ff73d7d1e9cd.jpg',30,'preview/b36d62f9-ffc9-4643-addd-8b7c2fb2e5d9.jpg'),(253,'fullView/ad1dd61d-daec-4327-8cfc-538d21b70a72.jpg','thumbs/ed3c79a1-695d-4126-8215-8776dc266ad2.jpg',16,'preview/df0fc9c9-4fd8-4fc3-a9ee-34e909784045.jpg'),(255,'fullView/81ac0d68-e08f-4504-bbd7-f6fda922edfa.jpg','thumbs/ac8748bd-cd2c-42ba-b690-55dd3d336f10.jpg',14,'preview/c4f3aa9b-6283-4d55-9f42-cc029cbe4fa8.jpg'),(256,'fullView/e009f6fc-3b15-4ce8-898c-f0540ddd05bb.jpg','thumbs/73aba414-16a9-4958-a96d-c708e19e635f.jpg',13,'preview/18dce2d3-b433-4540-a78b-09bca917a359.jpg'),(260,'fullView/a928ae3c-2699-4676-b152-fda096cf2cc1.jpg','thumbs/d64132ad-39e5-4507-b63a-1e79c50573fa.jpg',9,'preview/8839159f-1982-4772-b06d-33bbebba1c0e.jpg'),(261,'fullView/88e5e914-2f43-4e13-bf5f-498380179db1.jpg','thumbs/4f53593e-5e1e-4621-a1e7-f11bc3592163.jpg',8,'preview/9c1fb4b1-8c58-4961-b5ed-91fc534052e9.jpg'),(262,'fullView/f2d0122e-9545-4358-8e07-3686e3c301f2.jpg','thumbs/5c16af3a-7a02-45d7-8b8b-b85505ecf063.jpg',7,'preview/70964c66-9dd8-45f9-93bf-0244a46966cf.jpg'),(263,'fullView/24557f00-7f11-400b-a4ae-5a4febd31d7d.jpg','thumbs/9543cab2-c208-42f4-a8c0-91b334fabce7.jpg',6,'preview/637b690d-287f-4f1c-92c6-d0ccec1bced5.jpg'),(265,'fullView/3ade41ef-554a-43ab-95f7-f1305b815e75.jpg','thumbs/7391127c-b42f-448e-98f6-d7c398660150.jpg',4,'preview/8aa503b2-6d07-476b-bc6d-1876b7c4c388.jpg'),(266,'fullView/e5cf1b63-b363-40d0-91d9-27077930bc62.jpg','thumbs/f8d077ba-0783-4a4f-904c-0182f9b4d5f5.jpg',3,'preview/6adb7100-8c4e-46a6-8cd7-87c3aee35fc1.jpg'),(269,'fullView/fef0dc00-9508-4fce-8928-aef84e2543e6.jpg','thumbs/4d8669eb-7fd3-4d33-ac2c-caed207e8f8c.jpg',1,'preview/791b571d-d425-45df-a83b-855c1a71ad78.jpg'),(270,'fullView/e8997cd0-e8bc-497d-b11f-2048042449ac.jpg','thumbs/42666958-feed-4a7c-ab3c-2247e512897c.jpg',2,'preview/619a6821-7f46-4ef2-9ea4-552e0fd58c71.jpg'),(271,'fullView/78436a57-2437-4c26-abca-829d0f9978f9.jpg','thumbs/50de228d-68e6-41c0-a57b-1a4dc931e795.jpg',2,'preview/377aecb1-e210-4572-9ed5-8ac138fe45c9.jpg'),(272,'fullView/7f865ce4-82e3-489c-867d-7e48c9da2f08.jpg','thumbs/e1ccb8c2-a8b9-4f48-a202-fa8b98447b3f.jpg',5,'preview/4d0f0dec-767f-4169-a568-457d31c83f1a.jpg'),(273,'fullView/a362121b-f2d6-4d61-89b0-4857ac407104.jpg','thumbs/28158cf0-1b96-4e43-8557-d102271cc1c4.jpg',5,'preview/9eae1093-71c1-4f87-bd70-b6060d58e8c0.jpg'),(274,'fullView/998dcaa6-e2ad-4ebb-932f-0abd2f2dd410.jpg','thumbs/3ccc5f9c-16ba-4139-bac9-b59ff754ee25.jpg',10,'preview/8c233384-23fa-4228-9550-57ff13df10c1.jpg'),(275,'fullView/744b0875-ef7b-4284-8db1-6565dd582ac7.jpg','thumbs/611df1fd-d66a-4a11-991b-a421eb416f28.jpg',10,'preview/79863d15-94c0-4954-a61f-19707215c356.jpg'),(276,'fullView/36bc2e47-1d9d-4e7b-9538-e75933a507b5.jpg','thumbs/67eac029-2c00-4cc9-a0ac-1a9f5f11ec14.jpg',11,'preview/5aacb545-ed29-40b0-a34c-54a0cf0305a4.jpg'),(277,'fullView/a45c3824-c33f-491b-86f4-8bf59fe989d0.jpg','thumbs/299bd44c-6756-4d2a-9fc8-72134b03dbd1.jpg',11,'preview/85bdf7e0-cb3a-41ee-973e-d677558cee0c.jpg'),(280,'fullView/9efb59d8-6d2b-44dc-af2d-4d4c2ecc3b8b.jpg','thumbs/404cece1-383b-4ef0-b121-f9686ede5fa0.jpg',12,'preview/88f08101-5498-4781-965f-d126d67db7f7.jpg'),(281,'fullView/05cd8398-fd8c-4804-aa8a-789675ae3189.jpg','thumbs/c06cc91f-0935-4c95-9bad-f35a455de33d.jpg',12,'preview/872c8b61-2651-4c22-841b-ee99ebc955ab.jpg'),(282,'fullView/18f5183c-70e2-4da9-8f36-38d94659b650.jpg','thumbs/d2bdc609-1a86-45c9-bf33-313d83c1c31b.jpg',15,'preview/64c74a4f-9e08-4740-8831-f55b2828d3ac.jpg'),(283,'fullView/be1fc997-aec8-4b0b-b89d-f212d5a684e8.jpg','thumbs/6e2fe2dc-6bfd-46c1-9e08-a5dc3365268a.jpg',31,'preview/ce5853c0-6916-4a56-b3dc-535f17e540b1.jpg'),(284,'fullView/a401b130-5357-4f01-aac4-2661924ef08d.jpg','thumbs/4d2ab4b5-b512-403f-8210-5627692c3ebe.jpg',31,'preview/19b27549-9550-45f2-b80b-63f99fc0f83f.jpg');
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
  `Materials` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `HomePageRotation` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=33 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `artworks`
--

LOCK TABLES `Artworks` WRITE;
INSERT INTO `Artworks` (`Id`, `Title`, `Description`, `Artist`, `Price`, `HeightDimension`, `WidthDimension`, `ForSale`, `Materials`, `HomePageRotation`) VALUES (1,'Gyldent farvel ved klippene','Solen synker langsomt ned bak horisonten, og maler klippene i varme nyanser av rav og kobber. Havet ligger stille, som om det lytter til dagens siste pust. En sti av mørke steiner leder tankene utover, mot roen og det evige.','Bjarne Johnsen',NULL,70,50,0,'Akryl på lerret',1),(2,'Stille formiddag ved det hvite huset','En rolig bris stryker over det solfylte gresset, og gamle strømledninger strekker seg som minner fra en enklere tid. Blomstene i hagen nikker i stillhet, mens huset hviler i sin egen skygge – som om det venter på noen som snart skal komme hjem.','Bjarne Johnsen',2600.000000000000000000000000000000,40,30,1,'Akryl på lerret',0),(3,'Der vannet synger','Et brusende slør faller fra himmelens rand, og farger vannet i grønt, blått og gyllent. Naturen står stille i ærefrykt, mens fossen taler med kraft og nåde – en evig sang om frihet og fall.','Bjarne Johnsen',NULL,60,45,0,'Akryl på lerret',0),(4,'Skum og flammer i horisonten','Bølgene kaster seg vilt mot land og eksploderer i skum, som om havet prøver å rope høyere enn solnedgangen i det fjerne. En ensom måke hviler blant klippene, uberørt av stormen – et øyeblikk av fred midt i alt det ville.','Bjarne Johnsen',NULL,40,60,0,'Akryl på lerret',0),(5,'Vinterglød','Solen står lavt og brenner som gull over den frosne skogen. Snøen reflekterer lyset i blått og fiolett, og trærne står som flammer og skygger i det tause landskapet. En enslig gran bærer kulden med verdighet – badet i det siste lyset før mørket faller.','Bjarne Johnsen',2700.000000000000000000000000000000,50,40,1,'Akryl på lerret',1),(6,'Treets hemmelige samtaler','Tre trær står samlet ved elvebredden, som gamle venner i dyp samtale. Gresset vaier og blomstene hvisker, mens horisonten strekker seg rolig utover under en disig himmel. Et øyeblikk av fred i naturens tidløse rytme.','Bjarne Johnsen',NULL,45,65,0,'Akryl på lerret',1),(7,'Gulroten i vinden','En lavtvoksende busk brer ut sine gyldne blader midt på enga, badet i sol og omgitt av fjell og elv. Landskapet holder pusten, mens fargene hvisker om sensommerens varme og naturens egen hyllest til lyset.','Bjarne Johnsen',NULL,35,55,0,'Akryl på lerret',1),(8,'Lyset som åpner verden','En bred elv eller stille fjord leder blikket mot horisonten der lyset brer seg ut som vinger over landskapet. Fjellene står i snøblått i det fjerne, og bakken er pyntet med dryss av farger – som om jorden selv har pyntet seg for øyeblikket. Stillhet. Åpenhet. Et løfte.','Bjarne Johnsen',NULL,60,90,0,'Akryl på lerret',0),(9,'Vinden i manen','Blikket er årvåkent, ørene vendt mot lyden i det fjerne. Hesten står som et symbol på kraft og frihet, omgitt av gress og villblomster. Manen danser i vinden – som flammer mot himmelen. Et vesen i bevegelse, selv i stillheten.','Bjarne Johnsen',NULL,60,45,0,'Akryl på lerret',0),(10,'Skogens hemmelige gjester','To rådyr står i sollyset som siler inn mellom trærne, omgitt av blomster og grønne toner. Skogen åpner seg som en katedral av lys og ro, og i dette skjøre øyeblikket virker verden stillere – og større.','Bjarne Johnsen',3300.000000000000000000000000000000,60,45,1,'Akryl på lerret',0),(11,'Stille bekk, milde trær','En bekk slynger seg forsiktig gjennom landskapet, omgitt av trær som hvisker i grønne og fiolette toner. Lyset er diffust, som om dagen ennå ikke har bestemt seg for om den skal bli solrik eller grå. Alt hviler – alt er i balanse.','Bjarne Johnsen',1800.000000000000000000000000000000,40,30,1,'Akryl på lerret',0),(12,'Bak hagegjerdet','Bak det lyse gjerdet brer en ildrød busk seg ut, som om den vokter inngangen til en glemt hage. Solen siler ned gjennom trekronene og maler stien i lilla, blått og gull – en sommerdrøm fanget mellom skygge og lys.','Bjarne Johnsen',3700.000000000000000000000000000000,35,55,1,'Akryl på lerret',1),(13,'Veien inn i vinteren','Snødekte trær står som stille voktere langs en vei som forsvinner inn i lyset. Et rødlig uthus gir kontrast til det blå og hvite, og alt er pakket inn i et dempet vinterlys. En reise venter – kanskje hjem, kanskje et sted man ennå ikke har vært.','Bjarne Johnsen',NULL,45,60,0,'Akryl på lerret',0),(14,'Tulipaner i formiddagssol','Tre tulipaner folder seg stille ut i et klart glass, badet i mykt lys. Skarpe penselstrøk og duse skygger spiller sammen som minner om en rolig morgen. En enkel bukett – men full av liv.','Bjarne Johnsen',NULL,30,30,0,'Akryl på lerret',0),(15,'Seilas forbi skjærene','Bølgene bryter mot steinene i sprut og glans, mens en enslig seilbåt glir stille forbi i horisonten. Fargene i vannet veksler mellom smaragd og kobolt, og skjærene bader i sol. En hyllest til havets kraft og stillhet.','Bjarne Johnsen',3700.000000000000000000000000000000,50,70,1,'Akryl på lerret',1),(16,'Der lyset lander','En stripe av lys åpner seg over havet, som en himmelsk korridor. Ved strandkanten står en skikkelse med to hunder, vendt mot det åpne lyset som strømmer ned fra himmelen. Havet glitrer rundt dem i gyllent og sølv, som om hele landskapet holder pusten et øyeblikk. Et møte mellom ro, menneske og natur – akkurat der lyset lander.','Bjarne Johnsen',NULL,45,70,0,'Akryl på lerret',0),(30,'Dans i åkeren','Et ensomt tre bøyer seg lekent mot vinden over et gyllent landskap. Bak horisonten stiger åser i blått og fiolett, som drømmer malt på himmelen. Hele scenen dirrer av bevegelse og varme, som om jorden danser sin egen sommersang.','Bjarne Johnsen',NULL,50,70,0,'Akryl på lerret',1),(31,'Under drivende skyer','Over et fargerikt landskap driver skyer i tunge, brede strøk over himmelen. Lyset bryter gjennom mot horisonten og maler slettene i varme toner av gull, rødt og grønt. En reise under åpen himmel – der vær og jord møtes.','Bjarne Johnsen',3600.000000000000000000000000000000,70,50,1,'Akryl på lerret',1);
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-05-04 14:19:42
