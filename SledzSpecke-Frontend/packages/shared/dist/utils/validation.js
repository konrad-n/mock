import { z } from 'zod';
export const emailSchema = z.string()
    .email('Nieprawidłowy adres email')
    .max(100, 'Email może mieć maksymalnie 100 znaków');
export const passwordSchema = z.string()
    .min(8, 'Hasło musi mieć minimum 8 znaków')
    .regex(/[A-Z]/, 'Hasło musi zawierać przynajmniej jedną wielką literę')
    .regex(/[a-z]/, 'Hasło musi zawierać przynajmniej jedną małą literę')
    .regex(/[0-9]/, 'Hasło musi zawierać przynajmniej jedną cyfrę')
    .regex(/[^A-Za-z0-9]/, 'Hasło musi zawierać przynajmniej jeden znak specjalny');
export const signInSchema = z.object({
    email: emailSchema,
    password: z.string().min(1, 'Hasło jest wymagane')
});
export const signUpSchema = z.object({
    email: emailSchema,
    password: passwordSchema,
    confirmPassword: z.string(),
    fullName: z.string()
        .min(2, 'Imię i nazwisko musi mieć minimum 2 znaki')
        .max(100, 'Imię i nazwisko może mieć maksymalnie 100 znaków'),
    smkVersion: z.enum(['old', 'new'])
}).refine((data) => data.password === data.confirmPassword, {
    message: 'Hasła muszą być identyczne',
    path: ['confirmPassword']
});
export const medicalShiftSchema = z.object({
    date: z.string().min(1, 'Data jest wymagana'),
    hours: z.number()
        .min(0, 'Liczba godzin nie może być ujemna')
        .max(24, 'Liczba godzin nie może przekraczać 24'),
    minutes: z.number()
        .min(0, 'Liczba minut nie może być ujemna')
        .max(59, 'Liczba minut nie może przekraczać 59'),
    location: z.string()
        .min(1, 'Miejsce jest wymagane')
        .max(500, 'Miejsce może mieć maksymalnie 500 znaków'),
    year: z.number()
        .min(1, 'Rok musi być większy od 0')
        .max(6, 'Rok nie może przekraczać 6')
});
//# sourceMappingURL=validation.js.map