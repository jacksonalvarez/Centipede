import jwt, { type Secret } from 'jsonwebtoken';
import dotenv from 'dotenv';

// Load environment variables from .env file
dotenv.config();

// JWT_SECRET from the environment, or a fallback secret key
const JWT_SECRET: Secret = process.env.JWT_SECRET ?? (() => {
  throw new Error("JWT_SECRET is not defined in environment variables");
})();

export interface DecodedToken {
  userId: string;
  exp: number; // JWT expiration time
  iat: number; // JWT issued at time
}

// Function to generate a JWT token
export const createJwtToken = (userId: string): string => {
  const payload = {
    userId, // Only userId is included in the payload
  };

  const options = { expiresIn: '8766h' }; // Token expiration set to 1 year (8766 hours)

  try {
    const token = jwt.sign(payload, JWT_SECRET, options);
    console.log('JWT token generated:', token); // Log token for debugging
    return token;
  } catch (error) {
    console.error('Error generating JWT token:', error);
    throw new Error('Error generating JWT token');
  }
};

// Function to verify the session token
export function verifySessionToken(token: string): DecodedToken {
  try {
    // Verify the token and cast the result to `DecodedToken`
    const decoded = jwt.verify(token, JWT_SECRET) as DecodedToken;

    // Validate the presence of `userId` in the token
    if (!decoded.userId) {
      throw new Error('Invalid token structure: Missing userId');
    }

    return decoded;
  } catch (error) {
    console.error('Error verifying token:', error);
    throw new Error('Invalid or expired token');
  }
}
