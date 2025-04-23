// src/lib/jwt.ts
import jwt from 'jsonwebtoken';

// Function to create JWT
export function createJwtToken(userId: string) {
  const payload = { userId };
  const secretKey = process.env.JWT_SECRET || 'default-secret-key'; // Use your secret key from .env
  const options = { expiresIn: '1h' };

  return jwt.sign(payload, secretKey, options);
}

