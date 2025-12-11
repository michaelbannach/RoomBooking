// src/apiClient.js
const API_BASE = import.meta.env.VITE_API_BASE ?? "http://localhost:5135";

export async function authFetch(path, options = {}) {
    const token = localStorage.getItem("jwt");

    const headers = {
        "Content-Type": "application/json",
        ...(options.headers || {}),
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
    };

    const response = await fetch(`${API_BASE}${path}`, {
        ...options,
        headers,
    });


    return response;
}
