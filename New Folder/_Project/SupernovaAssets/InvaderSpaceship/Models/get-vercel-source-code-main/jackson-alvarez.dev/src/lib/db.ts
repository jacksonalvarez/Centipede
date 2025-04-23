import { MongoClient, Db } from 'mongodb';
import mongoose from 'mongoose';

// Store the connection instance
let client: MongoClient;
let db: Db | undefined; // Explicitly typing db as `Db | undefined`

// Use environment variable for MongoDB URI or fallback to a default URI
const uri = process.env.MONGODB_URI || "mongodb+srv://alvarezjd:Farts123123@mongodbsite.cwtstpa.mongodb.net/?retryWrites=true&w=majority&appName=MongoDBSite";

// Function to establish and return a database connection
async function connect(): Promise<Db> {
  // If the db is already connected, return it
  if (db) return db;

  try {
    if (!client) {
      // Initialize the client if not already done
      client = new MongoClient(uri);
    }
    try {
        await mongoose.connect(uri);
        console.log("Mongoose connected to MongoDB");
      } catch (error) {
        console.error("Failed to connect Mongoose to MongoDB:", error);
        throw new Error("Mongoose connection failed");
      }
    // Connect to the MongoDB server
    await client.connect();
    db = client.db(); // Use the default database
    console.log("Connected to MongoDB");

    return db;
  } catch (error) {
    console.error('Failed to connect to MongoDB:', error);
    throw new Error('Failed to connect to the database');
  }
}

  // Export the connect function to use it in other parts of the app
export { connect };