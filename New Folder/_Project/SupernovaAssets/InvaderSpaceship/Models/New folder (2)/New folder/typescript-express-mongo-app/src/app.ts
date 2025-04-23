import express from 'express';
import { json } from 'body-parser';
import { setupRoutes } from './routes/index';

const app = express();

// Middleware
app.use(json());

// Setup routes
setupRoutes(app);

export default app;