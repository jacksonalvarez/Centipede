import { Router } from 'express';
import { getDashboard, getJournal, getMood, getTasks, getCalendar, getProjects, getGoals, getSettings } from '../controllers';

const router = Router();

export const setupRoutes = (app) => {
    router.get('/dashboard', getDashboard);
    router.get('/journal', getJournal);
    router.get('/mood', getMood);
    router.get('/tasks', getTasks);
    router.get('/calendar', getCalendar);
    router.get('/projects', getProjects);
    router.get('/goals', getGoals);
    router.get('/settings', getSettings);

    app.use('/api', router);
};