export interface Section {
    id: string;
    title: string;
    component: string;
}

export const sections: Section[] = [
    { id: 'dashboard', title: 'Dashboard', component: 'DashboardComponent' },
    { id: 'journal', title: 'Journal', component: 'JournalComponent' },
    { id: 'mood', title: 'Mood Tracker', component: 'MoodTrackerComponent' },
    { id: 'tasks', title: 'Tasks', component: 'TasksComponent' },
    { id: 'calendar', title: 'Calendar', component: 'CalendarComponent' },
    { id: 'projects', title: 'Projects', component: 'ProjectsComponent' },
    { id: 'goals', title: 'Goals', component: 'GoalsComponent' },
    { id: 'settings', title: 'Settings', component: 'SettingsComponent' },
];