import { SmkVersion, ModuleType } from '../value-objects';
export interface Module {
    id: number;
    name: string;
    type: ModuleType;
    durationMonths: number;
    requiredInternships: number;
    requiredCourses: number;
    requiredProcedures: number;
    requiredShiftHours: number;
}
export interface Specialization {
    id: number;
    name: string;
    programCode: string;
    startDate: string;
    plannedEndDate: string;
    durationYears: number;
    smkVersion: SmkVersion;
    currentModuleId: number;
    modules: Module[];
}
//# sourceMappingURL=Specialization.d.ts.map