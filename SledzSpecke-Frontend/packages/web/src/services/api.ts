import axios, { AxiosInstance, AxiosRequestConfig } from 'axios';
import { AuthResponse, SignInRequest, SignUpRequest } from '@shared/types';

class ApiClient {
  private instance: AxiosInstance;

  constructor() {
    this.instance = axios.create({
      baseURL: '/api',
      headers: {
        'Content-Type': 'application/json'
      }
    });

    // Request interceptor to add auth token
    this.instance.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('authToken');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    // Response interceptor for error handling
    this.instance.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          localStorage.removeItem('authToken');
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }

  // Auth endpoints
  async signIn(data: SignInRequest): Promise<AuthResponse> {
    // Transform to match API expectations
    const response = await this.instance.post<AuthResponse>('/auth/sign-in', {
      Username: data.email,
      Password: data.password
    });
    return response.data;
  }

  async signUp(data: SignUpRequest): Promise<AuthResponse> {
    // Transform to match API expectations
    const response = await this.instance.post<AuthResponse>('/auth/sign-up', {
      Username: data.email,
      Password: data.password,
      Email: data.email,
      FullName: data.fullName,
      SmkVersion: {
        Value: data.smkVersion === 'new' ? 'New' : 'Old'
      },
      SpecializationId: 1 // TODO: This should be selectable
    });
    return response.data;
  }

  // Generic request methods
  async get<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.instance.get<T>(url, config);
    return response.data;
  }

  async post<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.instance.post<T>(url, data, config);
    return response.data;
  }

  async put<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.instance.put<T>(url, data, config);
    return response.data;
  }

  async delete<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.instance.delete<T>(url, config);
    return response.data;
  }
}

export const apiClient = new ApiClient();