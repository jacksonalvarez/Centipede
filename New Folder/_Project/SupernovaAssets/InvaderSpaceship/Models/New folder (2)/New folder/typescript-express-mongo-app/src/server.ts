import express from 'express';
import mongoose from 'mongoose';
import { connectToDatabase } from './config/database';
import app from './app';

const PORT = process.env.PORT || 5000;

const startServer = async () => {
    try {
        await connectToDatabase();
        app.listen(PORT, () => {
            console.log(`Server is running on http://localhost:${PORT}`);
        });
    } catch (error) {
        console.error('Error starting the server:', error);
        process.exit(1);
    }
};

startServer();