export interface CreateMedicalShiftRequest {
    internshipId: number;
    date: string;
    hours: number;
    minutes: number;
    location: string;
    year: number;
}
export interface UpdateMedicalShiftRequest {
    date?: string;
    hours?: number;
    minutes?: number;
    location?: string;
    year?: number;
}
export interface CreateProcedureRequest {
    internshipId: number;
    procedureCode: string;
    procedureName: string;
    performanceDate: string;
    operatorLevel: string;
    location: string;
    patientAge: number;
    patientSex: 'M' | 'K';
    additionalInfo?: string;
    icdCode?: string;
}
export interface UpdateProcedureRequest {
    procedureCode?: string;
    procedureName?: string;
    performanceDate?: string;
    operatorLevel?: string;
    location?: string;
    patientAge?: number;
    patientSex?: 'M' | 'K';
    additionalInfo?: string;
    icdCode?: string;
}
export interface CreateInternshipRequest {
    specializationId: number;
    moduleId: number;
    institutionName: string;
    departmentName: string;
    supervisorName: string;
    startDate: string;
    endDate: string;
}
export interface UpdateInternshipRequest {
    institutionName?: string;
    departmentName?: string;
    supervisorName?: string;
    startDate?: string;
    endDate?: string;
    moduleId?: number;
}
export interface CreateCourseRequest {
    specializationId: number;
    courseName: string;
    courseType: string;
    startDate: string;
    endDate: string;
    organizer: string;
    location: string;
    certificateNumber?: string;
    creditHours?: number;
    notes?: string;
}
export interface UpdateCourseRequest {
    courseName?: string;
    courseType?: string;
    startDate?: string;
    endDate?: string;
    organizer?: string;
    location?: string;
    certificateNumber?: string;
    creditHours?: number;
    notes?: string;
}
export interface CreateSelfEducationRequest {
    specializationId: number;
    activityType: string;
    title: string;
    description: string;
    date: string;
    creditHours: number;
    notes?: string;
}
export interface UpdateSelfEducationRequest {
    activityType?: string;
    title?: string;
    description?: string;
    date?: string;
    creditHours?: number;
    notes?: string;
}
//# sourceMappingURL=requests.d.ts.map