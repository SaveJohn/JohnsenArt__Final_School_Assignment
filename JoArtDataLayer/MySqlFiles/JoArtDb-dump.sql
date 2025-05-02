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

DROP TABLE IF EXISTS Admins;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE Admins (
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

LOCK TABLES Admins WRITE;
INSERT INTO Admins (`AdminId`, `Email`, `Name`, `HashedPassword`, `Role`) VALUES (1,'admin@mail.com','Admin','$2a$11$E3H1OJ/1HMc5JskhDBOAYeEmuKXztbCyRG3TJ7J4HEF8d4W9rnC1W','Admin'),(2,'test@mail.com','Tester','$2y$10$.4l4Sd4Et647qIgEhw6TW.O0Hg1qvrGCT/aWMWAyxa0TaFIvjKT4S','Test');
UNLOCK TABLES;

--
-- Table structure for table `artworkimages`
--

DROP TABLE IF EXISTS ArtworkImages;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE ArtworkImages (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FullViewKey` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ThumbnailKey` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ArtworkId` int NOT NULL,
  `PreviewKey` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ArtworkImages_ArtworkId` (`ArtworkId`),
  CONSTRAINT `FK_ArtworkImages_Artworks_ArtworkId` FOREIGN KEY (`ArtworkId`) REFERENCES Artworks (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=251 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `artworkimages`
--

LOCK TABLES ArtworkImages WRITE;
INSERT INTO ArtworkImages (`Id`, `FullViewKey`, `ThumbnailKey`, `ArtworkId`, `PreviewKey`) VALUES (221,'fullView/61c9e2e6-8ffb-47a0-a096-b8f0203d1702.jpg','thumbs/0571e23e-1629-4f68-b4c5-40af3fcf59f9.jpg',15,'preview/75ba39d0-6187-4873-8a16-6d7ae8228512.jpg'),(222,'fullView/c7eb4b96-d55d-4dab-b1bc-68ccce9780dd.jpg','thumbs/202e292d-95b1-4057-a692-3a28e142a394.jpg',15,'preview/f5403158-eed6-4042-a9ae-7bcabbaa91b9.jpg'),(223,'fullView/69b9c39a-3b3a-4e57-9deb-6168423e77c9.jpg','thumbs/0d2e17db-aba1-4d4b-bbe9-ec471b1e339e.jpg',14,'preview/84f2cadc-634c-4cae-b224-386153604c17.jpg'),(224,'fullView/83f610c2-bb08-43fe-966d-fbc7cbf57ed1.jpg','thumbs/e4c7e284-f049-4377-b8da-e256ef7a29f8.jpg',14,'preview/a9412a40-c80b-476a-8e53-3ea878e4fc8d.jpg'),(225,'fullView/8652c9a9-586b-45b5-9273-787930ee3151.jpg','thumbs/e08b09a3-8282-4c74-b70a-0a18725eccdb.jpg',13,'preview/1af36928-4d33-4cad-b0a6-7003712ee29c.jpg'),(226,'fullView/93bf0bb6-a154-410a-bddd-3026d9480a84.jpg','thumbs/b7c63d52-99cb-4e12-b168-7d2ffdc748f4.jpg',12,'preview/88aac9b6-cdd5-4b9e-a4fe-e72d30775c8b.jpg'),(227,'fullView/028b3303-f983-4b06-965c-f1b0a67ced05.jpg','thumbs/1acd76fa-4e4e-44aa-8be2-926179ed8384.jpg',12,'preview/dd2d0887-25b9-4160-a41a-f345ee44a2f4.jpg'),(228,'fullView/2c02e4fb-6982-49c2-a617-559065b1d8ab.jpg','thumbs/8ce6df6a-5751-4570-aa8b-06dda5714511.jpg',11,'preview/f9d1526d-7316-447f-bba6-a5bbaffa4edd.jpg'),(229,'fullView/81aaef4c-8a7f-4e52-835c-007a7f76e95e.jpg','thumbs/05ade750-4efc-4c98-8628-879698e389fc.jpg',11,'preview/ffded746-53b5-459a-a2d6-41f3322638a1.jpg'),(230,'fullView/9c0bc6c5-49de-48ed-9dd9-6b2c9cd89142.jpg','thumbs/0ff79416-75f0-4523-9ca3-d7902fcba8f6.jpg',10,'preview/417afa6f-ebe5-4dec-9666-1e314f1edbf5.jpg'),(231,'fullView/3914756f-4e16-4014-8fd8-0a854af66d3c.jpg','thumbs/b879d9b2-6a15-4f57-825f-f04eefb83a69.jpg',10,'preview/1b61020e-2519-4b28-b4e5-55ea90ab77f9.jpg'),(232,'fullView/29d9576d-e475-4071-ad39-39fec01b42dc.jpg','thumbs/085f239e-4e2a-4923-b9ac-6448eb82f821.jpg',9,'preview/581b4c4d-0551-41ad-9cf5-9bc52a50cd65.jpg'),(233,'fullView/d5dd9315-67ef-41d2-a3b1-fe6830b5d542.jpg','thumbs/9d39eeef-1a83-4925-b759-ccc9b664b991.jpg',8,'preview/7f296298-f012-4819-99d8-f16dbebf6d45.jpg'),(234,'fullView/7af1c0d2-0775-4c50-a5db-92dda1373e47.jpg','thumbs/84b0331c-2f3c-4330-8da0-289779c14c2f.jpg',7,'preview/d273838e-489e-4d6c-bbc6-ace3c68c327a.jpg'),(235,'fullView/ce38276a-2e4e-4cda-bf03-aa53a4d0b6ed.jpg','thumbs/636117a0-9f46-4ee8-ab68-a3e44572894a.jpg',6,'preview/b22dd89b-a3dd-4ab8-9fe2-1a6e705e91b6.jpg'),(236,'fullView/27df9747-2e4d-4cf3-8a0a-9699d0b09acc.jpg','thumbs/c40020b1-dedd-4f03-91ab-200e0345ed8d.jpg',6,'preview/c0c37cee-2024-43ba-9ba4-e1a4174f4f35.jpg'),(237,'fullView/b82c0a9f-c2d7-4e67-ae25-b7ae5643dad0.jpg','thumbs/77f79bdf-4909-457e-9f06-0270935ee535.jpg',5,'preview/488a3d0f-aba0-48c1-beb7-df2d0c620858.jpg'),(238,'fullView/40a14527-0acb-4571-864c-2300c2e270b2.jpg','thumbs/e24c90d0-3c96-4ee1-97d1-bd8127aaf2a3.jpg',5,'preview/dd6dd4fb-2d02-48e5-b4f6-565e354ee0f3.jpg'),(239,'fullView/d52b23d1-0ae2-4b4e-85f4-628f20682712.jpg','thumbs/05656469-cb6a-4123-93a1-8d229fa40a34.jpg',4,'preview/c1d79d6f-72d8-4720-b78a-2b995ba16e56.jpg'),(240,'fullView/c3501924-43aa-47c7-a412-2ff951e37e38.jpg','thumbs/28e41f8b-d409-4bca-b084-fcecc1455aa8.jpg',3,'preview/0eeeede2-b679-4cfb-bba6-811b8ddfaa69.jpg'),(241,'fullView/29614ef7-7fc2-4cf7-b244-c6648d1e6d9b.jpg','thumbs/db3b919b-1e85-4060-9237-67b4fa99f703.jpg',3,'preview/9c30eefc-31d9-4f96-b5c6-6b123179dfde.jpg'),(242,'fullView/e2d684c0-ca50-48f0-9a1c-5aad24a317c5.jpg','thumbs/053784cc-e13b-48fc-ac5e-fdf80788bf82.jpg',2,'preview/78b1be2c-1285-4511-9c51-35dcb0a8faf7.jpg'),(243,'fullView/3901215c-604d-4c37-89c3-912fe73c9f53.jpg','thumbs/c13c38d7-ee76-4975-8747-998c16afeb5f.jpg',2,'preview/7c806a1e-9bfd-4ee3-a0f2-bdb4171903b7.jpg'),(244,'fullView/294705c9-0cbb-4c62-95a8-708cf08bba0f.jpg','thumbs/5a0520bf-0835-4f54-9c68-b87f908f3b9c.jpg',1,'preview/25f2c54b-ce0f-471b-98c8-6478eaeeac2f.jpg'),(245,'fullView/76793b6d-7ac3-4a05-8676-80198858d7d2.jpg','thumbs/4c39921b-cddf-4c89-bced-303d525e8bf8.jpg',31,'preview/5b26fa45-ad82-4700-8ccb-704c59362b2d.jpg'),(246,'fullView/7ad3ba23-985c-4a3f-88f4-cfb671e8b344.jpg','thumbs/8773bc1d-54b0-4923-8063-5c54c5d03b72.jpg',31,'preview/12ca3938-0fdd-41f8-97e4-e519a18e9fc0.jpg'),(247,'fullView/82d6545f-5a69-4fbd-8830-20c6b289d0a1.jpg','thumbs/ae319c21-68fa-49cb-8598-0dd01102834b.jpg',30,'preview/526dcc8a-988c-420a-8d89-21d46c011318.jpg'),(248,'fullView/811d7a2a-3cfe-493c-aab9-57e0f02b3414.jpg','thumbs/4ef0c4d0-790f-42d5-82a8-5c4817c5d603.jpg',30,'preview/da8732bb-b31d-4ebc-8177-94d5a61a55ef.jpg'),(249,'fullView/2957a4b1-3dcb-46ad-8a17-b9c1a9804895.jpg','thumbs/7b10dc2b-1574-4877-9224-01cee25db15a.jpg',16,'preview/dd8ad4e5-f50c-4099-a936-f42846c993f5.jpg'),(250,'fullView/4761b3c9-e044-412a-9636-fae050e398d0.jpg','thumbs/cd331386-6594-468a-8a32-30215d288219.jpg',16,'preview/9fecdea4-f023-49ee-aa48-d2aef4455b0b.jpg');
UNLOCK TABLES;

--
-- Table structure for table `artworks`
--

DROP TABLE IF EXISTS Artworks;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE Artworks (
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

LOCK TABLES Artworks WRITE;
INSERT INTO Artworks (`Id`, `Title`, `Description`, `Artist`, `Price`, `HeightDimension`, `WidthDimension`, `ForSale`, `Materials`, `HomePageRotation`) VALUES (1,'Gyldent farvel ved klippene','Solen synker langsomt ned bak horisonten, og maler klippene i varme nyanser av rav og kobber. Havet ligger stille, som om det lytter til dagens siste pust. En sti av mørke steiner leder tankene utover, mot roen og det evige.','Bjarne Johnsen',NULL,70,50,0,'Akryl på lerret',1),(2,'Stille formiddag ved det hvite huset','En rolig bris stryker over det solfylte gresset, og gamle strømledninger strekker seg som minner fra en enklere tid. Blomstene i hagen nikker i stillhet, mens huset hviler i sin egen skygge – som om det venter på noen som snart skal komme hjem.','Bjarne Johnsen',2600.000000000000000000000000000000,40,30,1,'Akryl på lerret',0),(3,'Der vannet synger','Et brusende slør faller fra himmelens rand, og farger vannet i grønt, blått og gyllent. Naturen står stille i ærefrykt, mens fossen taler med kraft og nåde – en evig sang om frihet og fall.','Bjarne Johnsen',3200.000000000000000000000000000000,60,45,1,'Akryl på lerret',0),(4,'Skum og flammer i horisonten','Bølgene kaster seg vilt mot land og eksploderer i skum, som om havet prøver å rope høyere enn solnedgangen i det fjerne. En ensom måke hviler blant klippene, uberørt av stormen – et øyeblikk av fred midt i alt det ville.','Bjarne Johnsen',NULL,40,60,0,'Akryl på lerret',0),(5,'Vinterglød','Solen står lavt og brenner som gull over den frosne skogen. Snøen reflekterer lyset i blått og fiolett, og trærne står som flammer og skygger i det tause landskapet. En enslig gran bærer kulden med verdighet – badet i det siste lyset før mørket faller.','Bjarne Johnsen',2700.000000000000000000000000000000,50,40,1,'Akryl på lerret',1),(6,'Treets hemmelige samtaler','Tre trær står samlet ved elvebredden, som gamle venner i dyp samtale. Gresset vaier og blomstene hvisker, mens horisonten strekker seg rolig utover under en disig himmel. Et øyeblikk av fred i naturens tidløse rytme.','Bjarne Johnsen',2400.000000000000000000000000000000,45,65,1,'Akryl på lerret',1),(7,'Gulroten i vinden','En lavtvoksende busk brer ut sine gyldne blader midt på enga, badet i sol og omgitt av fjell og elv. Landskapet holder pusten, mens fargene hvisker om sensommerens varme og naturens egen hyllest til lyset.','Bjarne Johnsen',NULL,35,55,0,'Akryl på lerret',1),(8,'Lyset som åpner verden','En bred elv eller stille fjord leder blikket mot horisonten der lyset brer seg ut som vinger over landskapet. Fjellene står i snøblått i det fjerne, og bakken er pyntet med dryss av farger – som om jorden selv har pyntet seg for øyeblikket. Stillhet. Åpenhet. Et løfte.','Bjarne Johnsen',NULL,60,90,0,'Akryl på lerret',0),(9,'Vinden i manen','Blikket er årvåkent, ørene vendt mot lyden i det fjerne. Hesten står som et symbol på kraft og frihet, omgitt av gress og villblomster. Manen danser i vinden – som flammer mot himmelen. Et vesen i bevegelse, selv i stillheten.','Bjarne Johnsen',NULL,60,45,0,'Akryl på lerret',0),(10,'Skogens hemmelige gjester','To rådyr står i sollyset som siler inn mellom trærne, omgitt av blomster og grønne toner. Skogen åpner seg som en katedral av lys og ro, og i dette skjøre øyeblikket virker verden stillere – og større.','Bjarne Johnsen',3300.000000000000000000000000000000,60,45,1,'Akryl på lerret',0),(11,'Stille bekk, milde trær','En bekk slynger seg forsiktig gjennom landskapet, omgitt av trær som hvisker i grønne og fiolette toner. Lyset er diffust, som om dagen ennå ikke har bestemt seg for om den skal bli solrik eller grå. Alt hviler – alt er i balanse.','Bjarne Johnsen',1800.000000000000000000000000000000,40,30,1,'Akryl på lerret',0),(12,'Bak hagegjerdet','Bak det lyse gjerdet brer en ildrød busk seg ut, som om den vokter inngangen til en glemt hage. Solen siler ned gjennom trekronene og maler stien i lilla, blått og gull – en sommerdrøm fanget mellom skygge og lys.','Bjarne Johnsen',2800.000000000000000000000000000000,35,55,1,'Akryl på lerret',1),(13,'Veien inn i vinteren','Snødekte trær står som stille voktere langs en vei som forsvinner inn i lyset. Et rødlig uthus gir kontrast til det blå og hvite, og alt er pakket inn i et dempet vinterlys. En reise venter – kanskje hjem, kanskje et sted man ennå ikke har vært.','Bjarne Johnsen',NULL,45,60,0,'Akryl på lerret',0),(14,'Tulipaner i formiddagssol','Tre tulipaner folder seg stille ut i et klart glass, badet i mykt lys. Skarpe penselstrøk og duse skygger spiller sammen som minner om en rolig morgen. En enkel bukett – men full av liv.','Bjarne Johnsen',1500.000000000000000000000000000000,30,30,1,'Akryl på lerret',0),(15,'Seilas forbi skjærene','Bølgene bryter mot steinene i sprut og glans, mens en enslig seilbåt glir stille forbi i horisonten. Fargene i vannet veksler mellom smaragd og kobolt, og skjærene bader i sol. En hyllest til havets kraft og stillhet.','Bjarne Johnsen',3700.000000000000000000000000000000,50,70,1,'Akryl på lerret',1),(16,'Der lyset lander','En stripe av lys åpner seg over havet, som en himmelsk korridor. Ved strandkanten står en skikkelse med to hunder, vendt mot det åpne lyset som strømmer ned fra himmelen. Havet glitrer rundt dem i gyllent og sølv, som om hele landskapet holder pusten et øyeblikk. Et møte mellom ro, menneske og natur – akkurat der lyset lander.','Bjarne Johnsen',3600.000000000000000000000000000000,45,70,1,'Akryl på lerret',0),(30,'Dans i åkeren','Et ensomt tre bøyer seg lekent mot vinden over et gyllent landskap. Bak horisonten stiger åser i blått og fiolett, som drømmer malt på himmelen. Hele scenen dirrer av bevegelse og varme, som om jorden danser sin egen sommersang.','Bjarne Johnsen',3500.000000000000000000000000000000,50,70,1,'Akryl på lerret',1),(31,'Under drivende skyer','Over et fargerikt landskap driver skyer i tunge, brede strøk over himmelen. Lyset bryter gjennom mot horisonten og maler slettene i varme toner av gull, rødt og grønt. En reise under åpen himmel – der vær og jord møtes.','Bjarne Johnsen',3600.000000000000000000000000000000,70,50,1,'Akryl på lerret',1);
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-05-02 12:00:39
