export interface DashboardProgress {
    completed: number;
    total: number;
    percentage: number;
}
export interface DashboardOverview {
    currentModule: string;
    moduleType: 'Basic' | 'Specialist';
    overallProgress: number;
    internships: DashboardProgress;
    courses: DashboardProgress;
    procedures: DashboardProgress;
    medicalShifts: {
        completed: number;
        total: number;
        hoursCompleted: number;
        hoursRequired: number;
        percentage: number;
    };
    selfEducation: {
        completed: number;
        creditHours: number;
    };
    publications: {
        total: number;
        firstAuthor: number;
        peerReviewed: number;
    };
}
//# sourceMappingURL=dashboard.d.ts.map