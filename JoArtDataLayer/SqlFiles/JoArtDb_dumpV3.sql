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
INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES ('20250408115125_update03','8.0.10');
UNLOCK TABLES;

--
-- Table structure for table `Admins`
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
-- Dumping data for table `Admins`
--

LOCK TABLES `Admins` WRITE;
INSERT INTO `Admins` (`AdminId`, `Email`, `Name`, `HashedPassword`) VALUES (1,'Admin','Admin','$2a$11$E3H1OJ/1HMc5JskhDBOAYeEmuKXztbCyRG3TJ7J4HEF8d4W9rnC1W');
UNLOCK TABLES;

--
-- Table structure for table `ArtworkImages`
--

DROP TABLE IF EXISTS ArtworkImages;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE ArtworkImages (
                                 `Id` int NOT NULL AUTO_INCREMENT,
                                 `ObjectKey` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
                                 `ThumbnailKey` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
                                 `IsWallPreview` tinyint(1) NOT NULL,
                                 `ArtworkId` int NOT NULL,
                                 PRIMARY KEY (`Id`),
                                 KEY `IX_ArtworkImages_ArtworkId` (`ArtworkId`),
                                 CONSTRAINT `FK_ArtworkImages_Artworks_ArtworkId` FOREIGN KEY (`ArtworkId`) REFERENCES `Artworks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ArtworkImages`
--

LOCK TABLES ArtworkImages WRITE;
INSERT INTO ArtworkImages (`Id`, `ObjectKey`, `ThumbnailKey`, `IsWallPreview`, `ArtworkId`) VALUES (4,'dad8fcfb-d42e-48e6-beaa-670d3739d6a9.jpeg','thumbs/bb78c30f-53dd-496e-821c-6e43405ffc9f.jpg',0,3),(5,'64524246-c24a-45e8-a048-1ee0365f1e3d.jpeg','thumbs/d88aca9e-ee0a-40df-b0b4-7f707e25e233.jpg',1,3),(6,'0b28d32c-e3d4-4bc3-bb0e-4179d7064fa0.jpeg','',0,2),(7,'69054858-e41b-43f1-ac2a-7e4f026bbf0c.jpeg','',1,2),(9,'077f757e-d2f4-4bee-93cd-9bd72f9c2130.jpeg','thumbs/da78e826-e242-4b6d-a3e5-1a17f38f4a7b.jpg',0,4),(10,'21a4da7e-9d7b-4717-929e-02adff7d24c2.jpeg','thumbs/1af965b6-075e-4701-a139-391f847b1f33.jpg',0,5),(11,'304fdd9e-1774-481e-b515-f0e99291fb25.jpeg','thumbs/0873e2e9-e728-4bb0-bae1-1bfbc5bc0d6b.jpg',1,5),(12,'1f119551-ec0d-46f7-a385-b1064742834a.jpeg','thumbs/c02acf86-7c75-44bb-b142-d992c2a31abc.jpg',0,6),(13,'2723e5e2-8855-4d25-b127-32677daea6f2.jpeg','thumbs/238481ad-afe2-44df-a5d6-e5bc8feb53da.jpg',1,6),(14,'b8cf994f-42bc-4c03-97f9-3fc51eb36ed4.jpeg','thumbs/d45bdfbf-bd9a-4161-b325-e2bbeae1a75a.jpg',0,7),(15,'e3a958d5-d711-4b33-8a36-8cb421048cb5.jpeg','thumbs/577b4a63-983f-4965-8bcd-d469ad4640a7.jpg',0,8),(16,'13103920-bda0-4c6c-90d5-f57f4816857c.jpeg','thumbs/d53b6276-d8ab-4b67-b7ac-4596149d2059.jpg',0,9),(17,'0f87460c-ac3d-44e9-b1da-8738b9979c16.jpeg','thumbs/dffb40cd-8531-466b-abc5-f2945ffcb892.jpg',0,10),(18,'0e356ed9-205e-437a-aa73-a8a1793b8eea.jpeg','thumbs/03f6e319-be4f-48fd-bfb1-a8141fda6bae.jpg',1,10),(19,'43fa651b-e862-4005-98b1-b389ccc67885.jpeg','thumbs/5e94d9f8-814d-4936-8f6e-fd02bef5f809.jpg',0,11),(20,'649648ab-82e2-498c-8ce9-576bea9e4d59.jpeg','thumbs/9f557134-7039-45a2-984d-23738e94ac70.jpg',1,11),(21,'9440d77f-4560-4a8b-a006-a7a241a4927c.jpeg','thumbs/87123747-4740-4541-ad8f-a3798ee72370.jpg',0,12),(22,'238c823e-826a-45a6-b7d8-04ba477296f4.jpeg','thumbs/53f7322f-b9d7-4624-b194-17a4dfb49493.jpg',1,12),(23,'1e84fa30-fd7d-4b8a-b119-89f04853e644.jpeg','thumbs/7ae509d6-eb8f-42fa-9cc9-eae19ce5ad83.jpg',0,13),(24,'96c95ca4-da77-4f4a-bafb-d4e32d08be71.jpeg','thumbs/a9866911-bd38-469b-83a9-0b6d9b76632d.jpg',0,14),(25,'2b877dac-eb07-43e0-99e6-09c35876f7a9.jpeg','thumbs/0fc9f4f8-1a04-467c-9601-273fe391d16c.jpg',1,14),(26,'fc8081d2-1ebe-42de-9f52-d0614cc03fcb.jpeg','thumbs/9a9c5504-9d11-465d-8a07-d5285943b78e.jpg',0,15),(27,'a32ec1bd-ce54-4ce3-b56a-ec882d30cf51.jpeg','thumbs/7496bdb7-0c68-4300-908e-78f47c23ad48.jpg',1,15),(28,'9eb24029-3173-4fa1-9682-ea6e7aa5bba7.jpeg','thumbs/59fd46ab-c789-4cba-9bcf-9d4af9ed07e3.jpg',0,16),(29,'7367cf6f-bd32-46f1-8d1c-1bb411d9b095.jpeg','thumbs/f149c4d2-f5ef-4642-b6e7-306ab5da23eb.jpg',1,16),(30,'871577e1-d038-4260-bbfc-0a62df86fce9.jpg','',0,1);
UNLOCK TABLES;

--
-- Table structure for table `Artworks`
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
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Artworks`
--

LOCK TABLES `Artworks` WRITE;
INSERT INTO `Artworks` (`Id`, `Title`, `Description`, `Artist`, `Price`, `HeightDimension`, `WidthDimension`, `ForSale`) VALUES (1,'Gyldent farvel ved klippene','Solen synker langsomt ned bak horisonten, og maler klippene i varme nyanser av rav og kobber. Havet ligger stille, som om det lytter til dagens siste pust. En sti av mørke steiner leder tankene utover, mot roen og det evige.','Bjarne Johnsen',NULL,70,50,0),(2,'Stille formiddag ved det hvite huset','En rolig bris stryker over det solfylte gresset, og gamle strømledninger strekker seg som minner fra en enklere tid. Blomstene i hagen nikker i stillhet, mens huset hviler i sin egen skygge – som om det venter på noen som snart skal komme hjem.','Bjarne Johnsen',2600.000000000000000000000000000000,40,30,1),(3,'Der vannet synger','Et brusende slør faller fra himmelens rand, og farger vannet i grønt, blått og gyllent. Naturen står stille i ærefrykt, mens fossen taler med kraft og nåde – en evig sang om frihet og fall.','Bjarne Johnsen',3200.000000000000000000000000000000,60,45,1),(4,'Skum og flammer i horisonten','Bølgene kaster seg vilt mot land og eksploderer i skum, som om havet prøver å rope høyere enn solnedgangen i det fjerne. En ensom måke hviler blant klippene, uberørt av stormen – et øyeblikk av fred midt i alt det ville.','Bjarne Johnsen',NULL,40,60,0),(5,'Vinterglød','Solen står lavt og brenner som gull over den frosne skogen. Snøen reflekterer lyset i blått og fiolett, og trærne står som flammer og skygger i det tause landskapet. En enslig gran bærer kulden med verdighet – badet i det siste lyset før mørket faller.','Bjarne Johnsen',2700.000000000000000000000000000000,50,40,1),(6,'Treets hemmelige samtaler','Tre trær står samlet ved elvebredden, som gamle venner i dyp samtale. Gresset vaier og blomstene hvisker, mens horisonten strekker seg rolig utover under en disig himmel. Et øyeblikk av fred i naturens tidløse rytme.','Bjarne Johnsen',2400.000000000000000000000000000000,45,65,1),(7,'Gulroten i vinden','En lavtvoksende busk brer ut sine gyldne blader midt på enga, badet i sol og omgitt av fjell og elv. Landskapet holder pusten, mens fargene hvisker om sensommerens varme og naturens egen hyllest til lyset.','Bjarne Johnsen',NULL,35,55,0),(8,'Lyset som åpner verden','En bred elv eller stille fjord leder blikket mot horisonten der lyset brer seg ut som vinger over landskapet. Fjellene står i snøblått i det fjerne, og bakken er pyntet med dryss av farger – som om jorden selv har pyntet seg for øyeblikket. Stillhet. Åpenhet. Et løfte.','Bjarne Johnsen',NULL,60,90,0),(9,'Vinden i manen','Blikket er årvåkent, ørene vendt mot lyden i det fjerne. Hesten står som et symbol på kraft og frihet, omgitt av gress og villblomster. Manen danser i vinden – som flammer mot himmelen. Et vesen i bevegelse, selv i stillheten.','Bjarne Johnsen',NULL,60,45,0),(10,'Skogens hemmelige gjester','To rådyr står i sollyset som siler inn mellom trærne, omgitt av blomster og grønne toner. Skogen åpner seg som en katedral av lys og ro, og i dette skjøre øyeblikket virker verden stillere – og større.','Bjarne Johnsen',3300.000000000000000000000000000000,60,45,1),(11,'Stille bekk, milde trær','En bekk slynger seg forsiktig gjennom landskapet, omgitt av trær som hvisker i grønne og fiolette toner. Lyset er diffust, som om dagen ennå ikke har bestemt seg for om den skal bli solrik eller grå. Alt hviler – alt er i balanse.','Bjarne Johnsen',1800.000000000000000000000000000000,40,30,1),(12,'Bak hagegjerdet','Bak det lyse gjerdet brer en ildrød busk seg ut, som om den vokter inngangen til en glemt hage. Solen siler ned gjennom trekronene og maler stien i lilla, blått og gull – en sommerdrøm fanget mellom skygge og lys.','Bjarne Johnsen',2800.000000000000000000000000000000,35,55,1),(13,'Veien inn i vinteren','Snødekte trær står som stille voktere langs en vei som forsvinner inn i lyset. Et rødlig uthus gir kontrast til det blå og hvite, og alt er pakket inn i et dempet vinterlys. En reise venter – kanskje hjem, kanskje et sted man ennå ikke har vært.','Bjarne Johnsen',NULL,45,60,0),(14,'Tulipaner i formiddagssol','Tre tulipaner folder seg stille ut i et klart glass, badet i mykt lys. Skarpe penselstrøk og duse skygger spiller sammen som minner om en rolig morgen. En enkel bukett – men full av liv.','Bjarne Johnsen',1500.000000000000000000000000000000,30,30,1),(15,'Seilas forbi skjærene','Bølgene bryter mot steinene i sprut og glans, mens en enslig seilbåt glir stille forbi i horisonten. Fargene i vannet veksler mellom smaragd og kobolt, og skjærene bader i sol. En hyllest til havets kraft og stillhet.','Bjarne Johnsen',3700.000000000000000000000000000000,50,70,1),(16,'Der lyset lander','En stripe av lys åpner seg over havet, som en himmelsk korridor. Ved strandkanten står en skikkelse med to hunder, vendt mot det åpne lyset som strømmer ned fra himmelen. Havet glitrer rundt dem i gyllent og sølv, som om hele landskapet holder pusten et øyeblikk. Et møte mellom ro, menneske og natur – akkurat der lyset lander.','Bjarne Johnsen',3600.000000000000000000000000000000,45,70,1);
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-04-08 14:53:08
