![JoArt Logo](JoArtClassLib/Assets/JohnsenArt-Transparent.png)

# Johnsen Art

Johnsen Art is a collaborative web application built by Sabina Johnsen and Sebastian K. Holmen as the final project ("
Prosjektoppgaven") at Gokstad Akademiet.  
It is an online store for showcasing and selling artwork, with full support for user interaction, payment, and admin
features.

Built with:

- **Blazor WebAssembly** for the frontend
- **.NET 8** and **Entity Framework Core** for the backend logic and data access
- **MySQL** as relational database
- **AWS S3** for storing uploaded images
- **Stripe** integration for handling card and Klarna payments
- **Docker** for local development and containerization
- **MailKit** for sending emails
- **xUnit** and **Moq** for unit and integration testing

## Features

- Public gallery showcasing artwork
- Detailed view for each artwork with dimensions, materials, and pricing
- Stripe integration for purchasing artwork via card or Klarna
- Responsive checkout flow with shipping options
- Admin access via login endpoint for uploading, updating, or deleting artwork
- Conditional UI elements shown only when authenticated
- Image storage handled via AWS S3
- Error handling and status feedback integrated across the UI

## Getting started

###### These steps will assume you have .NET SDK 8 and MySQL 8.0.35 (or later).
###### You will also need AWS CLI, but if you don't the steps will guide you. 
###### Docker is optional


# AWS Configuration

#### If AWS CLI is not downloaded, run these commands:

```bash

msiexec.exe /i https://awscli.amazonaws.com/AWSCLIV2.msi 
```

### Confirm with the following command:

```bash

aws --version
```

### The following AWS credentials will be valid until 21st of May.

### These IAM give rights to read, put and delete from the S3 bucket you have been assigned.

### As well as read from AWS Secrets Manager all secrets starting with Development_JoArtAPI

## Sensor:

**Open PowerShell and enter the following to set your aws settings**

```bash

aws configure set aws_access_key_id AKIA6ELKN637T44O67NG
aws configure set aws_secret_access_key i9FPp9sEb+f5oEkaHMGsKPbzUJVx8bkbNUtAplNx
aws configure set region eu-north-1

```

### From the JoArtAPI project location (the JoArtFolder in the solution folder), run these commands:

```bash

dotnet user-secrets init
dotnet user-secrets set "AwsS3Settings:BucketName" "joart-s3-sensor"

```

## Teacher:

**Open PowerShell and enter the following to set your aws settings**

```bash

aws configure set aws_access_key_id AKIA6ELKN637QO7ZQYAT
aws configure set aws_secret_access_key masV7x/ZbVCv6r1s7KVHHeP2nhgGS6VHLEay0JBA
aws configure set region eu-north-1

```

### From the JoArtAPI project, run these commands:

```bash

dotnet user-secrets init
dotnet user-secrets set "AwsS3Settings:BucketName" "joart-s3-teacher"

```

## Populate the MySQL database

### Run the following SQL scripts

[Database dump](JoArtDataLayer/MySqlFiles/joartdb_dump.sql)

[Set user privileges](JoArtDataLayer/MySqlFiles/JoArtDb-UserPriveleges.sql)

**The database dump contain a populated art gallery with dummy data (and real artwork from our customer)**

### To be able to test the mailservice later on, you can run this SQL query to make sure you receive the mails as admin:

```bash

UPDATE admins
SET Email = [Enter your email here]
WHERE Role = 'Admin'
```

#### This will also change your login email
#### If you don't change it your admin login for the API will be using "admin@email.com" as user name
#### Your login password will be just that: Password

# Running the project using Docker

1. ## Navigate to the project


2. ## Navigate to root

```bash

cd JohnsenArt
```

3. ## Spin up the backend API and MYSQL database

```bash

docker compose up --build -d
```

4. ## In a new terminal, spin up the Blazor frontend

```bash

cd JoArtGUI

dotnet run
```

5. ## Access the site

Open your browser and navigate to:

```bash

http:localhost:5008
```

## You may also run the project without the use of Docker by navigating to the project, and manually running both the JoArtGUI and JoArtAPI configuration

--

## Usage

### For Public Users

Visitors may:

- Browse all artwork
- Click an individual painting to view more details such as title, dimensions, materials, and price
- Choose to **buy the artwork immediately** via card or Klarna (no cart functionality)
- Select either **mail delivery** or **pickup** at a predefined location
- Get redirected to a custom **thank you screen** after successful payment

### Admin User

- A "Login" button is always visible in the top-right corner
- Upon successful login:
    - The button becomes **"Logout"**
    - On artwork detail pages, new options appear:
        - **Edit** the painting
        - **Delete** the painting
        - **Upload** new artwork

Admin functionality is locked behind authentication.  
**Authentication method:** 
- The API uses JWT for authentication. 
- The Client takes the jwt and stores it in a cookie.
  - The cookie will have HttpOnly = true and CookieSecurePolicy.always in production.


### About & Contact Pages

- The **About** page is to contain information about the artist and relevant picture, it has placeholder text
  for now
- The **Contact** page includes a form with:
    - Name
    - Email
    - Message

When submitted, the written message and mail is sent to a predefined e-mail address with the help of the third party
library **MailKit**

### To test the mailservice, you can run this SQL query to make sure you receive the mails as admin:

```bash

UPDATE admins
SET Email = [Enter your email here]
WHERE Role = 'Admin'
```

#### This will also change your login email

## API Endpoints

These are some of the most relevant API endpoints on the Johnsen Art web application

### Public Endpoints

- `GET /api/PublicGallery/artworks`  
  Returns all available artworks, with possibility for filtering, sorting, and pagination.


- `GET /api/PublicGallery/artworks/{id}`  
  Returns a specific artwork by ID, including metadata and image URL's.


- `GET /api/PublicGallery/artworks/{id}/neighbors`  
  Returns previous and next artwork based on current filter and sort order.


- `GET /api/PublicGallery/homePageRotation`  
  Returns a list of featured artworks for carousel display on homepage.

### Admin Endpoints (Authentication Required)

- `POST /admin/api/Gallery/upload-artwork`  
  Upload a new artwork along with dimensions and metadata.


- `PUT /admin/api/Gallery/update-artwork/{id}`  
  Update an existing artworks details or replace images.


- `DELETE /admin/api/Gallery/delete-artwork/{id}`  
  Delete an artwork by id.

### Stripe Endpoints

- `POST /api/Stripe/create-intent/{artworkId}`  
  Creates a payment intent for a given artwork (used during checkout).


- `GET /api/Stripe/publishable-key`  
  Returns the stripe publishable key for use in frontend.

--
--

## Testing

Our project includes tests to both unit and integration levels of the backend API's

**xUnit** is our chosen library for test declaration and assertion,
**Moq** is used to mock all required dependencies in our tests.

### Tests

#### Integration tests

- **UploadArtworkIntegrationTests**  
  This verifies the entire upload flow


- **UpdateArtworkIntegrationTests**  
  We confirm image updates


- **DeleteArtworkIntegrationTests**  
  This checks that deletion behaves as expected with both valid and invalid credentials.

#### Unit tests

- **UploadArtworkUnitTests**  
  Validates AWS upload calls and repository saves.

- **UpdateArtworkUnitTests**  
  Covers update logic, image replacement, deletion of old assets, and failure scenarios.

--

### Stripe Webhook Tests

#### Integration tests

- **StripeWebhookIntegration**  
  Verifies processing of a real `payment_intent.succeeded` webhook payload.  
  This confirms that the correct artwork is marked as sold, and that we recieve a 200 ok from the controller.

#### Unit tests

- **StripeWebhook_PaymentIntentSuccess**  
  Simulates a proper stripe event and checks that `MarkAsSoldAsync` is called.  
  Ensures artwork is marked as sold and returns 200 OK.

- **StripeWebhook_MissingSignature**  
  Ensures that if the signature header is missing the webhook returns a 400 bad request.


## License Notice

This application was developed as part of an educational project and gifted to the client for personal use. It is not
licensed for public distribution or commercial use. See the [LICENSE](./LICENSE) file for details.



