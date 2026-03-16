import axios from 'axios';

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL || 'http://localhost:8080', // change this when transfer project into docker
    headers: {
        'Content-Type': 'application/json',
    },
});

// Автоматически подставляем JWT
api.interceptors.request.use((config) => {
    const token = localStorage.getItem('jwt');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export default api;

// const BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080';
//
// async function request(url, options = {}) {
//     const token = localStorage.getItem('jwt');
//
//     const headers = {
//         'Content-Type': 'application/json',
//         ...(options.headers || {}),
//     };
//
//     if (token) {
//         headers.Authorization = `Bearer ${token}`;
//     }
//
//     const response = await fetch(`${BASE_URL}${url}`, {
//         ...options,
//         headers,
//     });
//
//     // Аналог axios: выбрасываем ошибку на !2xx
//     if (!response.ok) {
//         let errorBody;
//         try {
//             errorBody = await response.json();
//         } catch {
//             errorBody = { message: response.statusText };
//         }
//
//         const error = new Error(errorBody.message || 'Request failed');
//         error.status = response.status;
//         error.data = errorBody;
//         throw error;
//     }
//
//     // axios возвращает response.data
//     if (response.status === 204) return null;
//
//     return response.json();
// }
//
// const api = {
//     get: (url, options = {}) =>
//         request(url, { ...options, method: 'GET' }),
//
//     post: (url, body, options = {}) =>
//         request(url, {
//             ...options,
//             method: 'POST',
//             body: JSON.stringify(body),
//         }),
//
//     put: (url, body, options = {}) =>
//         request(url, {
//             ...options,
//             method: 'PUT',
//             body: JSON.stringify(body),
//         }),
//
//     delete: (url, options = {}) =>
//         request(url, {
//             ...options,
//             method: 'DELETE',
//         }),
// };
//
// export default api;