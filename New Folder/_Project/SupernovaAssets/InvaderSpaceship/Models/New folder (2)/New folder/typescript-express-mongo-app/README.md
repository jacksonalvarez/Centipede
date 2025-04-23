# TypeScript Express MongoDB App

This project is a TypeScript-based Express application that connects to MongoDB Atlas. It serves as a template for building web applications with a structured approach, utilizing TypeScript for type safety and Express for handling HTTP requests.

## Project Structure

```
typescript-express-mongo-app
├── src
│   ├── app.ts               # Main application file
│   ├── server.ts            # Entry point for starting the server
│   ├── config
│   │   └── database.ts      # MongoDB connection logic
│   ├── routes
│   │   └── index.ts         # Route definitions
│   ├── controllers
│   │   └── index.ts         # Controller functions for handling requests
│   ├── utils
│   │   └── showSection.ts    # Page/component information for showSection function
│   └── types
│       └── index.ts         # TypeScript interfaces and types
├── package.json              # npm configuration file
├── tsconfig.json             # TypeScript configuration file
├── .env                      # Environment variables
└── README.md                 # Project documentation
```

## Getting Started

### Prerequisites

- Node.js (version 14 or higher)
- npm (Node Package Manager)
- MongoDB Atlas account

### Installation

1. Clone the repository:
   ```
   git clone <repository-url>
   cd typescript-express-mongo-app
   ```

2. Install dependencies:
   ```
   npm install
   ```

3. Set up your MongoDB Atlas database and create a `.env` file in the root directory with the following content:
   ```
   MONGODB_URI=<your-mongodb-uri>
   DB_NAME=<your-database-name>
   ```

### Running the Application

To start the server, run the following command:
```
npm run start
```

The application will be running on `http://localhost:3000`.

### Usage

- The application has various routes defined in `src/routes/index.ts`.
- Controllers in `src/controllers/index.ts` handle the logic for each route.
- The `showSection` utility in `src/utils/showSection.ts` manages the visibility of different sections in the application.

### Contributing

Contributions are welcome! Please open an issue or submit a pull request for any enhancements or bug fixes.

### License

This project is licensed under the MIT License. See the LICENSE file for more details.