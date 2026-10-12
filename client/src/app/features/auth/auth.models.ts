export interface RegisterRequest {
    displayName: string;
    email: string;
    password: string;
}

export interface LoginCredentails {
    email: string;
    password: string;
}

export interface AuthResponse {
    displayName: string;
    email: string;
    userName: string;
    token: string;
}

export interface User {
    displayName: string;
    userName: string;
    email: string;
}