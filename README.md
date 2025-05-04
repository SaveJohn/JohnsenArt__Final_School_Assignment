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
- **MailKit** for sending transactional emails
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

###### These steps will assume you have .NET and Docker installed.

1. ## Clone the repository

```Bash

git clone https://github.com/SaveJohn/JohnsenArt.git
```

2. ## Navigate to root

```bash

cd JohnsenArt
```

3. ## Spin up the backend API and MYSQL database

```bash

docker compose up --build
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
**Authentication method:** _TODO!!

### About & Contact Pages

- The **About** page contains is to contain information about the artist and relevant picture, it has placeholder text
  for now
- The **Contact** page includes a form with:
    - Name
    - Email
    - Message

When submitted, the written message and mail is sent to a predefined e-mail address with the help of the third party
library **MailKit**

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

# AUTHENTICATION

## Lorem ipsum etc etc

## Known Limitations

- The stripe integration is in test mode, using a sandbox environment.
- The **about** page uses placeholder text.

## License Notice

This application was developed as part of an educational project and gifted to the client for personal use. It is not
licensed for public distribution or commercial use. See the [LICENSE](./LICENSE) file for details.



