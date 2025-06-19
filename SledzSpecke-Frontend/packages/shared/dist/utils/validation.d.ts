import { z } from 'zod';
export declare const emailSchema: z.ZodString;
export declare const passwordSchema: z.ZodString;
export declare const signInSchema: z.ZodObject<{
    email: z.ZodString;
    password: z.ZodString;
}, "strip", z.ZodTypeAny, {
    email: string;
    password: string;
}, {
    email: string;
    password: string;
}>;
export declare const signUpSchema: z.ZodEffects<z.ZodObject<{
    email: z.ZodString;
    password: z.ZodString;
    confirmPassword: z.ZodString;
    fullName: z.ZodString;
    smkVersion: z.ZodEnum<["old", "new"]>;
}, "strip", z.ZodTypeAny, {
    email: string;
    password: string;
    confirmPassword: string;
    fullName: string;
    smkVersion: "old" | "new";
}, {
    email: string;
    password: string;
    confirmPassword: string;
    fullName: string;
    smkVersion: "old" | "new";
}>, {
    email: string;
    password: string;
    confirmPassword: string;
    fullName: string;
    smkVersion: "old" | "new";
}, {
    email: string;
    password: string;
    confirmPassword: string;
    fullName: string;
    smkVersion: "old" | "new";
}>;
export declare const medicalShiftSchema: z.ZodObject<{
    date: z.ZodString;
    hours: z.ZodNumber;
    minutes: z.ZodNumber;
    location: z.ZodString;
    year: z.ZodNumber;
}, "strip", z.ZodTypeAny, {
    date: string;
    hours: number;
    minutes: number;
    location: string;
    year: number;
}, {
    date: string;
    hours: number;
    minutes: number;
    location: string;
    year: number;
}>;
//# sourceMappingURL=validation.d.ts.map