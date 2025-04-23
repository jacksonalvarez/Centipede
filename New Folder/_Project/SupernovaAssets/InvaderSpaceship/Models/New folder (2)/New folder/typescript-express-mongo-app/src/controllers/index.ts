import { Request, Response } from 'express';

export const getDashboard = (req: Request, res: Response) => {
    res.send('Dashboard data');
};

export const getJournalEntries = (req: Request, res: Response) => {
    res.send('Journal entries data');
};

export const getMoodTracker = (req: Request, res: Response) => {
    res.send('Mood tracker data');
};

export const getTasks = (req: Request, res: Response) => {
    res.send('Tasks data');
};

export const getCalendar = (req: Request, res: Response) => {
    res.send('Calendar data');
};

export const getProjects = (req: Request, res: Response) => {
    res.send('Projects data');
};

export const getGoals = (req: Request, res: Response) => {
    res.send('Goals data');
};

export const getSettings = (req: Request, res: Response) => {
    res.send('Settings data');
};